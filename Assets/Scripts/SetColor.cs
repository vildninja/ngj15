using UnityEngine;
using System.Collections;

public class SetColor : MonoBehaviour {
    public int PlayerID = 1;

	// Use this for initialization
	void Start () {
        Color color = GameManager.Instance.PlayerColor(PlayerID);
        GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color",color);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
