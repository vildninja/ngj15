using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public string playerPrefix;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    Vector2 direction = new Vector2(Input.GetAxis(""), 0);
	}
}
