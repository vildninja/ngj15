using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinnerColorChange : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
	{
        Color color = GameManager.Instance.PlayerColor(GameManager.Instance.WinningPlayerId());
	    GetComponent<Image>().color = color;
	}
}
