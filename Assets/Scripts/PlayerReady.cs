using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerReady : MonoBehaviour
{
    public KeyCode Key = KeyCode.Joystick1Button0;
    public bool Active = false;
    public int PlayerID = 1;

	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(Key))
	    {
	        Active = !Active;
            if(Active)
                GetComponent<Image>().color = Color.green;
            else
            {
                GetComponent<Image>().color = Color.white;    
            }
	    }
	}
}
