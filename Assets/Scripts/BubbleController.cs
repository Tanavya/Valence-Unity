using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BubbleController : MonoBehaviour {

	public Dictionary <string, string> cations;
	public Dictionary <string, string> anions;
	public Dictionary <string, int> ions;
	public HashSet <string> metals;
	public Text compoundsData, ScoreCount, StreakCount;
	public AudioSource source;
	public AudioClip bubblePop;
	private List <List<GameObject>> compoundsFound;
	private GameObject start;
	int max_allowed = 1;
	int numDestroyed;
	int streak;

	void Start () {
		cations = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().cations;
		anions = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().anions;
		ions = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().ions;
	}
	void Awake() {
		source = GetComponent<AudioSource> ();
	}
	private int gcd(int a, int b) { return b == 0 ? a : gcd(b, a % b); }

	public void swap(ref string a, ref string b) {
		string temp = a;
		a = b;
		b = temp;
	}
	public void swap2(ref int a, ref int b) {
		int temp = a;
		a = b;
		b = temp;
	}
	 
	public List <string> createCompound(List <GameObject> elements) {
		List <string> ret = new List<string> {"0", "null", "null"};
		string ion1 = "", ion2 = "", name = "";
		GameObject el1 = gameObject, el2 = gameObject;
		int count1 = 0, count2 = 0; 

		foreach (GameObject el in elements) {
			string elementName = el.GetComponent<ElementData> ().elementName;
			if (ion1 == "") {
				el1 = el;
				ion1 = elementName;
			} else if (ion2 == "" && elementName != ion1) {
				el2 = el;
				ion2 = elementName;
			}
			if (elementName == ion1)
				count1++;
			else
				count2++;
		}

		//Debug.Log ("Element 1 name: " + ion1 + ", Element 2 name: " + ion2);
		//Debug.Log ("Count of el 1: " + count1.ToString () + ", Count of el 2: " + count2.ToString ());

		int v11 = el1.GetComponent<ElementData> ().valency;
		int v12 = el1.GetComponent<ElementData> ().valency2;
		int v21 = el2.GetComponent<ElementData> ().valency;
		int v22 = el2.GetComponent<ElementData> ().valency2;

		string type1 = el1.GetComponent<ElementData>().type, type2 = el2.GetComponent<ElementData>().type;
		string symbol1 = "", symbol2 = "", formula = "";

		int num = gcd (count1, count2);
		count1 /= num;
		count2 /= num;
		if (v11 * count1 + v21 * count2 == 0 || v11 * count1 + v22 * count2 == 0 || v12 * count1 + v21 * count2 == 0 || v12 * count1 + v22 * count2 == 0) {
			if ((type1 == "anion" && type2 == "cation") || (ion1 == "Hydrogen" && type2 == "cation")) {
				swap (ref ion1, ref ion2);
				swap2 (ref count1, ref count2);
			}
			symbol1 = cations [ion1];
			symbol2 = anions [ion2];
		} else {
			return ret;
		}
		if (num > max_allowed)
			return ret;
		if (ion2 == "Hydrogen") {
			ion2 = "Hydride";
		} 
		if (count1 > 1) {
			formula += symbol1 + count1.ToString();
		} else {
			symbol1 = symbol1.Replace ("(", "").Replace (")", "");
			formula += symbol1;
		}
		if (count2 > 1) {
			formula += symbol2 + count2.ToString ();
		} else {
			symbol2 = symbol2.Replace ("(", "").Replace (")", "");
			formula += symbol2;
		}
		name = ion1 + " " + ion2;

		ret = new List <string> {num.ToString(), name, formula};
		//if (num == 2) printList (elements, "Compound with 2 molecules: ");
		return ret;
	}
	int countUnique(List <GameObject> listx) {
		var set = new HashSet<string>();
		foreach (GameObject item in listx) {
			string elName = item.GetComponent<ElementData> ().elementName;
			if (!set.Contains(elName)) set.Add(elName);
		}
		return set.Count;
	}
		
	List <string> ExceptionCheck(List <GameObject> compound) {

		//if (countUnique (compound) < 2) {
		//	//Debug.Log ("Count of unique elements is less than 2!");
		//}
		List <string> data = createCompound (compound);
		string count = data [0];
		string name = data [1];
		string formula = data [2];

		switch (name) {
		case "Hydrogen Nitride":
			name = "Ammonia";
			formula = "NH3";
			break;
		case "Hydrogen Carbonate":
			name = "Carbonic acid";
			break;
		case "Hydrogen Oxide":
			name = "Dihydrogen Monoxide";
			formula = "H2O";
			break;
		case "Hydrogen Hydroxide":
			name = "Dihydrogen Monoxide";
			formula = "H2O";
			break;
		case "Hydrogen Hydride":
			name = "null";
			break;
		}
			
		return new List <string> { count, name, formula };
	}
	private void printList(List <GameObject> list, string extra_message = "") {
		string foo = extra_message;
		foreach (GameObject each in list) {
			foo += each.GetComponent<ElementData>().elementName + " ";
		}
		//Debug.Log (foo);
	}
	public void rec(List <GameObject> currBubbles) {

		//Debug.Log (currBubbles.Count);
		printList(currBubbles);
		if (countUnique (currBubbles) > 2 || currBubbles.Count == 0) 
			return;
		
		if (countUnique (currBubbles) == 2) {
			if (ExceptionCheck (currBubbles) [1] != "null") {
				//Debug.Log (currBubbles.Count);
				if (currBubbles.Contains(start))
					compoundsFound.Add (currBubbles);
			}
		}
		
		GameObject currBubble = currBubbles [currBubbles.Count - 1];
		//foreach (GameObject currBubble in currBubbles) {
			
		Collider[] neighbours = Physics.OverlapSphere (currBubble.transform.position, 4.0f);
		foreach (Collider sphere in neighbours) {
			if (sphere.CompareTag ("Bubble")) {
				if (!currBubbles.Contains (sphere.gameObject)) {
					List <GameObject> temp = new List <GameObject> (currBubbles);
					temp.Add (sphere.gameObject);
					rec (temp);
				}
			}
		}
		//}
	}
	public void DepthFirstSearch() { 

		//Debug.Log ("Starting DFS");

		Stack <List<GameObject>> stk = new Stack <List<GameObject>>();
		Collider[] adjacent = Physics.OverlapSphere (start.transform.position, 4.0f);
		foreach (Collider beg in adjacent) {
			if (beg.CompareTag("Bubble")) {
				stk.Push (new List<GameObject> {beg.gameObject});
				while (stk.Count > 0) {
					List <GameObject> currBubbles = stk.Pop ();
					GameObject currBubble = currBubbles [currBubbles.Count - 1];
					//printList (currBubbles);
					Collider[] neighbours = Physics.OverlapSphere (currBubble.transform.position, 4.0f);
					foreach (Collider sphere in neighbours) {
						if (sphere.CompareTag ("Bubble")) {
							List <GameObject> temp = new List <GameObject> (currBubbles);
							if (!temp.Contains (sphere.gameObject)) {
								temp.Add (sphere.gameObject);
								int cu = countUnique (temp);
								bool contains = temp.Contains (start);
								if (cu <= 2) {
									if (cu == 2) {
										//Debug.Log ("Checking if compound forms");
										//printList (temp, "Checking if compound forms on compound: ");
										if (contains) {
											if (ExceptionCheck(temp)[1] != "null") 
												compoundsFound.Add (temp);
										}
									}
									//if (temp.Contains(start)) 
									//if (!(cu == 2 && !contains)) stk.Push (new List <GameObject> (temp));
									if (contains) stk.Push (new List <GameObject> (temp));
								}
							}
						}
					}
				}
			}
		}
	}

	public void DepthFirstSearch2() { 

		//Debug.Log ("Starting DFS");
		compoundsFound.Add(new List <GameObject> () {start});
		Stack <List<GameObject>> stk = new Stack <List<GameObject>>();
		Collider[] adjacent = Physics.OverlapSphere (start.transform.position, 4.0f);
		foreach (Collider beg in adjacent) {
			if (beg.CompareTag("Bubble")) {
				stk.Push (new List<GameObject> {beg.gameObject});
				while (stk.Count > 0) {
					List <GameObject> currBubbles = stk.Pop ();
					GameObject currBubble = currBubbles [currBubbles.Count - 1];
					//printList (currBubbles);
					Collider[] neighbours = Physics.OverlapSphere (currBubble.transform.position, 4.0f);
					foreach (Collider sphere in neighbours) {
						if (sphere.CompareTag ("Bubble")) {
							List <GameObject> temp = new List <GameObject> (currBubbles);
							if (!temp.Contains (sphere.gameObject) && sphere.GetComponent<ElementData>().elementName == currBubble.GetComponent<ElementData>().elementName) {
								temp.Add (sphere.gameObject);
								bool contains = temp.Contains (start);
								if (contains) {
									stk.Push (new List <GameObject> (temp));
									compoundsFound.Add (new List <GameObject> (temp));
								}
							}
						}
					}
				}
			}
		}
	}
	public List <GameObject> BreadthFirstSearch(GameObject start) { 
		
		Queue <List<GameObject>> Q = new Queue <List<GameObject>>();
	
		List<GameObject> chain = new List<GameObject> {start};
		Collider[] adjacent = Physics.OverlapSphere (start.transform.position, 4.0f);
		foreach (Collider beg in adjacent) {
			if (beg.CompareTag("Bubble")) {
				Q.Enqueue (new List<GameObject> {beg.gameObject});
				while (Q.Count > 0) {
					List <GameObject> currBubbles = Q.Dequeue ();
					chain = currBubbles;
					GameObject currBubble = currBubbles [currBubbles.Count - 1];

					Collider[] neighbours = Physics.OverlapSphere (currBubble.transform.position, 4.0f);
					foreach (Collider sphere in neighbours) {
						if (sphere.CompareTag ("Bubble")) {
							List <GameObject> temp = new List <GameObject> (currBubbles);
							if (!temp.Contains (sphere.gameObject) && currBubble.GetComponent<ElementData>().elementName == sphere.GetComponent<ElementData>().elementName) {
								temp.Add (sphere.gameObject);
								Q.Enqueue (new List <GameObject> (temp));
							}
						}
					}
				}
			}
		}
		return chain;

	}

	public KeyValuePair<int,string> CheckForCompounds(GameObject shooterBubble, ref int totalScore, bool is_explosive, bool is_increment) {

		compoundsFound = new List<List<GameObject>> ();
		numDestroyed = 0;
		List <string> data = new List <string> { "0", "null", "null" };
		int points = 0;
		start = shooterBubble;
		if (is_increment) {
			max_allowed = 2;
			//Debug.Log ("Incremented!");
		}
		else
			max_allowed = 1;
		if (is_explosive) {
			//Debug.Log ("EXPLOSIVE!!!!");
			DepthFirstSearch2 ();
			compoundsFound.Sort (delegate(List<GameObject> l1, List<GameObject> l2) { return l1.Count.CompareTo(l2.Count);}); 
			List <GameObject> to_destroy = compoundsFound [compoundsFound.Count - 1];
			numDestroyed = to_destroy.Count;
			DestroyBubbles (to_destroy);
			points += numDestroyed * 100;
			streak++;

			compoundsData.text = numDestroyed.ToString () + " " + shooterBubble.GetComponent<ElementData>().elementName + " ions destroyed";
		} else {
			DepthFirstSearch ();
			if (compoundsFound.Count > 0) {
				compoundsFound.Sort (delegate(List<GameObject> l1, List<GameObject> l2) { return l1.Count.CompareTo(l2.Count);}); 
				List<GameObject> bestCompound = compoundsFound [compoundsFound.Count - 1];

				foreach (List<GameObject> compound in compoundsFound) {
					printList (compound, "Compound Length " + compound.Count.ToString() + ": ");
					if (compound.Count > bestCompound.Count)
						Debug.Assert (false);
				}
			
				//if (!bestCompound.Contains (shooterBubble))
				//Debug.Log ("Shooter bubble not present in compound");
				data = ExceptionCheck (bestCompound);
				if (System.Convert.ToInt32(data[0]) > 1) compoundsData.text = data [0] + " molecules of " + data [1] + "(" + data [2] + ")"; 
				else compoundsData.text = data [0] + " molecule of " + data [1] + "(" + data [2] + ")"; 
				numDestroyed = bestCompound.Count;
				DestroyBubbles (bestCompound); 
				points += numDestroyed * 100;
				streak++;
				//StreakCount.color = colors [(streak - 1) % 4];
			} else {
				compoundsData.text = "No compound formed";
				if (totalScore >= 500)
					totalScore -= 500;
				streak = 0;
				//StreakCount.color = Color.black;
			}
		}
		points *= streak;
		totalScore += points;

		StreakCount.text = streak.ToString ();
		ScoreCount.text = totalScore.ToString();
		return new KeyValuePair<int, string> (numDestroyed, data [1]);
	}
	/*
	List <GameObject> FindIsolated(GameObject start, List<GameObject> visited, ref bool found) {
		List <GameObject> isolated = new List <GameObject> {start};
		visited.Add(start);
		Queue <GameObject> Q = new Queue <GameObject> ();
		Q.Enqueue (start);
		while (Q.Count > 0) {
			GameObject curr = Q.Dequeue ();
			Collider[] neighbours = Physics.OverlapSphere (curr.transform.position, 3.0f);
			foreach (Collider neighbour in neighbours) {
				if (neighbour.CompareTag ("Wall")) {
					found = false;
					//return isolated;
				}
				if (neighbour.CompareTag ("Bubble") && !visited.Contains (neighbour.gameObject)) {
					visited.Add (neighbour.gameObject);
					isolated.Add (neighbour.gameObject);
					Q.Enqueue (neighbour.gameObject);
				}
			}
		}
		visited = new List <GameObject> ();
		return isolated;

	}
	*/
	void DestroyBubbles(List <GameObject> toDestroy) { //Destroy bubbles 
		List <GameObject> visited = new List <GameObject> ();
		bool found = true;
		foreach (GameObject bubble in toDestroy) {
			
			Destroy (bubble);
			source.Play ();
			visited.Add (bubble);
		}
		GameObject[] allBubbles = GameObject.FindGameObjectsWithTag ("Bubble");
		Queue <GameObject> Q = new Queue <GameObject> ();
		foreach (GameObject bubble in allBubbles) {
			if (!visited.Contains(bubble)) {
				List <GameObject> cluster = new List<GameObject> ();
				visited.Add (bubble);
				cluster.Add (bubble);
				found = true;
				Q.Enqueue (bubble);
				while (Q.Count > 0) {
					GameObject curr = Q.Dequeue ();
					Collider[] neighbours = Physics.OverlapSphere (curr.transform.position, 5.0f);
					foreach (Collider neighbour in neighbours) {
						if (neighbour.CompareTag("Wall")) {
							found = false;
						}
						if (neighbour.CompareTag ("Bubble") && !visited.Contains (neighbour.gameObject)) {
							visited.Add (neighbour.gameObject);
							cluster.Add (neighbour.gameObject);
							Q.Enqueue (neighbour.gameObject);
						}
					}
				}
				if (found) {
					numDestroyed += cluster.Count;
					foreach (GameObject fall_bubble in cluster) {
						fall_bubble.transform.position += new Vector3 (0, 0, -10);
						if (fall_bubble.GetComponent<Rigidbody> () == null) {
							fall_bubble.AddComponent<Rigidbody> ();

						}
						else {
							fall_bubble.GetComponent<Rigidbody> ().isKinematic = false;
							fall_bubble.GetComponent<Rigidbody> ().useGravity = true;
						}
						fall_bubble.tag = "Falling";
					}
				}
			}
		}
	}
}
