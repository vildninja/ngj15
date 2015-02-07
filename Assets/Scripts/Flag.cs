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
        owner = player;
        transform.position = owner.transform.position;
        transform.rotation = owner.transform.rotation;
    }
}
