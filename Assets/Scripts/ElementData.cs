using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class ElementData : MonoBehaviour {

	//public Dictionary <string, string> cations;
	//public Dictionary <string, string> anions;
	public Dictionary <string, int> ions;
	public List <string> cations, anions;
	public string elementName;
	public string type;
	public int valency, valency2 = -100;
	public GameObject color;
	public void SetValues(string inpname, GameObject col) {
		cations = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().cationsNames;
		anions = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().anionsNames;
		ions = GameObject.Find ("InstantiateBubbles").GetComponent<InstantiateSpheres> ().ions;
		elementName = inpname;
		valency = ions [elementName];
		if (elementName == "Hydrogen") {
			//valency2 = -1;
			valency2 = -1000; //No hydrides lolol makes it too easy
		}
		else if (elementName == "Iron")
			valency2 = 3;
		if (cations.Contains(elementName))
			type = "cation";
		if (anions.Contains(elementName))
			type = "anion";
		color = col;
	}
}
