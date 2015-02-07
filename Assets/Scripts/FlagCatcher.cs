using UnityEngine;
using System.Collections;

public class FlagCatcher : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 angle;
    public Flag captured;

    public int score;

	// Use this for initialization
	IEnumerator Start ()
	{
	    score = 0;
	    while (true)
	    {
	        yield return new WaitForSeconds(1);
	        if (captured)
	            score++;
	    }
	}

    public void ClaimFlag(Flag flag)
    {
        captured = flag;
        flag.transform.parent = transform;
        flag.transform.localPosition = offset;
        flag.transform.localEulerAngles = angle;
        flag.collider.enabled = false;
    }

    void OnTriggerEnter(Collider col)
    {
        var flag = col.GetComponent<Flag>();
        if (flag)
        {
            ClaimFlag(flag);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        var enemy = col.gameObject.GetComponent<FlagCatcher>();
        if (enemy && enemy.captured)
        {
            ClaimFlag(enemy.captured);
            enemy.captured = null;
        }
    }
}
