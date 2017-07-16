using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameOverController : MonoBehaviour {

	public Text scoreText, nameText, informationText, scoreTextHolder, GameOver;
	public TextAsset data;
	public GameObject buttonPrefab, informationDisplay, content;
	private Animation anim;

	void Awake() {
		
		anim = informationDisplay.GetComponent<Animation> ();
		var json_data = JSON.Parse (data.text);

		ScoreTracker ST = GameObject.Find ("ScoreTracker").GetComponent<ScoreTracker> ();
		scoreText.text = ST.score.ToString();
		if (ST.newHighScore)
			scoreTextHolder.text = "New High Score:";
		else
			scoreTextHolder.text = "Your score:";
		if (ST.ggwp) {
			GameOver.text = "YOU WIN!";
			PlayerPrefs.SetInt ("ClassicModeComplete", 1);
		}
		else
			GameOver.text = "GAME OVER";
		PlayerPrefs.Save ();

		HashSet <string> allCompounds = ST.allCompounds;

		//HashSet <string> allCompounds = new HashSet<string> ();
		if (allCompounds.Count == 0) {
			allCompounds = new HashSet <string> {
				"Sodium Chloride", "Aluminium Chloride", "Zinc Nitrate", "Calcium Carbonate", "Sodium Phosphate", "Lithium Bromide", "Lithium Chloride", "Lithium Sulfate", "Lithium Phosphate", "Calcium Phosphate", "Aluminium Nitride", "Sodium Phosphate", "Iron(III) oxide"
			};
			Debug.Log ("HERE!!!!");
		}
		foreach (string compoundName in allCompounds) {
			
			GameObject button = Instantiate (buttonPrefab) as GameObject;
			button.name = compoundName;
			button.GetComponent<InformationDisplay> ().information = json_data [compoundName];
			button.transform.GetChild (0).GetComponent<Text> ().text = compoundName;
			button.transform.parent = content.transform;
		}

	}
	public void DisplayInfo(string name, string information) {
		anim.Play ("OpenDisplay");
		nameText.text = name;
		informationText.text = information;
	}
	public void CloseDisplay() {
		anim.Play ("CloseDisplay");
	}
}