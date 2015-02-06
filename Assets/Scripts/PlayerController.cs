using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;

public class PlayerController : MonoBehaviour
{

    public string playerPrefix;
    public float force;
    public float jumpForce;

    public bool isGrounded;

    private List<Vector3> ups;
    private Vector3 surfaceUp;
    private Vector3 lastForward;

	// Use this for initialization
	void Start ()
    {
	    ups = new List<Vector3>();
	    lastForward = transform.forward;
    }

    void Update()
    {
        if (isGrounded && Input.GetButtonDown(playerPrefix + "Jump"))
        {
            rigidbody.AddForce(surfaceUp * jumpForce, ForceMode.Impulse);
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
	    if (ups.Count == 0)
	    {
	        isGrounded = false;
	        surfaceUp = Vector3.Lerp(surfaceUp, Vector3.up, .05f);
	    }
	    else
        {
            isGrounded = true;
            Vector3 sum = new Vector3();
	        foreach (var up in ups)
	        {
	            sum += up;
	        }
            surfaceUp = Vector3.Lerp(surfaceUp, sum.normalized, .3f);
            ups.Clear();
        }

        Vector3 direction = new Vector3(Input.GetAxis(playerPrefix + "Horizontal"), 0, Input.GetAxis(playerPrefix + "Vertical"));

	    Vector3 forward = direction.normalized;
	    if (direction.sqrMagnitude < 0.2f)
	        forward = rigidbody.velocity.normalized;

	    forward = Vector3.ProjectOnPlane(Vector3.Lerp(lastForward, forward, .4f), surfaceUp).normalized;
	    lastForward = forward;

        transform.LookAt(transform.position + forward, surfaceUp);


        rigidbody.AddForce(direction * force);
	}

    void OnCollisionStay(Collision col)
    {
        foreach (var c in col.contacts)
        {
            ups.Add(c.normal);
        }
    }
}
