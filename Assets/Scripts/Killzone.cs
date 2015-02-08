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
            InAudio.Play(gameObject, deathSound);
            Object.Destroy(other.gameObject);
            GameManager.Instance.Respawn(player);
        }
    }
}
