using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UtilExtensions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<int> ActivePlayers;
    public GameObject PlayerPrefab;

    public List<PlayerController> Players;

	// Use this for initialization
	void Start ()
	{
	    if (Instance == null)
	    {
            Object.DontDestroyOnLoad(gameObject);
	        Instance = this;
	    }
	    else
	        Object.Destroy(gameObject);
	}

    private PlayerReady[] readyPlayers;
    void Update()
    {
        if (Input.GetButtonDown("Start"))
        {
            if (Application.loadedLevel == 0)
            {
                readyPlayers = Object.FindObjectsOfType<PlayerReady>();
                if (readyPlayers.Count(r => r.Active) > 0)
                {
                    for (int i = 0; i < readyPlayers.Length; i++)
                    {
                        if (readyPlayers[i].Active)
                        {
                            ActivePlayers.Add(readyPlayers[i].PlayerID);
                        }
                    }
                    Application.LoadLevel("scene");
                }
            }
            else
            {
                int count = Players.Count + 1;
                Spawn(count, Object.FindObjectsOfType<SpawnPoint>().Find(s => s.PlayerNoSpawn == count).transform);
            }
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            SpawnPoint[] spawnPoints = Object.FindObjectsOfType<SpawnPoint>();
            if (readyPlayers != null)
            {
                for (int i = 0; i < readyPlayers.Length; i++)
                {
                    if(!readyPlayers[i].Active)
                        continue;
                    int count = i;
                    var spawn = spawnPoints.Find(s => s.PlayerNoSpawn == readyPlayers[count].PlayerID);
                    Spawn(readyPlayers[i].PlayerID, spawn.transform);
                }
            }

            Debug.Log("span manager");
        }
    }

    void Spawn(int player, Transform spawn)
    {
        var go = Instantiate(PlayerPrefab, spawn.position, Quaternion.identity) as GameObject;
        var controller = go.GetComponent<PlayerController>();
        Players.Add(controller);
        controller.playerPostFix = player.ToString();
    }
}