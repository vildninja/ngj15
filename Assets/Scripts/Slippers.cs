using UnityEngine;
using System.Collections;

public class Slippers : MonoBehaviour {

    public float forceMultiplier = 1;
    public float jumpMultiplier = 1;
    public float pushMultiplier = 1;
    public float gravityMultiplier = 1;
    public float airMultiplier = 1;
    public float turnMultiplier = 1;

    public float animSpeed = 1;

    public Transform left, right;

    void Update()
    {
        if (transform.childCount > 0)
            transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
}
