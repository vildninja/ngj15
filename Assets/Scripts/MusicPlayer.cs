using UnityEngine;
using GameCore;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
    public InAudioEvent gameMusicEvent;

	// Use this for initialization
	void Start () {
        Services.Override(GetType(), this);
        //InAudio.PostEvent(gameObject, gameMusicEvent);
	}

    void OnDisable()
    {
        //InAudio.StopAll(gameObject);
        GetComponent<AudioSource>().Stop();
    }

}
