using UnityEngine;

public class InEventBusStopAction : AudioEventAction
{
    public InAudioBus Bus;
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
