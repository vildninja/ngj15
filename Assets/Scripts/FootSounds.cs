using UnityEngine;
using System.Collections;

public class FootSounds : MonoBehaviour
{
    public InAudioNode FootSound;

    public void Foot()
    {
        InAudio.Play(gameObject, FootSound);
    }
}
