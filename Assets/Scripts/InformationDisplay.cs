using UnityEngine;
using System.Collections;

public class InformationDisplay : MonoBehaviour {

	private GameOverController GoC;
	public string information;
	/*
	IEnumerator Start () {
		GoC = GameObject.Find("ControllerScript").GetComponent<GameOverController>();
		string part1 = name.Split (' ') [0], part2 = name.Split (' ') [1];
		string url = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=&titles=" + (part1 + " " + part2.ToLower()).Replace ("(", "%28").Replace (")", "%29").Replace (" ", "%20");

		WWW www = new WWW (url);
		yield return www;
		Debug.Log (url);
		Debug.Log (www.text);
		information = www.text;
	}
	*/
	void Start() {
		GoC = GameObject.Find("ControllerScript").GetComponent<GameOverController>();
	}
	public void OnClickButton() {
		GoC.DisplayInfo (name, information);
	}
}
