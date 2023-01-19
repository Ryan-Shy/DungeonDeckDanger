using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
	[SerializeField] private GameObject gameOverMenu;

	private void OnEnable()
	{
		EventManager.StartListening("gameOverMenu", ToggleActive);
	}

	private void OnDisable()
	{
		EventManager.StopListening("gameOverMenu", ToggleActive);
	}

	private void ToggleActive()
	{
		bool curActive = gameOverMenu.activeSelf;
		gameOverMenu.SetActive(!curActive);
	}


	public void MainMenu()
	{
		// brick save (avoid scumming)
		EventManager.TriggerEvent("save"); // Save event
		Saves.current.SaveToDisk();
		Saves.profile.SaveToDisk();
		// delete from disk
		string name = Saves.current.GetName();
		Saves.current = null;
		Saves.DeleteSave(Saves.GetSaveId(name));
		EventManager.TriggerEvent("endPause");
		SceneManager.LoadScene("MainMenu");
	}
}
