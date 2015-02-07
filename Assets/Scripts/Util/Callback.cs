using System;
using GameCore;
using UnityEngine;
using System.Collections;

public class Callback : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	    Services.RegisterIfNew(typeof(Callback), this);
	}

    public void ExecuteIn(Action action, float delay)
    {
        //StartCoroutine(delayed(action, delay));
    }

    public void ExecuteNextFrame(Action action)
    {
        StartCoroutine(nextFrame(action));
    }

    private IEnumerator delayed(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log(action);
        //action();
    }

    private IEnumerator nextFrame(Action action)
    {
        yield return null;
        action();
    }
}
