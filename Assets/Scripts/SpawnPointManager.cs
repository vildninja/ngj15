using UnityEngine;
using System.Collections;

public class SpawnPointManager : MonoBehaviour {
    public static SpawnPointManager Instance { get; private set; }

    void Awake()
    {
        SpawnPoint[] spawnPoints = Object.FindObjectsOfType<SpawnPoint>();
        Debug.Log("span manager");
    }
}