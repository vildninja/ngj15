using InAudioSystem.Runtime;
using UnityEngine;

public class InEventBusAction : AudioEventAction
{
    public InAudioBus Bus;
    public float Volume = 1.0f;
    public VolumeSetMode VolumeMode = VolumeSetMode.Absolute;

    public FadeCurveType FadeCurve = FadeCurveType.SmoothLerp;
    public float Duration;

    public override Object Target
    {
        get
        {
            return Bus;
        }
        set
        {
            if (value is InAudioBus)
                Bus = value as InAudioBus;
        }
    }


    public enum VolumeSetMode
    {
        Absolute, Relative
    }

    public override string ObjectName
    {
        get
        {
            if (Bus != null)
                return Bus.GetName;
            else
            {
                return "Missing Bus";
            }
        }
    }
}
