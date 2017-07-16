using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//DONE Fix Reflection
//DONE Loading between scenes
//TODO Work with akhtar to improve look
//TODO Destroy Animation
//TODO (low priority) add more elements
//DONE Fix bubbles going out of box
//DONE Fix Gravity issue
//DONE Fix scoring
//DONE Fix BFS, some compounds not forming
//DONE Save Highscore
//DONE Aim line
//DONE Add shooter boundary
//DONE allow bubbles to squeeze through
//DONE Last screen show data of compounds formed
//DONE Add learner's mode
//DONE Mechanism to have total valency around zero //TODO Ensure if game hasn't become TOO easy lmao
//TODO Identify and remove inefficiencies, such as Debug statements
//DONE Some bubbles falling faltu mein
//TODO Bug fix : Says YOU WIN after you win once and restart

public class InstantiateSpheres : MonoBehaviour {
	

	//This is the main script. It is responsible for instantiating the game objects, and co-ordinating their actions.

	public GameObject sphere, highlighter;
	public GameObject red, green, yellow, blue, cyan, magenta, pink, orange;
	public GameObject al, nh4, br, ca, c, c03, f, h, oh, zn, li, mg, n, o, p04, k, na, s04, cl;
	public List <string> elements, chosenAnions, chosenCations;
	public Text timeCountText, BonusText;
	public Dictionary <string, string> cations;
	public Dictionary <string, string> anions;
	public Dictionary <string, int> ions;
	public List <string> anionsNames, cationsNames;
	public int totalValency;
	public Button TimeIncrease, Remove, Bomb, Increment;
	public int num_rows = 7, num_columns = 10;

	private int bubblesCount;
	private HashSet <string> allCompounds;
	private float diameter = 6.0f;
	private Dictionary <string, GameObject> sphereColor;
	private Dictionary <string, GameObject> symbolToGO;
	private GameObject shooter1, shooter2, shooter3;
	private float timeLeft, timeCount;
	static int totalScore, inc_moves_left = -1, remove_moves_left = -1, explosive_moves_left = -1; 

	GameObject InstantiateElementBubble(Vector3 pos, bool is_shooter) {
		string ion_name = "";
		if (totalValency > 0) {
			ion_name = chosenAnions [Random.Range (0, chosenAnions.Count)];
		} else {
			ion_name = chosenCations [Random.Range (0, chosenCations.Count)];
		}
		GameObject bubble = Instantiate (sphere, pos, Quaternion.identity) as GameObject;
		if (is_shooter) {
			bubble.tag = "Shooter";
			bubble.transform.localScale = new Vector3 (2.6f, 2.6f, 2.6f);
		}
		GameObject cover = Instantiate (sphereColor [ion_name], pos + new Vector3 (0, 0, -10), Quaternion.identity) as GameObject;
		GameObject text = Instantiate (symbolToGO [ion_name], pos + new Vector3 (0, 0, -12), Quaternion.identity) as GameObject;
		bubble.gameObject.transform.eulerAngles = new Vector3 (0, 90, 0);
		cover.transform.parent = bubble.transform;
		text.transform.parent = bubble.transform;
		bubble.AddComponent<ElementData> ();
		bubble.GetComponent<ElementData> ().SetValues (ion_name, sphereColor [ion_name]);
		bubblesCount++;
		totalValency += ions [ion_name];
		return bubble;
	}
	void printList (List <string> list) {
		string output = "";
		foreach (string x in list)
			output += x + " ";
		//Debug.Log (output);
	}
	void Activate(string pref, bool interactable) {
		if (pref == "Bomb")
			Bomb.interactable = interactable;
		else if (pref == "Increment")
			Increment.interactable = interactable;
		else if (pref == "TimeIncrease")
			TimeIncrease.interactable = interactable;
		else if (pref == "Remove")
			Remove.interactable = interactable;
	}
	void CheckActivation(string pref) {
		System.DateTime currentTime = System.DateTime.Now;
		System.DateTime oldTime = System.DateTime.Now;
		if (PlayerPrefs.HasKey (pref)) {
			long temp = System.Convert.ToInt64(PlayerPrefs.GetString(pref));
			oldTime = System.DateTime.FromBinary(temp);
			System.TimeSpan difference = currentTime.Subtract(oldTime);
			//Debug.Log (difference);
			if (difference.Hours >= 1) {
				Activate (pref, true);
			} else {
				Activate (pref, false);
			}
		}
		else {
			Activate (pref, true);
		}
	}

