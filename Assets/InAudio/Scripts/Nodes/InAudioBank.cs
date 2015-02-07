using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

namespace InAudioSystem
{
    [System.Serializable]
    public class BankTuple
    {
        public InAudioNode Node;
        public AudioClip Clip;
    }
}

public class InAudioBank : MonoBehaviour
{
    public int GUID;
    public List<BankTuple> Clips = new List<BankTuple>();
}

