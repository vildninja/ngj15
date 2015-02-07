using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinnerColorChange : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
	{
        Debug.Log(GameManager.Instance.WinningPlayerId());
        Color color = GameManager.Instance.PlayerColor(GameManager.Instance.WinningPlayerId());
        //Debug.Log(color);
	    GetComponent<Image>().color = color;
	}
}
