using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

[AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "Runtime Audio Data")]
public class InRuntimeAudioData : MonoBehaviour {
    public Dictionary<int, InAudioEventNode> Events;

    public InAudioEventNode GetEvent(int id)
    {
        InAudioEventNode audioEvent;
        Events.TryGetValue(id, out audioEvent);
        return audioEvent;
    }

    public void UpdateEvents(InAudioEventNode root)
    {
        Events = new Dictionary<int, InAudioEventNode>();
        BuildEventSet(root, Events);
    }
 
    void BuildEventSet(InAudioEventNode audioevent, Dictionary<int, InAudioEventNode> events)
    {
        if (audioevent.Type != EventNodeType.Folder && audioevent.Type != EventNodeType.Root)
        {
            events[audioevent.GUID] = audioevent;
        }
        for (int i = 0; i < audioevent.Children.Count; ++i)
        {
            BuildEventSet(audioevent.Children[i], events);
        }
    }
}
