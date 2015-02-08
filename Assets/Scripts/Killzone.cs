using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour
{

    public InAudioNode deathSound;
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            InAudio.PlayAtPosition(gameObject, deathSound, other.transform.position);
            Object.Destroy(other.gameObject);
            GameManager.Instance.Respawn(player);
        }
    }
}
