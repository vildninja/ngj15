using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<int> ActivePlayers;

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
        if (Input.GetButtonDown("Start") && Application.loadedLevel == 0)
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
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            SpawnPoint[] spawnPoints = Object.FindObjectsOfType<SpawnPoint>();


            Debug.Log("span manager");
        }
    }

    void Spawn(int player)
    {
        
    }
}