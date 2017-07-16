using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public GameObject help, playScreen, controller, how_to_play, settings;
	private int pos = 0;
	public void LoadSceenWithLoading(int scene) {
		LoadingScreenManager.LoadScene (scene);
	}
	public void LoadScene(int scene) {
		SceneManager.LoadScene (scene);
	}
	public void Quit() {
		Application.Quit ();
	}
	public void HelpScreen() {
		help.GetComponent<RectTransform> ().anchoredPosition = new Vector3(0, 0, 0);
	}
	public void HomeScreen() {
		help.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (-1280, 0, 0);
		playScreen.GetComponent<RectTransform>().anchoredPosition = new Vector3 (-3000, 0, 0);
		settings.GetComponent<RectTransform>().anchoredPosition = new Vector3 (1280, 0, 0);
	}
	public void SettingsScreen() {
		settings.GetComponent<RectTransform>().anchoredPosition = new Vector3 (0, 0, 0);
	}
	public void PlayOptionsScreen() {
		playScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
	}
	public void ClickArrow(int dir) {
		if (pos == 0 && dir == 1) {
			controller.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (-106.8f, 0, 0);
			pos = 1;
		} else if (pos == 1 && dir == 0) {
			pos = 0;
			controller.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
		}
		Debug.Log ("Arrow Clicked!");
	}
	public void ShowHowToPlay() {
		how_to_play.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, 0, 0);
	}
	public void OKHowToPlay() {
		PlayerPrefs.SetInt ("HelpRead", 1);
		how_to_play.GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0, -110, 0);
		PlayerPrefs.Save ();
	}
}
