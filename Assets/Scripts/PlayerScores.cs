using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PlayerScores : MonoBehaviour
{
    public Text Text;

    void Update () {
	    StringBuilder builder = new StringBuilder();
	    for (int i = 0; i < GameManager.Instance.Score.Count; i++)
	    {
            var tuple = GameManager.Instance.Score[i];
	        var player = tuple.Item1;
            builder.Append("Player " + player.playerPostFix + ": " + Mathf.Min(GameManager.Instance.ScoreLimit, tuple.Item2) + "  ");
	    }
	    builder.Append("  out of " + GameManager.Instance.ScoreLimit);
	    Text.text = builder.ToString();
	}
}

