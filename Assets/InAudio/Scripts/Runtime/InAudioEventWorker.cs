using System;
using System.Collections.Generic;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using UnityEngine;

[AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "Runtime Event Worker")]
public class InAudioEventWorker : MonoBehaviour
{
    public InPlayer PlayAttachedTo(GameObject controllingObject, InAudioNode audioNode, GameObject attachedTo, float fade = 0f, LeanTweenType fadeType = LeanTweenType.notUsed)
    {
        List<InstanceInfo> currentInstances = audioNode.CurrentInstances;
        if (!AllowedStealing(audioNode, currentInstances))
        {
            return null;
        }

        var runtimePlayer = InAudioInstanceFinder.RuntimePlayerControllerPool.GetObject();
        if (runtimePlayer == null)
        {
            Debug.LogWarning("InAudio: A pooled objected was not initialized. Try to restart play mode. If the problem persists, please submit a bug report.");
        }
        currentInstances.Add(new InstanceInfo(AudioSettings.dspTime, runtimePlayer));
        runtimePlayer.transform.parent = attachedTo.transform;
        runtimePlayer.transform.localPosition = new Vector3();
        Play(controllingObject, audioNode, runtimePlayer, fade, fadeType);
        return runtimePlayer;
    }

    public InPlayer PlayAtPosition(GameObject controllingObject, InAudioNode audioNode, Vector3 position, float fade = 0f, LeanTweenType fadeType = LeanTweenType.notUsed)
    {
        List<InstanceInfo> currentInstances = audioNode.CurrentInstances;
        if (!AllowedStealing(audioNode, currentInstances))
            return null;
        var poolObject = InAudioInstanceFinder.RuntimePlayerControllerPool.GetObject();
        poolObject.transform.position = position; 
        Play(controllingObject, audioNode, poolObject, fade, fadeType);
        return poolObject;
    }

    public void StopAll(GameObject controllingObject, float fadeOutTime, LeanTweenType type)
    {
        ObjectAudioList infoList;
        GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);

