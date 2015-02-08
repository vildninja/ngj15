using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartGameAnim : MonoBehaviour {
    public Text Text;

	// Use this for initialization
	IEnumerator Start () {
        for (int i = 4 - 1; i >= 1; i--)
        {
            yield return new WaitForSeconds(0.4f);
            Text.text = i.ToString(); 
        }
        yield return new WaitForSeconds(0.4f);
	    Text.text = "";
	}
}