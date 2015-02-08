using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameCore;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public string playerPostFix;

    public float force = 10;
    public float jump = 10;
    public float push = 2;
    public float gravity = 20;
    public float air = 0.5f;
    public float turn = 1;

    public Slippers slippers;

    public InAudioNode jumpSound;
    public InAudioNode deathSound;

    public float Force { get { return force * (slippers ? slippers.forceMultiplier : 1); } }
    public float Jump { get { return jump * (slippers ? slippers.jumpMultiplier : 1); } }
    public float Push { get { return push * (slippers ? slippers.pushMultiplier : 1); } }
    public float Gravity { get { return gravity * (slippers ? slippers.gravityMultiplier : 1); } }
    public float Air { get { return air * (slippers ? slippers.airMultiplier : 1); } }
    public float Turn { get { return turn * (slippers ? slippers.turnMultiplier : 1) - 1; } }

    public float AnimSpeed { get { return slippers ? slippers.animSpeed : 1; } }

    public bool isGrounded;

    private List<Vector3> ups;
    private Vector3 surfaceUp;
    private Vector3 lastForward;

    private Animator anim;

    [HideInInspector]
    public Color color;

    public Renderer playerColor;

    public Transform left, right;

    public Slippers baseSlippers;

    public Transform brokenBody;

    public float wobbleShift = 5;

	public int Id { get; private set; }
	// Use this for initialization
	IEnumerator Start ()
	{
	    anim = GetComponentInChildren<Animator>();
	    ups = new List<Vector3>();
	    lastForward = transform.forward;
	    Id = Int32.Parse(playerPostFix);

	    playerColor.material.color = color;

        var scoreUI = FindObjectOfType<PlayerScoreUI>();
        if (scoreUI)
        {
            scoreUI.Register(this);
        }

	    var firstShoes = Instantiate(baseSlippers, transform.position, Quaternion.identity) as Slippers;
	    firstShoes.collider.enabled = false;
	    yield return null;
        WearSlippers(firstShoes);
	}

    void Update()
    {
        if (GameInput.AllowInput)
        {
            if (isGrounded && Input.GetButtonDown("A_" + playerPostFix))
            {
                isGrounded = false;
                InAudio.Play(gameObject, jumpSound);
                rigidbody.AddForce(surfaceUp*Jump, ForceMode.Impulse);
            }
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        // calculate up vector + grounded
	    if (ups.Count == 0)
	    {
	        isGrounded = false;
	        surfaceUp = Vector3.Lerp(surfaceUp, Vector3.up, .05f);
	    }
	    else
        {
            isGrounded = true;
            Vector3 sum = new Vector3();
            sum = ups.Aggregate(sum, (current, up) => current + up);
            surfaceUp = Vector3.Lerp(surfaceUp, sum.normalized, .3f);
            ups.Clear();
        }

        // input
	    Vector3 direction = Vector3.zero;
        if (GameInput.AllowInput)
            direction = new Vector3(Input.GetAxis("L_XAxis_" + playerPostFix), 0, -Input.GetAxis("L_YAxis_" + playerPostFix));

        // rotate model to look forward
	    Vector3 forward = direction.normalized;
	    if (direction.sqrMagnitude < 0.2f)
	        forward = rigidbody.velocity.normalized;

	    forward = Vector3.ProjectOnPlane(Vector3.Lerp(lastForward, forward, .4f), surfaceUp).normalized;
	    lastForward = forward;

        transform.LookAt(transform.position + forward, surfaceUp);

        // faster turning
	    if (Turn > 0.01 && direction.sqrMagnitude > 0.2f)
	    {
	        float a = Vector3.Angle(direction, Vector3.ProjectOnPlane(rigidbody.velocity, Vector3.up));
	        direction += direction*(a/180)*Turn;
	    }


        //Debug.Log(rigidbody.velocity.magnitude);
	    if (direction.sqrMagnitude > 0.2f)
	    {
            anim.Play("Run");
            anim.speed = Mathf.Clamp01(direction.magnitude) * AnimSpeed;
        }
        else if (rigidbody.velocity.magnitude > wobbleShift)
        {
            anim.Play("WobbleWobble");
            anim.speed = 1;
        }
        else
        {
            anim.Play("Wobble");
            anim.speed = 1;
        }

        rigidbody.AddForce(direction * Force * (isGrounded ? 1 : Air));

        // gravity
        rigidbody.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

        //Debug.DrawRay(transform.position, rigidbody.velocity, Color.green);
	}

    void OnTriggerEnter(Collider col)
    {
        var flag = col.GetComponent<Flag>();
        if (flag)
        {
            flag.Capture(this);
            return;
        }

        var slip = col.GetComponent<Slippers>();
        if (slip && col.enabled)
        {
            col.enabled = false;
            WearSlippers(slip);
        }
    }

    void WearSlippers(Slippers slip)
    {
        if (slippers)
        {
            // trash old slippers
            Destroy(slippers.left.gameObject);
            Destroy(slippers.right.gameObject);
            Destroy(slippers.gameObject);
        }

        slippers = slip;

        if (slippers.audio)
            slippers.audio.Play();

        // attatch new slippers;

        slippers.left.SetParent(left, true);
        slippers.left.localPosition = Vector3.zero;
        slippers.left.localScale = Vector3.one;
        slippers.left.localEulerAngles = Vector3.zero;
        slippers.right.SetParent(right, true);
        slippers.right.localPosition = Vector3.zero;
        slippers.right.localScale = Vector3.one;
        slippers.right.localEulerAngles = new Vector3(0, 180, 180);

        var scoreUI = FindObjectOfType<PlayerScoreUI>();
        if (scoreUI)
        {
            scoreUI.SetSlipper(Id, slippers.left);
        }
    }
     
    void OnCollisionEnter(Collision col)
    {
        var other = col.gameObject.GetComponent<PlayerController>();
        if (other)
        {
//            Debug.Log("Impact " + playerPostFix);

            if (slippers && slippers.name.Contains("Shark"))
            {
                other.collider.enabled = false;
                var dead = Instantiate(brokenBody, other.transform.position, other.transform.rotation) as Transform;
                foreach (var parts in dead.GetComponentsInChildren<Rigidbody>())
                {
                    parts.AddForce(-col.contacts[0].normal * col.relativeVelocity.magnitude, ForceMode.VelocityChange);
                    parts.renderer.material.color = other.color;
                }

                InAudio.PlayAtPosition(GameManager.Instance.gameObject, deathSound, other.gameObject.transform.position);
                Destroy(other.gameObject);
                GameManager.Instance.Respawn(other);

                Services.Get<CameraShake>().ApplyShake(0.4f, 0.5f);
            }

            var impact = Vector3.Project(col.relativeVelocity, col.contacts[0].normal);
            impact *= other.Push/Push;
            rigidbody.AddForce(impact, ForceMode.VelocityChange);
            Services.Get<CameraShake>().ApplyShake(0.2f, 0.2f);
            //Debug.DrawRay(transform.position, impact, Color.red);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.rigidbody)
            return;

        foreach (var c in col.contacts)
        {
            ups.Add(c.normal);
        }
    }
}
