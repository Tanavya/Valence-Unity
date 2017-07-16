using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class ScoreTracker : MonoBehaviour {

	public int score;
	public HashSet <string> allCompounds;
	public bool newHighScore, ggwp;
	void Start () {

		/*score = 9999999;
		allCompounds = new HashSet <string> {
			"c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "c10", "c11", "c12", "c13", "c14", "c15"
		};*/
	}
	
	void Update () {
	
	}
	void Awake() {
		newHighScore = false;
		ggwp = false;
		//Debug.Log ("Here.");
		//if (SceneManager.GetActiveScene ().buildIndex != 1) {
			
			//string gameMode = "";
			/*
			if (levelNum == 2)
				gameMode = "Classic";
			else
				gameMode = "Learners";
			highScoreText.text = "High Score: " + PlayerPrefs.GetInt (gameMode + "HighScore").ToString ();
			GameObject.Find ("HighScoreText").GetComponent<Text> ().text = 
				"High Score: " + PlayerPrefs.GetInt (SceneManager.GetActiveScene ().name + "HighScore").ToString ();
			*/
		//}
		//else 
		DontDestroyOnLoad (gameObject);
		
	}
}
