using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace InAudioSystem
{
    [System.Serializable]
    public class InLayerData
    {
        public InAudioNode Node;
        public double Position;


#if UNITY_EDITOR
        public Vector2 ScrollPos;
#endif
    }

}