	void Start() {
		/*
		string gameMode = "";
		if (SceneManager.GetActiveScene ().buildIndex == 2)
			gameMode = "Classic";
		else
			gameMode = "Learners";
		//Debug.Log ("Current High Score is " + PlayerPrefs.GetInt (gameMode + "HighScore").ToString ());
		*/

		CheckActivation ("Bomb");
		CheckActivation ("Increment");
		CheckActivation ("TimeIncrease");
		CheckActivation ("Remove");

		GameObject.Find ("ScoreTracker").GetComponent<ScoreTracker> ().newHighScore = false;
		totalValency = 0;
		bubblesCount = 0;
		timeCount = 0.0f;
		timeLeft = 180.0f;
		totalScore = 0;
		Physics.gravity = new Vector3(0, -49.0f, 0); //For isolated clusters of bubbles to fall down
		cations = new Dictionary <string, string> ();
		anions = new Dictionary <string, string> ();
		ions = new Dictionary <string, int> ();
		symbolToGO = new Dictionary <string, GameObject> ();
		sphereColor = new Dictionary <string, GameObject> ();
		allCompounds = new HashSet<string> ();

		cations ["Hydrogen"] = "H";
		cations ["Potassium"] = "K";
		cations ["Sodium"] = "Na";
		cations ["Ammonium"] = "(NH4)";
		cations ["Lithium"] = "Li";
		cations ["Calcium"] = "Ca";
		cations ["Aluminium"] = "Al";
		cations ["Zinc"] = "Zn";
		cations ["Magnesium"] = "Mg";

		anions ["Bromide"] = "Br";
		anions ["Sulfate"] = "(SO4)";
		anions ["Phosphate"] = "(PO4)";
		anions ["Chloride"] = "Cl";
		anions ["Fluoride"] = "F";
		anions ["Oxide"] = "O";
		anions ["Nitride"] = "N";
		anions ["Carbonate"] = "(CO3)";
		anions ["Hydroxide"] = "(OH)";
		anions ["Hydrogen"] = "H";
		anions ["Iodide"] = "I";

		ions ["Hydrogen"] = 1;
		ions ["Potassium"] = 1;
		ions ["Sodium"] = 1;
		ions ["Ammonium"] = 1;
		ions ["Lithium"] = 1;
		ions ["Calcium"] = 2;
		ions ["Aluminium"] = 3;
		ions ["Zinc"] = 2;
		ions ["Magnesium"] = 2;


		ions ["Bromide"] = -1;
		ions ["Sulfate"] = -2;
		ions ["Phosphate"] = -3;
		ions ["Hydroxide"] = -1;
		ions ["Chloride"] = -1;
		ions ["Oxide"] = -2;
		ions ["Carbonate"] = -2;
		ions ["Fluoride"] = -1;
		ions ["Nitride"] = -3;
		ions ["Iodide"] = -1;

		symbolToGO ["Sodium"] = na;
		symbolToGO ["Ammonium"] = nh4;
		symbolToGO ["Hydrogen"] = h;
		symbolToGO ["Calcium"] = ca;
		symbolToGO ["Lithium"] = li;
		symbolToGO ["Magnesium"] = mg;
		symbolToGO ["Aluminium"] = al;
		symbolToGO ["Zinc"] = zn;

		symbolToGO ["Chloride"] = cl;
		symbolToGO ["Sulfate"] = s04;
		symbolToGO ["Phosphate"] = p04;
		symbolToGO ["Hydroxide"] = oh;
		symbolToGO ["Oxide"] = o;
		symbolToGO ["Carbonate"] = c03;
		symbolToGO ["Fluoride"] = f;
		symbolToGO ["Nitride"] = n;
		symbolToGO ["Bromide"] = br;

		cationsNames = new List <string> {
			"Sodium",
			"Ammonium",
			"Hydrogen",
			"Calcium",
			"Lithium",
			"Magnesium",
			"Aluminium",
			"Zinc"
		};

		anionsNames = new List <string> {
			"Sulfate",
			"Phosphate",
			"Hydroxide",
			"Oxide",
			"Carbonate",
			"Fluoride",
			"Nitride",
			"Bromide",
			"Chloride"
		};

		//randomly choses 4 cations for the rest of the game
		var set1 = new HashSet<string>(); 
		while (set1.Count < 4) {
			set1.Add(cationsNames[Random.Range (0, cationsNames.Count)]);
		}
		foreach (string go in set1) {
			chosenCations.Add (go);
			elements.Add (go);
		}
		//randomly choses 4 anions for the rest of the game
		var set2 = new HashSet <string> ();
		while (set2.Count < 4) {
			set2.Add(anionsNames[Random.Range(0, anionsNames.Count)]);
		}
		foreach (string go in set2) {
			chosenAnions.Add (go);
			elements.Add (go);
		}
		printList (chosenCations);
		printList (chosenAnions);
		printList (elements);
		//randomly allocates colors to each ion for the rest of the game
		var set3 = new HashSet <int> ();
		while (set3.Count < 8) {
			set3.Add (Random.Range (0, 8));
		}
		List <GameObject> colors = new List <GameObject> { red, green, yellow, blue, cyan, magenta, pink, orange };

		int j = 0;
		foreach (int i in set3) {
			sphereColor [elements [j]] = colors [i];
			j++;
		}

		for (int y = 0; y < num_rows; y++) {
			for (int x = 0; x < num_columns; x++) {

				if (y%2 == 1 && x==0) continue;
				if (y % 2 == 0) {
					InstantiateElementBubble (new Vector3 (-27.0f + diameter * x, 33.0f - Mathf.Sqrt (diameter/2) * diameter/2 * y, -2.0f), false);
				} else {
					InstantiateElementBubble (new Vector3 (-30.0f + diameter * x, 33.0f - Mathf.Sqrt (diameter/2) * diameter/2 * y, -2.0f), false);
				}
			}
		}
		shooter1 = InstantiateElementBubble (new Vector3 (0.0f, -38.5f, -2), true);
		shooter2 = InstantiateElementBubble (new Vector3 (8.0f, -42.0f, -2), true);
		shooter3 = InstantiateElementBubble (new Vector3 (15.0f, -42.0f, -2), true);

		shooter1.AddComponent<Rigidbody> ();
		shooter1.GetComponent<Rigidbody> ().useGravity = false;
		shooter1.AddComponent<MoveBall>();


	}


