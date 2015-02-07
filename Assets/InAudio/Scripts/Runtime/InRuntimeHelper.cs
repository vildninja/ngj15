using InAudioSystem.ExtensionMethods;
using InAudioSystem.Runtime;
using UnityEngine;

namespace InAudioSystem 
{
public static class RuntimeHelper
{
    public static InAudioNode SelectRandom(InAudioNode randomNode)
    {
        int childCount = randomNode.Children.Count;
        var weights = ((RandomData)randomNode.NodeData).weights;
        int sum = 0;
        if (childCount != weights.Count)
        {
            InAudio.InDebug.LogError("InAudio: There is a problem with the random weights in the node " + randomNode.Name + ", id=" + randomNode.ID+ ". \nPlease open the audio window for the node and follow instructions");
        }
        for (int i = 0; i < childCount; ++i)
        {
            sum += weights[i];
        }
        int randomArea = Random.Range(0, sum + 1); //+1 because range is non-inclusive

        int currentMax = 0;
        for (int i = 0; i < childCount; ++i)
        {
            currentMax += weights[i];
            if (weights[i] != 0 && randomArea <= currentMax)
            {
                return randomNode.Children[i];
            }
        }
        return null; //Only happens if all the sums are 0*/
    }

    public static AudioRolloffMode ApplyRolloffData(InAudioNode current, InAudioNodeData data, AudioSource workOn)
    {
        workOn.rolloffMode = data.RolloffMode;
        workOn.maxDistance = data.MaxDistance;
        workOn.minDistance = data.MinDistance;

        if (data.RolloffMode == AudioRolloffMode.Custom)
        {
            workOn.maxDistance = float.MaxValue;//Set to max so we can use our own animation curve
        }
        return data.RolloffMode;
    }


    public static AudioRolloffMode ApplyAttentuation(InAudioNode root, InAudioNode current, AudioSource workOn)
    {
        var data = (InAudioNodeData)current.NodeData;
        if (current == root || data.OverrideAttenuation)
        {
            return ApplyRolloffData(current, data, workOn);
        }
        else
        {
            return ApplyAttentuation(root, current.Parent, workOn);
        }
    }

    public static void ReleaseRuntimeInfo(RuntimeInfo info)
    {
        //Swap remove and set the new position
        if (info != null)
        {
            info.PlacedIn.InfoList.FindSwapRemove(info);
            if (InAudioInstanceFinder.RuntimeInfoPool != null)
                InAudioInstanceFinder.RuntimeInfoPool.ReleaseObject(info);
        }
    }

    public static float InitialDelay(InAudioNodeData data)
    {
        if (!data.RandomizeDelay)
            return data.InitialDelayMin;

        return Random.Range(data.InitialDelayMin, data.InitialDelayMax);
    }

    public static float Offset(InAudioNodeData data)
    {
        if (!data.RandomSecondsOffset)
            return data.MinSecondsOffset;

        return Random.Range(data.MinSecondsOffset, data.MaxSecondsOffset);
    }


    public static float ApplyVolume(InAudioNode root, InAudioNode current)
    {
        var data = (InAudioNodeData) current.NodeData;
        if (current == root)
        {
            if (!data.RandomVolume)
                return data.MinVolume;
            else
                return Random.Range(data.MinVolume, data.MaxVolume);
        }

        if (!data.RandomVolume)
            return data.MinVolume * ApplyVolume(root, current.Parent);
        else
            return Random.Range(data.MinVolume, data.MaxVolume) * ApplyVolume(root, current.Parent);

    }

    public static byte GetLoops(InAudioNode node)
    {
        byte loops;
        var data = (InAudioNodeData)node.NodeData;
        if (data.RandomizeLoops)
            loops = (byte)Mathf.Min(Random.Range(data.MinIterations, data.MaxIterations + 1), 255);
        else
            loops = data.MinIterations;
        return loops;
    }

    public static float ApplyPitch(InAudioNode root, InAudioNode current)
    {
        InAudioNodeData inAudioNodeData = (InAudioNodeData)current.NodeData;
        float minPitch = inAudioNodeData.MinPitch;
        float maxPitch = inAudioNodeData.MaxPitch;
        bool isRandom = inAudioNodeData.RandomPitch;
        if (current == root)
        {
            if(!isRandom)
                return minPitch;
            else
            {
                return Random.Range(minPitch, maxPitch);
            }
        }

        if (!isRandom)
            return inAudioNodeData.MinPitch + ApplyPitch(root, current.Parent) - 1;
        else
        {
            return Random.Range(minPitch, maxPitch) + ApplyPitch(root, current.Parent) - 1;
        }

        
    }

    public static float LengthFromPitch(float length, float pitch)
    {
        return length / pitch;
    }
}
}
