using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MoveBall : MonoBehaviour
{

	private float diameter = 6.0f;
	Vector3 offset1, offset2, offset3, offset4;
	Vector3 newPosition;
	public int speed = 150;
	Rigidbody rb;
	bool clickRecorded = false, moving = false, moved = false;
	public Sprite yellow, blue, magenta, pink, green, cyan, orange, red;
	public GameObject aim_line;
	List<GameObject> aim_sprites;
	void Start ()
	{

		newPosition = transform.position;
		rb = GetComponent<Rigidbody> ();
		//gameObject.GetComponent<Sprite>().
		offset1 = new Vector3 (-diameter, 0.0f, 0.0f);
		offset2 = new Vector3 (-diameter * 0.5f, -diameter * (float)(Mathf.Sqrt (diameter / 2) / 2.0f), 0.0f);
		offset3 = new Vector3 (diameter * 0.5f, -diameter * (float)(Mathf.Sqrt (diameter / 2) / 2.0f), 0.0f);
		offset4 = new Vector3 (diameter, 0.0f, 0.0f);
		aim_line = GameObject.Find ("AimLine");
		aim_sprites = new List<GameObject> ();
		//Debug.Log (aim_line.transform.childCount);
		//Debug.Log(gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name);

		for (int i = 0; i < aim_line.transform.childCount; i++) {
			aim_line.transform.GetChild (i).transform.GetComponent<SpriteRenderer> ().sprite = 
				gameObject.GetComponent<ElementData> ().color.GetComponent < SpriteRenderer > ().sprite; 
			aim_sprites.Add (aim_line.transform.GetChild (i).gameObject);
		}
		aim_sprites.Reverse ();
	}
	
	void Update ()
	{	
		if (Input.GetMouseButtonUp (0) && !clickRecorded) { //Checks for user click. 

			for (int i = 0; i < 21; i++) {
				aim_sprites [i].transform.position = new Vector3 (0, 0, -100);
			}
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit) && hit.point.y > -32) {
				newPosition = (hit.point - transform.position) * 100 + transform.position; // To make sure that even if the user clicks away from the balls, it still reaches them
				newPosition.z = -2;
				moving = true;
				clickRecorded = true;
			}
		} else if (Input.GetMouseButton (0) && !clickRecorded) {

			RaycastHit hit, hit2;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit) && hit.point.y > -35) {

				Vector3 start = new Vector3 (0.0f, -40.0f, -2);
				Vector3 direction = (hit.point - start) * 100;
				direction.z = 0;

				Physics.SphereCast (start, 1.3f, direction, out hit2);
				//lineRenderer.SetPositions(new Vector3[] {start, hit2.point});

				Vector3 end = hit2.point;
				end.z = -2;
				direction = end - start;

				float dist = Vector3.Distance (end, start);
				float count = dist / 4.0f;
				Vector3 increment = direction / count;
				Vector3 pos = start;

				for (int i = 0; i < Mathf.CeilToInt(count) && i < 21; i++) {
					//aim_sprites [i].transform.position.z = -2;
					aim_sprites [i].transform.position = pos;
					pos += increment;
				}
				if (hit2.transform.CompareTag ("Wall") && hit2.transform.name != "Top Wall") {

					if (Overlaps (end, 2.5f, "Bubble")) {
						expandBubble (4.5f);
						//Debug.Log ("Overlapping on wall");
					} else {
						//GameObject temp = Instantiate (GameObject.CreatePrimitive(PrimitiveType.Sphere), end, Quaternion.identity) as GameObject;
						//temp.transform.localScale = new Vector3 (2, 2, 2);
						//Debug.Log ("Not overlapping on wall");
						expandBubble (2.6f);
					}
					pos = end;
					increment = Vector3.Reflect (increment, Vector3.right);
					pos += increment;
					int i = Mathf.CeilToInt (count);
					for (int j = 0; i < 21 && j < 3; i++, j++) {
						//Debug.Log (increment);
						aim_sprites [i].transform.position = pos;
						pos += increment;
					}
					for (; i < 21; i++) {
						aim_sprites [i].transform.position = new Vector3 (0, 0, -100);
					}
				} else {
					for (int i = Mathf.CeilToInt (count); i < 21; i++) {
						aim_sprites [i].transform.position = new Vector3 (0, 0, -100);
					}
				}
			}
		}

		if (transform.position == newPosition)
			moving = false;
	}
	bool Overlaps(Vector3 pos, float radius, string tag) {
		Collider[] neighbours = Physics.OverlapSphere (pos, radius);
		foreach (Collider neighbour in neighbours) {
			if (neighbour.CompareTag (tag) && neighbour.gameObject != gameObject) {
				return true;
			}
		}
		return false;
	}
	void FixedUpdate ()
	{

		if (moving) {
			transform.position = Vector3.MoveTowards (transform.position, newPosition, Time.deltaTime * speed);
		}
	}

	void SetPosition (Collision collision, Vector3 offset)
	{ 
		if (Overlaps (collision.transform.position + offset, 2.0f, "Wall") || Overlaps (collision.transform.position + offset, 2.5f, "Bubble")) {
			//Debug.Log ("Overlap alert!");
			if (offset == offset1)
				SetPosition (collision, offset2);
			else if (offset == offset2)
				SetPosition (collision, offset3);
			else if (offset == offset3)
				SetPosition (collision, offset4);
			else if (offset == offset4)
				SetPosition (collision, offset1);
			return;
		}
		//Debug.Log ("OK. No Overlap!");
		transform.position = collision.transform.position + offset;
		rb.isKinematic = true;
	}
	void expandBubble(float size) {
		GameObject cover = gameObject.transform.GetChild (0).gameObject;
		GameObject text = gameObject.transform.GetChild (1).gameObject;
		gameObject.transform.DetachChildren ();
		gameObject.transform.localScale = new Vector3 (size, size, size);
		cover.transform.parent = gameObject.transform;
		text.transform.parent = gameObject.transform;
		gameObject.transform.eulerAngles = new Vector3 (0, 90, 0);
	}
	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.CompareTag ("Bubble") && !moved) {

			//if (gameObject.transform.localScale != new Vector3 (6,6,6)) {
			expandBubble(6.0f);
			float angle = Vector3.Angle (Vector3.right, collision.gameObject.transform.position - gameObject.transform.position);

			Vector3 offset = Vector3.zero;
			if (angle < 45 && angle >= 0) {
				offset = offset1;
				if (Overlaps (collision.transform.position + offset, 2.0f, "Wall") || Overlaps (collision.transform.position + offset, 2.0f, "Bubble"))
					offset = offset2;
			}
			else if (angle <= 90 && angle >= 45) {
				offset = offset2;
				if (Overlaps (collision.transform.position + offset, 2.0f, "Wall") || Overlaps (collision.transform.position + offset, 2.0f, "Bubble"))
					offset = offset3;
			} else if (angle <= 135 && angle >= 90) {
				offset = offset3;
				if (Overlaps (collision.transform.position + offset, 2.0f, "Wall") || Overlaps (collision.transform.position + offset, 2.0f, "Bubble"))
					offset = offset2;
			} else if (angle <= 180 && angle >= 135) {
				offset = offset4;
				if (Overlaps (collision.transform.position + offset, 2.0f, "Wall") || Overlaps (collision.transform.position + offset, 2.0f, "Bubble"))
					offset = offset3;
			}
			//else {
			//	Debug.Log ("What now?!?!");
			//}

			SetPosition (collision, offset);
			moving = false;
			moved = true;
			gameObject.transform.eulerAngles = new Vector3 (0, 90, 0);
			//}
				
		}
		if (collision.gameObject.CompareTag ("Wall")) { //To make ball bounce off walls.

			if (collision.gameObject.name == "Top Wall") {

				expandBubble (6.0f);
				moved = true;
				moving = false;
				rb.isKinematic = true;
				float x = (transform.position.x + 27.0f) / diameter;
				bool overlaps = Overlaps (new Vector3 (-27.0f + diameter * Mathf.Round (x), 33.0f, -2.0f), 2.95f, "Bubble");
				if (!overlaps)
					transform.position = new Vector3 (-27.0f + diameter * Mathf.Round (x), 33.0f, -2.0f);
				else {
					float temp = Mathf.Floor (x);
					if (temp == Mathf.Round (x))
						temp = Mathf.Ceil (x);
					transform.position = new Vector3 (-27.0f + diameter * temp, 33.0f, -2.0f);
				}
				//rb.isKinematic = true;

				gameObject.transform.eulerAngles = new Vector3 (0, 90, 0);

			} else if (collision.gameObject.name == "Bottom Wall") {
				newPosition = Vector3.Reflect (newPosition, Vector3.up);
			} else {
				if (Overlaps (transform.position, 2.0f, "Bubble")) {
					
					//Debug.Log ("Colliding on wall, expanding bubble.");
					Vector3 offset;
					if (collision.gameObject.name == "Right Wall")
						offset = offset2;
					else
						offset = offset3;
					transform.position += offset;
					expandBubble (5.5f);
				}
				newPosition = Vector3.Reflect (newPosition, Vector3.right);
			}
		}

	}


	public bool Completed ()
	{
		return moved;
	}
	public bool isMoving() {
		return moving;
	}

}