using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

	// Use this for initialization
	void Start ()
	{
	    if (Instance == null)
	        Instance = this;
        else
            Object.Destroy(gameObject);
	}


    public void StartGame()
    {
        
    }
}
