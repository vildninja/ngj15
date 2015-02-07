using UnityEngine;
using System.Collections;
using GameCore;

public class WinnerHook : MonoBehaviour {
    void Awake()
    {
        Services.Override(typeof(WinnerHook), this);
    }
}