        if (infoList != null)
        {
            var list = infoList.InfoList;
            for (int i = 0; i < list.Count; ++i)
            {
                if (fadeOutTime > 0)
                    list[i].Player.Stop(fadeOutTime, type);   
                else
                    list[i].Player.Stop();   
            }       
        } 
    }

    public void SetVolumeForNode(GameObject controllingObject, InAudioNode node, float newVolume)
    {
        ObjectAudioList outInfoList;
        GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out outInfoList);
        if (outInfoList != null)
        {
            List<RuntimeInfo> infoList = outInfoList.InfoList;
            int count = infoList.Count;
            for (int i = 0; i < count; ++i)
            {
                var player = infoList[infoList.Count - 1].Player;
                if (player == node)
                    player.Volume = newVolume;
            }
        }
    }

    public void SetVolumeForGameObject(GameObject controllingObject, float newVolume)
    {
        ObjectAudioList outInfoList;
        GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out outInfoList);
        if (outInfoList != null)
        {
            List<RuntimeInfo> infoList = outInfoList.InfoList;
            int count = infoList.Count;
            for (int i = 0; i < count; ++i)
            {
                var player = infoList[infoList.Count - 1].Player;
                player.Volume = newVolume;
            }
        }
    }

    public InPlayer[] GetPlayers(GameObject go)
    {
        ObjectAudioList infoList;
        GOAudioNodes.TryGetValue(go.GetInstanceID(), out infoList);

        if (infoList != null)
        {
            InPlayer[] players = new InPlayer[infoList.InfoList.Count];
            var list = infoList.InfoList;
            for (int i = 0; i < list.Count; i++)
            {
                players[i] = infoList.InfoList[i].Player;
            }
            return players;
        }
        return null;
    }

    public void GetPlayers(GameObject go, IList<InPlayer> copyTo)
    {
        ObjectAudioList infoList;
        GOAudioNodes.TryGetValue(go.GetInstanceID(), out infoList);

        if (infoList != null)
        {
            var list = infoList.InfoList;
            for (int i = 0; i < list.Count && i < copyTo.Count; i++)
            {
                copyTo[i] = infoList.InfoList[i].Player;
            }
        }
    }

    public void Break(GameObject controllingObject, InAudioNode toBreak)
    {
        ObjectAudioList infoList;
        GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);
        if (infoList != null)
        {
            var list = infoList.InfoList;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].Node == toBreak)
                {
                    list[i].Player.Break();
                }
            }
        }
    }

    public void StopByNode(GameObject controllingObject, InAudioNode nodeToStop, float fadeOutTime = 0.0f, LeanTweenType tweenType = LeanTweenType.easeOutCubic)
    {
        ObjectAudioList infoList;
        GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);

        if (infoList != null)
        {
            var list = infoList.InfoList;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].Node == nodeToStop)
                {
                    list[i].Player.Stop(fadeOutTime, tweenType);
                }
            }
        } 
    }

    private void Play(GameObject controllingObject, InAudioNode audioNode, InPlayer player, float fade, LeanTweenType fadeType)
    {        
        ObjectAudioList tupleList = GetValue(GOAudioNodes, controllingObject);

        RuntimeInfo runtimeInfo = runtimeInfoPool.GetObject();
        tupleList.InfoList.Add(runtimeInfo);
        runtimeInfo.Node = audioNode;
        runtimeInfo.Player = player;
        runtimeInfo.ListIndex = tupleList.InfoList.Count - 1;
        runtimeInfo.PlacedIn = tupleList;

        player._internalPlay(audioNode, controllingObject, runtimeInfo, fade, fadeType);
    }

    private ObjectAudioList GetValue(Dictionary<int, ObjectAudioList> dictionary, GameObject go)
    {
        ObjectAudioList infoList;
        int instanceID = go.GetInstanceID();
        if (!dictionary.TryGetValue(instanceID, out infoList))
        {
            infoList = AudioListPool.GetObject();
            infoList.GO = go;
            dictionary.Add(instanceID, infoList);
        }
        return infoList;
    }

    private InRuntimeInfoPool runtimeInfoPool;
    private ObjectPool<ObjectAudioList> AudioListPool = new ObjectPool<ObjectAudioList>(10);

    //Dictionary<int, List<RuntimeInfo>> 
    private Dictionary<int, ObjectAudioList> GOAudioNodes = new Dictionary<int, ObjectAudioList>();

    private static bool AllowedStealing(InAudioNode audioNode, List<InstanceInfo> currentInstances)
    {
        var data = (InAudioNodeData)audioNode.NodeData;
        if (data.LimitInstances && currentInstances.Count >= data.MaxInstances)
        {
            InPlayer player = null;
            var stealType = data.InstanceStealingTypes;
            if (stealType == InstanceStealingTypes.NoStealing)
                return false;

            int index = 0;
            InstanceInfo foundInfo;
            if (stealType == InstanceStealingTypes.Newest)
            {
                double newestTime = 0;

                for (int i = 0; i < currentInstances.Count; i++)
                {
                    InstanceInfo instanceInfo = currentInstances[i];
                    if (instanceInfo.Timestamp > newestTime)
                    {
                        newestTime = instanceInfo.Timestamp;
                        index = i;
                    }
                }
            }
            else if (stealType == InstanceStealingTypes.Oldest)
            {
                double oldestTime = Double.MaxValue;
                for (int i = 0; i < currentInstances.Count; i++)
                {
                    InstanceInfo instanceInfo = currentInstances[i];
                    if (instanceInfo.Timestamp < oldestTime)
                    {
                        oldestTime = instanceInfo.Timestamp;
                        index = i;
                    }
                }
            }

            foundInfo = currentInstances[index];
            player = foundInfo.Player;
            currentInstances.SwapRemoveAt(ref index);
            if(player != null)
                player.Stop();
        }
        return true;
    }

    private void InitializeController(InPlayer controller)
    {
        controller.internalInitialize(FreeController);
    }

    private void FreeController(ObjectAudioList audioList)
    {
        if (audioList.GO == null)
        {
            AudioListPool.ReleaseObject(audioList);
        }
    }

    public void Cleanup()
    {
        //Yes, an object is allucated to clean up memory. 
        List<int> toRemove = new List<int>();
        foreach (KeyValuePair<int, ObjectAudioList> pair in GOAudioNodes)
        {
            var audioList = pair.Value;
            if (audioList.GO == null)
            {
                toRemove.Add(pair.Key);
                AudioListPool.ReleaseObject(pair.Value);
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
        {
            GOAudioNodes.Remove(toRemove[i]);
        }
    }

    void Awake()
    {
        if (runtimeInfoPool == null)
        {
            runtimeInfoPool = GetComponent<InRuntimeInfoPool>();
            InAudioInstanceFinder.RuntimePlayerControllerPool.Initialization = InitializeController;
        }
    }
}