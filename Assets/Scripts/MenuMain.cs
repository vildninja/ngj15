using UnityEngine;
using System.Collections;
using GameCore;

public class MenuMain : MonoBehaviour
{
    public InAudioEvent startMusic;
    public InAudioEvent startGame;

    void Awake()
    {
        Services.Override(typeof(MenuMain), this);
    }

	// Use this for initialization
	void Start () {
	    InAudio.PostEvent(gameObject, startMusic);
	}

    public void StartGame()
    {
        InAudio.PostEvent(gameObject, startGame);
    }
}
