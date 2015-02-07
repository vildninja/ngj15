using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public static class GObject
{
    public static GameObject Instantiate(GameObject prefab, Action<GameObject> bindings = null)
    {
        var go = Object.Instantiate(prefab) as GameObject;
        if (bindings != null)
            bindings(go);
        go.BroadcastMessage("Setup", SendMessageOptions.DontRequireReceiver);
        return go;
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 pos, Quaternion rot, Action<GameObject> bindings = null)
    {
        var go = Object.Instantiate(prefab, pos, rot) as GameObject;
        if(bindings != null)
            bindings(go);
        go.BroadcastMessage("Setup", SendMessageOptions.DontRequireReceiver);
        return go;
    }
}
