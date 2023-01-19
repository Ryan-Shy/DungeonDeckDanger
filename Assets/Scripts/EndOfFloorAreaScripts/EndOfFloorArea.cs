using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfFloorArea : MonoBehaviour
{
	private bool isInZone = false;
	private bool isPaused = false;

	private void OnEnable()
	{
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	void Update()
	{
		if (!isPaused && isInZone && Input.GetKeyDown(UI_Hotkeys.GetHotkey("interact")))
		{
			// trigger menu
			EventManager.TriggerEvent("startPause");
			EventManager.TriggerEvent("endFloorMenu");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			isInZone = true;
			EventManager.TriggerEvent("showChestKey");
		}

	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			isInZone = false;
			EventManager.TriggerEvent("hideChestKey");
		}
	}

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}
}
