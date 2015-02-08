using UnityEngine;
using System.Collections;
using GameCore;

public class MenuMain : MonoBehaviour
{
    public InAudioEvent startMusic;
    public InAudioEvent startGame;

    public AudioSource backgroundMusic;
    public AudioSource StartSound;

    void Awake()
    {
        Services.Override(typeof(MenuMain), this);
    }

	// Use this for initialization
	void Start () {
        //InAudio.PostEvent(GameManager.Instance.gameObject, startMusic);
	}

    public void StartGame()
    {
        backgroundMusic.Stop();
        StartSound.Play();
        //InAudio.PostEvent(GameManager.Instance.gameObject, startGame);
    }
}
