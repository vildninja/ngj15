using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
    public InAudioEvent gameMusicEvent;

	// Use this for initialization
	void Start () {
        InAudio.PostEvent(GameManager.Instance.gameObject, gameMusicEvent);
	}

}
