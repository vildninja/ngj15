using System.Collections.Generic;
using InAudioSystem;

public class InTrackData : InAudioNodeData
{
    public float TrackLength = 10f;
    public List<InLayerData> Layers = new List<InLayerData>();

#if UNITY_EDITOR
    public float Zoom = 1f;
#endif
}
 