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

    public Transform left, right;
	public int Id { get; private set; }
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponentInChildren<Animator>();
	    ups = new List<Vector3>();
	    lastForward = transform.forward;
	    Id = Int32.Parse(playerPostFix);

	    renderer.material.color = color;
	}

    void Update()
    {
        if (isGrounded && Input.GetButtonDown("A_"+playerPostFix))
        {
            isGrounded = false;
            rigidbody.AddForce(surfaceUp * Jump, ForceMode.Impulse);
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
        Vector3 direction = new Vector3(Input.GetAxis("L_XAxis_" + playerPostFix), 0, -Input.GetAxis("L_YAxis_" + playerPostFix));

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

	    if (direction.sqrMagnitude > 0.2f)
	    {
            anim.Play("Run");
            anim.speed = Mathf.Clamp01(direction.magnitude) * AnimSpeed;
	    }
	    else
	    {
            anim.Play("Idle");
            anim.speed = 1;
	    }

        rigidbody.AddForce(direction * Force * (isGrounded ? 1 : Air));

        // gravity
        rigidbody.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);

        Debug.DrawRay(transform.position, rigidbody.velocity, Color.green);
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
            if (slippers)
            {
                // trash old slippers
                Destroy(slippers.left.gameObject);
                Destroy(slippers.right.gameObject);
                Destroy(slippers.gameObject);
            }

            slippers = slip;

            // attatch new slippers;

            slippers.left.SetParent(left, true);
            slippers.left.localPosition = Vector3.zero;
            slippers.left.localEulerAngles = Vector3.zero;
            slippers.right.SetParent(right, true);
            slippers.right.localPosition = Vector3.zero;
            slippers.right.localEulerAngles = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        var other = col.gameObject.GetComponent<PlayerController>();
        if (other)
        {
            Debug.Log("Impact " + playerPostFix);

            var impact = Vector3.Project(col.relativeVelocity, col.contacts[0].normal);
            impact *= other.Push/Push;
            rigidbody.AddForce(impact, ForceMode.VelocityChange);

            Debug.DrawRay(transform.position, impact, Color.red);
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
