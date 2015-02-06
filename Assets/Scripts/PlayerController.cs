using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public string playerPrefix;
    public float force;
    public float jumpForce;

    public bool isGrounded;

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (isGrounded && Input.GetButtonDown(playerPrefix + "Jump"))
        {
            rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 direction = new Vector3(Input.GetAxis(playerPrefix + "Horizontal"), 0, Input.GetAxis(playerPrefix + "Vertical"));

        if (direction.sqrMagnitude > 0.2f)
        {
            transform.forward = direction.normalized;
            //transform.up = Vector3.Cross(rigidbody.velocity, transform.right).normalized;
            //transform.LookAt(transform.position + rigidbody.velocity);
        }

        rigidbody.AddForce(direction * force);
	}
}
