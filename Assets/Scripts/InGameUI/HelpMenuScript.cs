using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuScript : MonoBehaviour
{
	[SerializeField] private GameObject helpMenu;
	[SerializeField] private int pageCount = 1;
	[SerializeField] private int pageNr = 0;

	private bool isPaused = false;
	private bool isActive = false;

	private void OnEnable()
	{
		EventManager.StartListening("helpMenu", ToggleActive);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
		// hide previous button on first page
		if (pageNr == 0)
		{
			helpMenu.transform.Find("Back").gameObject.SetActive(false);
		}
		// hide next button on last page
		if (pageNr == pageCount - 1)
		{
			helpMenu.transform.Find("Back").gameObject.SetActive(false);
		}
		for(int p = 0; p < pageCount-1; p++)
		{
			helpMenu.transform.Find("Pages").GetChild(p).gameObject.SetActive(p==pageNr);
		}
	}

	private void OnDisable()
	{
		EventManager.StopListening("helpMenu", ToggleActive);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	void Update()
	{
		if (isPaused && helpMenu.activeSelf)
		{
			if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("pause")))
			{
				Resume();
			}
		}
	}

	private void ToggleActive()
	{
		isActive = !helpMenu.activeSelf;
		helpMenu.SetActive(isActive);
	}

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}

	public void Previous()
	{
		if (pageNr == 0) return;
		// show next button if we move back from last page
		if(pageNr == pageCount - 1)
		{
			helpMenu.transform.Find("Next").gameObject.SetActive(true);
		}
		// switch pages
		helpMenu.transform.Find("Pages").GetChild(pageNr).gameObject.SetActive(false);
		pageNr--;
		helpMenu.transform.Find("Pages").GetChild(pageNr).gameObject.SetActive(true);

		// hide button on first page
		if(pageNr == 0)
		{
			helpMenu.transform.Find("Back").gameObject.SetActive(false);
		}
	}

	public void Resume()
	{
		ToggleActive();
		EventManager.TriggerEvent("endPause");
	}

	public void Next()
	{
		if (pageNr == pageCount - 1) return;
		// show next button if we move forward from first page
		if (pageNr == 0)
		{
			helpMenu.transform.Find("Back").gameObject.SetActive(true);
		}
		// switch pages
		helpMenu.transform.Find("Pages").GetChild(pageNr).gameObject.SetActive(false);
		pageNr++;
		helpMenu.transform.Find("Pages").GetChild(pageNr).gameObject.SetActive(true);

		// hide button on last page
		if (pageNr == pageCount - 1)
		{
			helpMenu.transform.Find("Next").gameObject.SetActive(false);
		}
	}
}
