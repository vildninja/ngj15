using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour
{

    public PlayerController owner;

    private Vector3 velocity;
    private Transform flagPole;
    private Vector3 startPos;

    public float lastCaptureTime;

    IEnumerator Start()
    {
        flagPole = transform.GetChild(0);
        startPos = transform.position;
        lastCaptureTime = 0;

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

    void LateUpdate()
    {
        if (owner)
        {

            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
        }
    }

    public void Capture(PlayerController player)
    {
        if (lastCaptureTime > Time.time - 1 || owner == player)
            return;

        lastCaptureTime = Time.time;
        owner = player;
        transform.position = owner.transform.position;
        transform.rotation = owner.transform.rotation;
    }
}
