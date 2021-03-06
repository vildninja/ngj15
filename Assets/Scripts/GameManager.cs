﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameCore;
using GameCore.Util;
using UtilExtensions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //public List<int> ActivePlayers;
    public GameObject PlayerPrefab;

    [SerializeField] private List<Color> PlayerColors;

    public List<PlayerController> Players;

    public int ScoreLimit = 20;

    public InAudioEvent Winning;
    public InAudioNode Point;
    public float startSpeed = 10;

    public List<SpawnPoint> itemSpawns;
        
    public Color PlayerColor(int player)
    {
        player -= 1;
        if (player < PlayerColors.Count && player >= 0)
        {
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

    private bool WinnerFound;
    private Color WinnerColor;

	// Use this for initialization
	void Awake ()
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
            if (Application.loadedLevel == 1)
            {
                readyPlayers = Object.FindObjectsOfType<PlayerReady>();
                if (readyPlayers.Count(r => r.Active) > 0)
                    StartCoroutine(StartGame());

            }
            else
            {
                int count = Players.Count + 1;
                Spawn(count, Object.FindObjectsOfType<SpawnPoint>().Find(s => s.PlayerNoSpawn == count).transform);
            }
        }
        if (Input.GetButtonDown("Back_All") && Application.loadedLevel >= 2)
        {
            StopAllCoroutines();
            Application.LoadLevel(1);
        }
    }

    IEnumerator StartGame()
    {
        Services.Get<MenuMain>().StartGame();
        var logo = Services.Get<LogoMenu>();
        if(logo != null)
            logo.Stop();
        yield return new WaitForSeconds(1.8f);
        //if(readyPlayers == null || readyPlayers.Length == 0)
        //    readyPlayers = Object.FindObjectsOfType<PlayerReady>();
        
        //Debug.Log(readyPlayers.Count(p => p.Active));
        //for (int i = 0; i < readyPlayers.Length; i++)
        //{
        //    if (readyPlayers[i].Active)
        //    {
        //        ActivePlayers.Add(readyPlayers[i].PlayerID);
        //    }
        //}

        InAudio.StopAll(gameObject);
        Application.LoadLevel(2);
        
        
    }

    public void GivePlayerScore(PlayerController controller, int score)
    {

        Services.Get<CameraShake>().ApplyShake(0.2f, 0.2f);

        if (controller == null)
        {
            Debug.LogError("Dont try and give null controllers a score");
        }
        var item = Score.Find(s => s.Item1.Id == controller.Id);
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

        var scoreUI = FindObjectOfType<PlayerScoreUI>();
        if (scoreUI)
        {
            scoreUI.GivePoints(controller, item.Item2);
        }

        if (item.Item2 > ScoreLimit && !WinnerFound)
        {
            WinnerFound = true;
            InAudio.StopAll();
            //WinnerColor = PlayerColor(controller.Id);
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

    private bool gameOver;
    IEnumerator PlayWin()
    {
        gameOver = true;
        Services.Get<MusicPlayer>().GetComponent<AudioSource>().Stop();
        InAudio.PostEvent(gameObject, Winning);
        yield return new WaitForSeconds(4);
        Application.LoadLevel(Application.loadedLevel == 2 ? 3 : 2);
        gameOver = false;
    }
        
    public TupleList<PlayerController, int> Score = new TupleList<PlayerController, int>();

    void OnLevelWasLoaded(int level)
    {
        if (level >= 2)
        {
            GameInput.AllowInput = false;
            StartCoroutine(PlayStartAnim());

            Score.Clear();
            Players.Clear();
            WinnerFound = false;

            StartCoroutine(SpawnPlayers());
        }
    }

    private IEnumerator SpawnPlayers()
    {
        yield return new WaitForSeconds(1.6f);
        if (readyPlayers != null)
        {
            for (int i = 0; i < readyPlayers.Length; i++)
            {
                if (!readyPlayers[i].Active)
                    continue;

                int playerId = readyPlayers[i].PlayerID;
                var spawn = GetSpawnPoint(playerId);
                if (spawn != null)
                {
                    Spawn(playerId, spawn.transform);
                }
                else
                {
                    Debug.LogError("Could not fint spawn for player id " + playerId);
                }
            }


            foreach (var playerController in Players)
            {
                Score.Add(playerController, 0);
            }
        }
    }

    private IEnumerator PlayStartAnim()
    {
        for (int i = 3 - 1; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.6f);
        }
        GameInput.AllowInput = true;
    }

    private SpawnPoint GetSpawnPoint(int playerId)
    {
        if(SpawnPoints == null || SpawnPoints[0] == null)
            SpawnPoints = Object.FindObjectsOfType<SpawnPoint>();
        foreach (var spawnPoint in SpawnPoints)
        {
            if(spawnPoint.PlayerNoSpawn == playerId)
                return spawnPoint;
        }
        return null;
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
        if (gameOver)
            return;

        Services.Get<CameraShake>().ApplyShake(0.6f, 0.6f);
        int number = Int32.Parse(controller.playerPostFix);
        StartCoroutine(DelayedRespawn(number));
    }

    IEnumerator DelayedRespawn(int number)
    {
        yield return new WaitForSeconds(1.5f);
        Spawn(number, GetSpawnPoint(number).transform);
    }
}