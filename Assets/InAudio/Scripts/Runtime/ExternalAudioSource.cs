using InAudioSystem.ExtensionMethods;
using UnityEngine;

public class ExternalAudioSource : MonoBehaviour
{
    public InAudioBus Bus;
    public AudioSource AudioSource;

    [SerializeField] [Range(0.0f, 1.0f)] private float _volume = 1.0f;

    public float volume
    {
        get { return _volume; }
        set
        {
            _volume = Mathf.Clamp(value, 0.0f, 1.0f);
            if (AudioSource != null)
            {
                if (Bus != null)
                    AudioSource.volume = _volume*Bus.FinalVolume;
                else
                    AudioSource.volume = _volume;
            }
        }
    }

    public void UpdateBusVolume(float newVolume)
    {
        if (AudioSource != null)
        {
            AudioSource.volume = _volume * newVolume;
        }       
    }

    public void Start()
    {
        if(Bus != null)
            Bus.ExternalSources.Add(this);
        if (AudioSource != null)
        {
            if (Bus != null)
                AudioSource.volume = _volume*Bus.FinalVolume;
            else
                AudioSource.volume = _volume;
        }
    }

    void OnDestroy()
    {
        if(Bus != null && Bus.ExternalSources != null)
            Bus.ExternalSources.FindSwapRemove(this);
    }
}
