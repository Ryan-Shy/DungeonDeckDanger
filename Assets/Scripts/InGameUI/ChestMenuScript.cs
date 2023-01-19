using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestMenuScript : MonoBehaviour
{
	[SerializeField] private GameObject chestMenu;
	[SerializeField] private GameObject contentPane;
	[SerializeField] private GameObject PackTemplate;
	private bool isPaused = false;
	private Queue<GameObject> currentPacks = new();
	// when a chest is opened, set this variable to the correct value for that chest
	public static List<Pack> chestPacks = new();

	private void OnEnable()
	{
		EventManager.StartListening("chestMenu", ToggleActive);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("chestMenu", ToggleActive);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

    // Update is called once per frame
    void Update()
    {
		if(isPaused && chestMenu.activeSelf)
		{
			if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("inventory")) || Input.GetKeyDown(UI_Hotkeys.GetHotkey("pause")))
			{
				EventManager.TriggerEvent("endPause"); // tell other components to resume
				EventManager.TriggerEvent("chestMenu"); // close chest menu
			}
		}
	}

	private void ToggleActive()
	{
		bool curActive = chestMenu.activeSelf;
		// create view of packs / remove when toggling off
		if (curActive)
		{
			// turn off
			while (currentPacks.TryDequeue(out GameObject slot))
			{
				Destroy(slot);
			}
		}
		else
		{
			// turn on
			foreach (Pack pack in chestPacks)
			{
				GameObject slot = Instantiate(PackTemplate);
				slot.transform.SetParent(contentPane.transform);
				PackScript packScript = slot.GetComponent<PackScript>();
				packScript.SetID(pack.packID.ToString());
				packScript.SetPack(pack);
				packScript.addToList = AddToCollection;
				packScript.removeFromList = RemoveFromCollection;
				slot.SetActive(true);
				currentPacks.Enqueue(slot);
			}
		}

		chestMenu.SetActive(!curActive);
	}

	private void AddToCollection(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		DDDPlayerModel.AddToCollection(pack);
		chestPacks.Remove(pack);
		foreach(GameObject go in currentPacks)
		{
			Pack curPack = go.GetComponent<PackScript>().GetPack();
			Toggle t = go.GetComponentInChildren<Toggle>();
			if(pack == curPack && go.activeSelf && t.isOn)
			{
				go.SetActive(false);
				break;
			}
		}
	}

	private void RemoveFromCollection(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		DDDPlayerModel.RemoveFromCollection(pack);
		chestPacks.Add(pack);
		foreach (GameObject go in currentPacks)
		{
			Pack curPack = go.GetComponent<PackScript>().GetPack();
			if (pack == curPack && !go.activeSelf)
			{
				go.SetActive(true);
				break;
			}
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

	public void Resume()
	{
		ToggleActive();
		EventManager.TriggerEvent("endPause");
	}

}
