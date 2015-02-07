using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using GameCore.Util;
using UtilExtensions;
using Object = UnityEngine.Object;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<int> ActivePlayers;
    public GameObject PlayerPrefab;

    [SerializeField] private List<Color> PlayerColors;

    public List<PlayerController> Players;

    public int ScoreLimit = 20;

    public InAudioNode Point;
    public float startSpeed = 10;
        
    public Color PlayerColor(int player)
    {
        player -= 1;
        Debug.Log(player + "  "+PlayerColors.Count);
        if (player < PlayerColors.Count && player >= 0)
        {
            Debug.Log(PlayerColors[player]);
            return PlayerColors[player];
        }
        return Color.white;
    }

    public int WinningPlayerId()
    {
        var p = Score.FindMax(s => s.Item2);
        if(p != null)
            return p.Item1.Id;
        else
        {
            return -1;
        }
    }

    public PlayerController Winner = null;

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
    private SpawnPoint[] SpawnPoints;
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

    public void GivePlayerScore(PlayerController controller, int score)
    {
        if (controller == null)
        {
            Debug.LogError("Dont try and give null controllers a score");
        }
        var item = Score.Find(s => s.Item1 == controller);
        if (item == null)
        {
            item = Score.Add(controller, score);
            InAudio.Play(gameObject, Point);
        }
        else
        {
            if (item.Item2 < ScoreLimit)
                InAudio.Play(gameObject, Point);
            item.Item2 += score;
        }



        if (item.Item2 > ScoreLimit && Winner == null)
        {
            Winner = controller;
            var hook = Services.Get<WinnerHook>().gameObject;
            hook.SetActive(true);
            for (int i = 0; i < hook.transform.childCount; i++)
            {
                hook.transform.GetChild(i).gameObject.SetActive(true);
            }
            hook.GetComponentInParent<Animator>().enabled = true;
            StartCoroutine(PlayWin());
        }
    }

    IEnumerator PlayWin()
    {
        yield return new WaitForSeconds(4);
        Application.LoadLevel(0);
    }
        
    public TupleList<PlayerController, int> Score = new TupleList<PlayerController, int>();

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            Score.Clear();
            Players.Clear();
            Winner = null;

            if (readyPlayers != null)
            {
                for (int i = 0; i < readyPlayers.Length; i++)
                {
                    if(!readyPlayers[i].Active)
                        continue;
                    var spawn = GetSpawnPoint(readyPlayers[i].PlayerID);
                    Spawn(readyPlayers[i].PlayerID, spawn.transform);
                }


                foreach (var playerController in Players)
                {
                    Score.Add(playerController, 0);
                }
            }
        }
    }

    private SpawnPoint GetSpawnPoint(int playerId)
    {
        if(SpawnPoints == null)
            SpawnPoints = Object.FindObjectsOfType<SpawnPoint>();
        return SpawnPoints.Find(s => s.PlayerNoSpawn == playerId);
    }

    void Spawn(int player, Transform spawn)
    {
        var go = Instantiate(PlayerPrefab, spawn.position, Quaternion.identity) as GameObject;
        var controller = go.GetComponent<PlayerController>();
        Players.Add(controller);
        controller.playerPostFix = player.ToString();
        controller.color = PlayerColor(player);
        go.rigidbody.velocity = spawn.forward*startSpeed;
    }

    public void Respawn(PlayerController controller)
    {
        int number = Int32.Parse(controller.playerPostFix);
        Spawn(number, GetSpawnPoint(number).transform);
        Services.Get<CameraShake>().ApplyShake(0.6f, 0.6f);

    }
}