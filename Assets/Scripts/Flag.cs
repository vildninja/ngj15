using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour
{

    public PlayerController owner;

    private Vector3 velocity;
    private Transform flagPole;
    private Vector3 startPos;

    IEnumerator Start()
    {
        flagPole = transform.GetChild(0);
        startPos = transform.position;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (owner)
            {
                // TODO add score to owner
            }
            else
            {
                transform.position = startPos;
            }
        }
    }

    //void FixedUpdate()
    //{
    //    Vector3 destination = startPos;
    //    if (owner)
    //        destination = owner.transform.position;

    //    var lastPos = transform.position;
    //    var newPos = Vector3.Lerp(lastPos, destination, 0.5f);

    //    velocity = Vector3.Lerp(velocity, (newPos - lastPos) * Time.fixedDeltaTime, 0.1f);
    //    transform.position = newPos;

    //    if (owner)
    //        transform.forward = owner.transform.forward;
    //}

    public void Capture(PlayerController player)
    {
        owner = player;
        transform.parent = owner.transform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}
