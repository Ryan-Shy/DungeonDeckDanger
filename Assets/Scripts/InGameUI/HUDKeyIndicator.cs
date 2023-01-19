using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDKeyIndicator : MonoBehaviour
{
	[SerializeField] GameObject keyIndicator;

	private bool isPaused;
	private bool show;
	private bool updateUI;

	private void OnEnable()
	{
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
		EventManager.StartListening("showChestKey", OnShowInteractKey);
		EventManager.StartListening("hideChestKey", OnHideInteractKey);
		EventManager.StartListening("showDoorKey", OnShowInteractKey);
		EventManager.StartListening("hideDoorKey", OnHideInteractKey);
	}

	private void OnDisable()
	{
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
		EventManager.StopListening("showChestKey", OnShowInteractKey);
		EventManager.StopListening("hideChestKey", OnHideInteractKey);
		EventManager.StopListening("showDoorKey", OnShowInteractKey);
		EventManager.StopListening("hideDoorKey", OnHideInteractKey);
	}

	// Start is called before the first frame update
	void Start()
    {
		keyIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if (!updateUI) return;
		if (!isPaused)
		{
			keyIndicator.SetActive(show);
		}
		else
		{
			keyIndicator.SetActive(false);
		}
    }

	private void OnStartPause()
	{
		isPaused = true;
		updateUI = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
		updateUI = true;
	}

	private void OnShowInteractKey()
	{
		show = true;
		keyIndicator.transform.Find("Background").Find("Key Text").GetComponent<TMPro.TextMeshProUGUI>().text = UI_Hotkeys.GetHotkey("interact").ToString();
		updateUI = true;
	}

	private void OnHideInteractKey()
	{
		show = false;
		updateUI = true;
	}

}
