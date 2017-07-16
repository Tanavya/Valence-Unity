using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolume : MonoBehaviour {

	// Use this for initialization
	private float original_volume;
	public bool isSoundEffect = false;
	void Start () {
		original_volume = gameObject.GetComponent<AudioSource> ().volume;
		if (isSoundEffect) {
			Debug.Log (PlayerPrefs.GetInt ("SoundEffects", 1));
			gameObject.GetComponent<AudioSource> ().volume = original_volume * PlayerPrefs.GetInt ("SoundEffects", 1);
		} else {
			gameObject.GetComponent<AudioSource> ().volume = original_volume * PlayerPrefs.GetFloat("Volume", 1.0f);
		}
	}

	public void updateVolume(float factor) {
		PlayerPrefs.SetFloat ("Volume", factor);
		gameObject.GetComponent<AudioSource> ().volume = original_volume * factor;
		PlayerPrefs.Save ();
	}
	public void updateSoundEffects(bool toggle) {
		Debug.Log (System.Convert.ToInt32 (toggle));
		PlayerPrefs.SetInt ("SoundEffects", System.Convert.ToInt32(toggle));
		PlayerPrefs.Save ();
	}


}
