using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerReady : MonoBehaviour
{
    public KeyCode Key = KeyCode.Joystick1Button0;
    public bool Active = false;
    public int PlayerID = 1;

    public GameObject EnableIfReady;
    public GameObject EnableIfNotReady;
    public GameObject StartInfo;
    

	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(Key))
	    {
	        Active = !Active;
            
	    }

	    if (Active)
	    {
	        EnableIfReady.SetActive(true);
            EnableIfNotReady.SetActive(false);
            StartInfo.SetActive(true);
	    }
	    else
	    {
            EnableIfReady.SetActive(false);
            EnableIfNotReady.SetActive(true);
	    }
	}
}
