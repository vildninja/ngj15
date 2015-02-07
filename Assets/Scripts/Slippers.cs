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

    public Transform slipperModel;

    void Start()
    {
        var t = Instantiate(slipperModel, Vector3.zero, Quaternion.identity) as Transform;
        t.localScale = Vector3.one;
        t.SetParent(left, false);
        t = Instantiate(slipperModel, Vector3.zero, Quaternion.identity) as Transform;
        t.localScale = Vector3.one;
        t.SetParent(right, false);
    }

    void Update()
    {
        if (transform.childCount > 0)
            transform.Rotate(0, 90 * Time.deltaTime, 0);
    }
}
