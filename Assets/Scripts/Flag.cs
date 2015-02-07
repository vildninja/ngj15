using UnityEngine;
using System.Collections;
using GameCore;

public class Flag : MonoBehaviour
{
    public PlayerController owner;

    private Vector3 velocity;
    private Vector3 startPos;

    public float lastCaptureTime;

    public float WaitTime = 2.5f;
    public int Score = 3;
    public int TotalScoreMax = 6;
    private int ExtraScore;

    void Start()
    {
        startPos = transform.position;
        lastCaptureTime = 0;
    }

    IEnumerator Capturing()
    {
        while (true)
        {
            yield return new WaitForSeconds(WaitTime);
            if (owner != null)
            {
                GameManager.Instance.GivePlayerScore(owner,  Mathf.Min(Score + ExtraScore, TotalScoreMax));
            }
            else
            {
                transform.position = startPos;
            }
            ExtraScore += 1;
        }
    }

    void LateUpdate()
    {
        if (owner)
        {

            transform.position = owner.transform.position;
            transform.rotation = owner.transform.rotation;
        }
        else
        {
            transform.Rotate(0, Time.deltaTime * 90, 0);
        }
    }

    public void Capture(PlayerController player)
    {
        if (lastCaptureTime > Time.time - 1 || owner == player)
            return;
        StopAllCoroutines();


        lastCaptureTime = Time.time;
        owner = player;
        transform.position = owner.transform.position;
        transform.rotation = owner.transform.rotation;
        GameManager.Instance.GivePlayerScore(owner, 1);
        Services.Get<CameraShake>().ApplyShake(0.2f, 0.2f);
        StartCoroutine(Capturing());
    }
}
