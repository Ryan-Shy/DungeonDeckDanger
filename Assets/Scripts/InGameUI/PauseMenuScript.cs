using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// This script should be attached to the Pause Manager
/// It takes care of opening/closing the pause menu
/// as well as all the buttons within the pauseMenu
/// </summary>
public class PauseMenuScript : MonoBehaviour
{
	[SerializeField] private GameObject pauseMenu;
	private bool isPaused = false;
	private float prevTimeScale = 1;

	private void OnEnable()
	{
		EventManager.StartListening("pauseMenu", ToggleActive);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("pauseMenu", ToggleActive);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("pause")))
		{
			if (isPaused && pauseMenu.activeSelf)
			{
				EventManager.TriggerEvent("endPause"); // tell other components to resume
				EventManager.TriggerEvent("pauseMenu"); // close pause menu
			}
			else if (!isPaused)
			{
				EventManager.TriggerEvent("startPause"); // tell other components to pause
				EventManager.TriggerEvent("pauseMenu"); // open pause menu
			}
		}
	}

	private void ToggleActive()
	{
		bool curActive = pauseMenu.activeSelf;
		pauseMenu.SetActive(!curActive);
	}

	private void OnStartPause()
	{
		// do stuff that needs to happen on every pause
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		if(!isPaused) prevTimeScale = Time.timeScale;
		Time.timeScale = 0;
		isPaused = true;
	}

	private void OnEndPause()
	{
		// do stuff that needs to happen on every unpause
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		if(isPaused) Time.timeScale = prevTimeScale;
		isPaused = false;
	}

	public void Resume()
	{
		ToggleActive();
		EventManager.TriggerEvent("endPause");
	}

	public void Inventory()
	{
		ToggleActive();
		EventManager.TriggerEvent("inventoryMenu");
	}

	public void MainMenu()
	{
		EventManager.TriggerEvent("save"); // Save event
		Saves.current.SaveToDisk();
		EventManager.TriggerEvent("endPause");
		SceneManager.LoadScene("MainMenu");
	}

	public void Settings()
	{
		ToggleActive();
		EventManager.TriggerEvent("settingsMenu");
	}

	public void Help()
	{
		ToggleActive();
		EventManager.TriggerEvent("helpMenu");
	}

	public void Quit()
	{
		EventManager.TriggerEvent("save"); // Save event
		Saves.current.SaveToDisk();
		Saves.profile.SaveToDisk();
		Application.Quit();
	}
}
