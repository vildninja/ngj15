using UnityEngine;
using System.Collections;

public class LogoMenu : MonoBehaviour {
    public InAudioNode MainMenuMusic;
    public float WaitTime = 3;

	// Use this for initialization
	IEnumerator Start () {
        InAudio.Play(gameObject, MainMenuMusic);
        yield return new WaitForSeconds(WaitTime);
        Application.LoadLevel(1);
	}
} 