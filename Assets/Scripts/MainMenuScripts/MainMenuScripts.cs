using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		Time.timeScale = 1;
	}

	public void NewGame()
	{
		Debug.Log("New Game");
		SceneManager.LoadScene("NewGame");
	}

	public void Continue()
	{
		Debug.Log("Continue");
		Saves.LoadSceneFromSave(Saves.current);
	}

	public void LoadGame()
	{
		Debug.Log("Load Game");
		SceneManager.LoadScene("LoadGame");
	}

	public void Settings()
	{
		Debug.Log("Settings");
		SceneManager.LoadScene("Settings");
	}

	public void Credits()
	{
		Debug.Log("Credits");
		SceneManager.LoadScene("Credits");
	}

	public void Exit()
	{
		Debug.Log("Exit");
		Application.Quit();
	}

}
