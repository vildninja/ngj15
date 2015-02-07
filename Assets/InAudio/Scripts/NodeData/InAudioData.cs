using System;
using UnityEngine;

public class InAudioData : InAudioNodeData {

#if UNITY_EDITOR
    public AudioClip EditorClip;
    [NonSerialized]
    public AudioClip RuntimeClip;
#endif
#if !UNITY_EDITOR
    [NonSerialized]
    public AudioClip RuntimeClip;
#endif

    public bool IsLoaded
    {
        get { return RuntimeClip != null; }
    }

    public AudioClip Clip
    {
        get
        {
            #if UNITY_EDITOR
                if (Application.isPlaying)
                    return RuntimeClip;
                else
                    return EditorClip;
            #endif
            #if !UNITY_EDITOR
                return RuntimeClip;
            #endif
        }

        set
        {
            #if UNITY_EDITOR
                if (Application.isPlaying)
                    RuntimeClip = value;
                else
                    EditorClip = value;
            #endif
            #if !UNITY_EDITOR
                RuntimeClip = value;
            #endif
        }
    }
}
