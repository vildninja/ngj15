using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    [System.Serializable]
    public class DuckingData
    {
        public InAudioBus DuckedBy;
        public float VolumeDucking = -0.4f;
        public float AttackTime = 0.0f;
        public float ReleaseTime = 1.0f;
        


        [System.NonSerialized]
        public float LastDuckedVolume;
        [System.NonSerialized]
        public bool IsBeingDucked;

    }
}

