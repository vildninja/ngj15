using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Object.Destroy(other.gameObject);
            GameManager.Instance.Respawn(player);
        }
    }
}
