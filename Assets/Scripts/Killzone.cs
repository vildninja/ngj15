using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            GameManager.Instance.Respawn(player);
        }
    }
}
