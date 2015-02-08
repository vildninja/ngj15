using UnityEngine;
using System.Collections;
using GameCore;

public class LogoMenu : MonoBehaviour {
    public InAudioNode MainMenuMusic;
    public float WaitTime = 3;

	// Use this for initialization
	IEnumerator Start () {
        InAudio.Play(gameObject, MainMenuMusic);
        yield return new WaitForSeconds(WaitTime);
        Application.LoadLevel(1);

        Services.Register(this.GetType(), this);
	}

    public void Stop()
    {
        InAudio.Stop(gameObject, MainMenuMusic);
    }
} 