	void Update() {
		timeLeft -= Time.deltaTime;
		timeCount += Time.deltaTime;
		if (timeCount >= 1) {
			timeCount = 0;
			timeCountText.text = Mathf.Floor(timeLeft).ToString () + " seconds";
		}
		if (timeLeft <= 0)
			GameOver (false);

		if (Input.GetMouseButtonDown (0)) {


			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.CompareTag ("Shooter") || (hit.point.y >= -41.5 && hit.point.y <= -35.5 && hit.point.x >= -3 && hit.point.x <= 3)) {
					//Debug.Log ("Clicked shooter");
					if (remove_moves_left > 0) {
						Destroy (shooter1);
						shooter1 = shooter2; //shooter 2 becomes main shooter
						shooter2 = shooter3;

						shooter1.AddComponent<Rigidbody>(); 
						shooter1.AddComponent<MoveBall> ();
						shooter1.GetComponent<Rigidbody> ().useGravity = false;

						shooter1.transform.position = new Vector3 (0.0f, -38.5f, -2);
						shooter2.transform.position = new Vector3 (8.0f, -42.0f, -2);
						shooter3 = InstantiateElementBubble (new Vector3 (15.0f, -42.0f, -2), true);
						remove_moves_left--;
						bubblesCount--;
						if (remove_moves_left == 0) 
							highlighter.SetActive(false);
					}
				}
			}
		}
	}

	public void GameOver(bool finished) {
		ScoreTracker ST = GameObject.Find ("ScoreTracker").GetComponent<ScoreTracker> ();
		ST.score = totalScore;
		ST.allCompounds = allCompounds;
		string gameMode = "";
		if (SceneManager.GetActiveScene ().buildIndex == 2)
			gameMode = "ChallengeMode";
		else
			gameMode = "ClassicMode";

		if (totalScore > PlayerPrefs.GetInt (gameMode + "HighScore")) {
			PlayerPrefs.SetInt (gameMode + "HighScore", totalScore);
			ST.newHighScore = true;
		}
		if (finished)
			ST.ggwp = true;
		else
			ST.ggwp = false;
		SceneManager.LoadScene (5);
	}
	void DisableAll() {
		Remove.interactable = false;
		Increment.interactable = false;
		Bomb.interactable = false;
		TimeIncrease.interactable = false;
	}
	public void TimeIncreaseInitiate() {
		PlayerPrefs.SetString ("TimeIncrease", System.DateTime.Now.ToBinary().ToString());
		DisableAll ();
		timeLeft += 60;
	}
	public void BombInitiate() {
		PlayerPrefs.SetString ("Bomb", System.DateTime.Now.ToBinary().ToString());
		DisableAll ();
		explosive_moves_left = 5;
		highlighter.SetActive (true);
		highlighter.GetComponent<SpriteRenderer> ().color = Color.yellow;
	}
	public void IncrementInitiate() {
		PlayerPrefs.SetString ("Increment", System.DateTime.Now.ToBinary().ToString());
		DisableAll ();
		inc_moves_left = 5;
		highlighter.SetActive (true);
		highlighter.GetComponent<SpriteRenderer> ().color = Color.magenta;
	}
	public void RemoveInitiate() {
		PlayerPrefs.SetString ("Remove", System.DateTime.Now.ToBinary().ToString());
		DisableAll ();
		remove_moves_left = 5;
		highlighter.SetActive (true);
		highlighter.GetComponent<SpriteRenderer> ().color = Color.red;

	}
	void FixedUpdate() {

		if (shooter1.GetComponent<MoveBall> ().isMoving ()) {
			//if (inc_moves_left == 0 || explosive_moves_left == 0)
			highlighter.SetActive (false);
		}
		if (shooter1.GetComponent<MoveBall> ().Completed ()) { //Movement of main shooter is completed
			shooter1.tag = "Bubble";
			if (shooter1.transform.position.y <= -33.0f && !shooter1.CompareTag("Falling")) {
				GameOver (false);
				return;
			}

			KeyValuePair <int, string> pair = GameObject.Find ("BubbleController").GetComponent<BubbleController> ().CheckForCompounds (shooter1, ref totalScore, explosive_moves_left > 0, inc_moves_left > 0);
			if (inc_moves_left > 0) 
				inc_moves_left--;
			else if (explosive_moves_left > 0) 
				explosive_moves_left--;

			if (inc_moves_left > 0 || explosive_moves_left > 0 || remove_moves_left > 0)
				highlighter.SetActive (true);
			else {
				//Debug.Log ("hi");
				highlighter.SetActive (false);
			}

			

			int numDestroyed = pair.Key;
			string compoundFormed = pair.Value;
			if (compoundFormed != "null")
				allCompounds.Add (compoundFormed);
			
			Destroy (shooter1.GetComponent<MoveBall> ()); //shooter1 should no longer be able to move
			shooter1 = shooter2; //shooter 2 becomes main shooter
			shooter2 = shooter3;

			shooter1.AddComponent<Rigidbody>(); 
			shooter1.AddComponent<MoveBall> ();
			shooter1.GetComponent<Rigidbody> ().useGravity = false;

			shooter1.transform.position = new Vector3 (0.0f, -38.5f, -2);
			shooter2.transform.position = new Vector3 (8.0f, -42.0f, -2);
			shooter3 = InstantiateElementBubble (new Vector3 (15.0f, -42.0f, -2), true);

			bubblesCount -= numDestroyed;

			if (bubblesCount <= 3) { //All bubbles, other than shooter bubbles remain

				int bonus = (int)Mathf.Floor (timeLeft) * 10000;
				//BonusText.text = "Time Bonus! +" + bonus.ToString ();
				totalScore += bonus;
				GameOver(true);
			}
		}
	}
}
