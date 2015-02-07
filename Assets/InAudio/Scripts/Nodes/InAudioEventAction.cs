using UnityEngine;

namespace InAudioSystem
{
    public enum EventActionTypes
    {
        [EnumIndex(1, "Play")]  Play = 1,
        [EnumIndex(2, "Stop")]  Stop = 2,
        [EnumIndex(3, "Break")]  Break = 3,
        //[EnumIndex(4)] //Pause = 4, //Not implemented
        [EnumIndex(5, "Stop All")]  StopAll = 5,
        //[EnumIndex(6)] //SetVolume     = 6, //Not implemented
        [EnumIndex(7, "Stop All In Bus")]  StopAllInBus = 10,
        [EnumIndex(8, "Set Bus Volume")]  SetBusVolume = 7,
        [EnumIndex(9, "Set Bus Mute")]  SetBusMute = 11,
        [EnumIndex(10, "Bank Loading")] BankLoading = 8,
        //[EnumIndex(11, "Unload Bank")] UnloadBank = 9,
        
        
    };
}

public abstract class AudioEventAction : MonoBehaviour
{
    public float Delay;
    public InAudioSystem.EventActionTypes EventActionType;

    public abstract Object Target { get; set; }

    public abstract string ObjectName { get; }
}
