using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StartOptions : MonoBehaviour {

	// Use this for initialization
	public Text ClassicHighScore, ChallengeHighScore;
	public Button ChallengePlayButton;
	public GameObject how_to_play;
	void Start () {
		if (PlayerPrefs.GetInt ("HelpRead", 0) == 0) {
			how_to_play.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
		}
		ClassicHighScore.text = "Best score: " + PlayerPrefs.GetInt ("ClassicModeHighScore", 0).ToString ();
		if (PlayerPrefs.GetInt ("ClassicModeComplete", 0) == 0) {
			ChallengeHighScore.text = "Win classic mode to unlock";
			ChallengePlayButton.interactable = false;
		} else {
			ChallengePlayButton.interactable = true;
			ChallengeHighScore.rectTransform.sizeDelta = new Vector2 (760, 300);
			ChallengeHighScore.text = "Best score: " + PlayerPrefs.GetInt ("ChallengeModeHighScore", 0).ToString ();
		}
	}
}
