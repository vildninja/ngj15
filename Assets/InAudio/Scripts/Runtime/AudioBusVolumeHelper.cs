using InAudioSystem;
using InAudioSystem.InMath;
using InAudioSystem.Runtime;
using UnityEngine;

public static class AudioBusVolumeHelper {
    public static void SetTargetVolume(InAudioBus bus, float targetVolume, InEventBusAction.VolumeSetMode setMode, float duration, FadeCurveType curveType)
    {
        //bus.Dirty = true;
        if (duration == 0)
        {
            bus.Fader.Activated = false;
            bus.RuntimeSelfVolume = targetVolume;
        }
        else
        {
            if (setMode == InEventBusAction.VolumeSetMode.Absolute)
            {
                bus.Fader.Activated = true;
                double currentTime = AudioSettings.dspTime;
                bus.Fader.Initialize(curveType, currentTime, currentTime + duration, bus.RuntimeSelfVolume,
                    targetVolume);
            }
            else
            {
                bus.Fader.Activated = true;
                double currentTime = AudioSettings.dspTime;
                float newVolume = Mathf.Clamp(bus.RuntimeSelfVolume + targetVolume, 0.0f, 1.0f);
                bus.Fader.Initialize(curveType, currentTime, currentTime + duration, bus.RuntimeSelfVolume,
                    newVolume);
            }
        }
        UpdateVolumes(bus);
    }

    public static void UpdateVolumes(InAudioBus bus)
    {   
        Fader fader = bus.Fader;
        if (fader.Activated)
        {
            double currentTime = AudioSettings.dspTime;
            bus.RuntimeSelfVolume = (float)fader.Lerp(AudioSettings.dspTime);
            bus.Dirty = true;
            if (/*bus.RuntimeSelfVolume == fader.EndValue ||*/  currentTime >= fader.EndTime)
            {
                fader.Activated = false;
            }
        }

        float parentVolume;
        if (bus.Parent != null)
        {
            var busParent = bus.Parent; 
            parentVolume = busParent.FinalVolume;
        }
        else
        {
            parentVolume = 1.0f;
        }

        if (bus.Parent != null)
            bus.Dirty |= bus.Parent.Dirty;

        float oldVolume = bus.FinalVolume;
        bus.FinalVolume = bus.Volume * bus.RuntimeSelfVolume * parentVolume;

        float duckingVolume = Ducking(bus);
        bus.FinalVolume += duckingVolume;

        if (bus.RuntimeMute)
            bus.FinalVolume = 0;

        if (bus.FinalVolume != oldVolume)
            bus.Dirty = true;

        bool noListener = false;
        Vector3 pos = Vector3.zero;
        if (InAudio.ActiveListener == null)
            noListener = true;
        else 
            pos = InAudio.ActiveListener.transform.position;
        var players = bus.RuntimePlayers;
        for (int i = 0; i < players.Count; ++i)
        {
            var player = players[i];

            if (player != null)
            {
                if(!noListener)
                    player.internalUpdateFalloff(pos);
                if (bus.Dirty)
                {
                    player.UpdateBusVolume(bus.FinalVolume);
                }
            }
            else
            {
                players.RemoveAt(i);
                --i;
            }

        }
        if (bus.Dirty)
        {
            for (int i = 0; i < bus.ExternalSources.Count; i++)
            {
                bus.ExternalSources[i].UpdateBusVolume(bus.FinalVolume);
            }
        }

        for (int i = 0; i < bus.Children.Count; ++i)
        {
            bus.Children[i].Dirty |= bus.Dirty;
            UpdateVolumes(bus.Children[i]);
        }

        bus.Dirty = false;
    }

    private static float Ducking(InAudioBus bus)
    {
        //The maximum amount of volume it is being ducked
        float minDucking = 0;

        int duckedByCount = bus.DuckedBy.Count;
        for (int i = 0; i < duckedByCount; i++)
        {
            DuckingData ducking = bus.DuckedBy[i];
            float ducked = 0;
            if (!ducking.IsBeingDucked) //Release time
            {
                //Release time
                if (ducking.LastDuckedVolume < 0)
                {
                    ducked = ducking.LastDuckedVolume;

                    ducked -= Mathf.Lerp(ducking.VolumeDucking, 0, ducked) * Time.deltaTime / ducking.ReleaseTime;

                    ducked = Mathf.Clamp(ducked, ducking.VolumeDucking, 0);
                    minDucking = Mathf.Min(minDucking, ducked);

                    minDucking = (1.0f - Curves.CumulativeDistribution(Fader.GetT(ducking.VolumeDucking, 0, minDucking))) * ducking.VolumeDucking;
                    minDucking = Mathf.Clamp(minDucking, ducking.VolumeDucking, 0);
                }
                //else 
                //Do nothing as then the amount ducked is zero
            }
            else if (ducking.AttackTime > 0 && ducking.LastDuckedVolume > ducking.VolumeDucking) //Attack time
            {
                ducked = ducking.LastDuckedVolume;
                ducked += ducking.VolumeDucking*Time.deltaTime/ducking.AttackTime;
                ducked = Mathf.Clamp(ducked, ducking.VolumeDucking, 0);
                minDucking = Mathf.Min(minDucking, ducked);
            }
            else 
            {
                minDucking = Mathf.Min(minDucking, ducking.VolumeDucking);
                ducked = ducking.VolumeDucking; 
            }
            
            ducking.LastDuckedVolume = ducked;
            //Util.Debug.Graph("Ducked volume", minDucking);
        }
        bus.LastDuckedVolume = minDucking;
        
        
        return minDucking;
    }

    

    public static void InitVolumes(InAudioBus bus)
    {
        bus.RuntimeSelfVolume = bus.SelfVolume;
        for (int i = 0; i < bus.Children.Count; ++i)
        {
            InitVolumes(bus.Children[i]);
        }        
    }

    public static void MuteAction(InAudioBus audioBus, InEventBusMuteAction.MuteAction muteAction)
    {
        if (muteAction == InEventBusMuteAction.MuteAction.Mute)
            audioBus.RuntimeMute = true;
        else
            audioBus.RuntimeMute = false;
        audioBus.Dirty = true;
    }

    public static void UpdateDucking(InAudioBus inAudioBus)
    {
        UpdateIsActive(inAudioBus);
        UpdateIfDucking(inAudioBus);
    }

    private static int UpdateIsActive(InAudioBus bus)
    {        
        int soundsInChildren = bus.RuntimePlayers.Count + bus.ExternalSources.Count;

        for (int i = 0; i < bus.Children.Count; i++)
        {
            soundsInChildren += UpdateIsActive(bus.Children[i]);
        }

        if (soundsInChildren > 0)
        {
            bus.IsActive = true;
        }
        else
        {
            bus.IsActive = false;
        }

        return soundsInChildren;
    }

    private static void UpdateIfDucking(InAudioBus bus)
    {
        for (int i = 0; i < bus.DuckedBy.Count; i++)
        {
            DuckingData currentBus = bus.DuckedBy[i];
            if (currentBus.DuckedBy.IsActive)
            {
                
                if (!currentBus.IsBeingDucked)
                {
                    currentBus.IsBeingDucked = true;
                }
            }
            else 
            {
                currentBus.IsBeingDucked = false;
            }
        }

        for (int i = 0; i < bus.Children.Count; i++)
        {
            UpdateIfDucking(bus.Children[i]);
        }
    }
}
