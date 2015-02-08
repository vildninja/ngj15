using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerScoreUI : MonoBehaviour
{

    public RectTransform[] playerUI;


    public void Register(PlayerController player)
    {
        if (!playerUI[player.Id - 1].gameObject.activeSelf)
        {
            playerUI[player.Id - 1].gameObject.SetActive(true);
            playerUI[player.Id - 1].GetComponentInChildren<Image>().color = player.color;
            playerUI[player.Id - 1].GetComponentInChildren<Text>().text = "0";
        }
    }

    public void GivePoints(PlayerController player, int score)
    {
        if (!playerUI[player.Id - 1].gameObject.activeSelf)
        {
            playerUI[player.Id - 1].gameObject.SetActive(true);
            playerUI[player.Id - 1].GetComponentInChildren<Image>().color = player.color;
        }

        playerUI[player.Id - 1].GetComponentInChildren<Text>().text = score.ToString();

        playerUI[player.Id - 1].animation.Play("ScoreUI");
    }

    public void SetSlipper(int player, Transform slipper)
    {
        var pos = playerUI[player - 1].FindChild("SlipperBase");

        if (pos.childCount > 0)
            Destroy(pos.GetChild(0).gameObject);

        var model = Instantiate(slipper) as Transform;
        model.parent = pos;
        model.localPosition = Vector3.zero;
        model.localEulerAngles = Vector3.zero;
        model.localScale = Vector3.one;
    }

    void Awake()
    {
        foreach (var ui in playerUI)
        {
            ui.gameObject.SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
