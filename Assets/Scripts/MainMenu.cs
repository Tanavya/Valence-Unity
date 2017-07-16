using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{

	public int pos = 0;
    public void Play()
    {
		SceneManager.LoadSceneAsync(1);
    }
    public void About()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void Quit()
    {
        Application.Quit();
    }


}