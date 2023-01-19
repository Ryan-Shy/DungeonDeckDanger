using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndFloorScript : MonoBehaviour
{
	[SerializeField] private GameObject chestMenu;
	[SerializeField] private GameObject contentPane;
	[SerializeField] private GameObject PackTemplate;
	private bool isPaused = false;
	private Queue<GameObject> currentPacks = new();

	private void OnEnable()
	{
		EventManager.StartListening("endFloorMenu", ToggleActive);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("endFloorMenu", ToggleActive);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	// Update is called once per frame
	void Update()
	{
		if (isPaused && chestMenu.activeSelf)
		{
			if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("pause")))
			{
				Resume();
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
		else if(DDDPlayerModel.IsActive())
		{
			// turn on
			List<Pack> collection = DDDPlayerModel.GetPackCollection();
			List<Pack> deck = DDDPlayerModel.GetDeckAsPacks();
			foreach (Pack pack in collection)
			{
				if (deck.Contains(pack))
				{
					deck.Remove(pack);
				}
				else
				{
					GameObject slot = Instantiate(PackTemplate);
					slot.transform.SetParent(contentPane.transform);
					PackScript packScript = slot.GetComponent<PackScript>();
					packScript.SetID(pack.packID.ToString());
					packScript.SetPack(pack);
					packScript.addToList = SendToHub;
					packScript.removeFromList = KeepInCollection;
					slot.SetActive(true);
					currentPacks.Enqueue(slot);
				}
			}
		}
		chestMenu.SetActive(!curActive);
	}

	private void SendToHub(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		if (Saves.profile == null) return;

		AvailablePacks avpacks = new();
		if (!Saves.profile.HasKey("packs"))
		{
			avpacks.packs = new();
		}
		else
		{
			avpacks = AvailablePacks.CreateFromJson(Saves.profile.GetString("packs"));
		}
		DDDPlayerModel.RemoveFromCollection(pack);
		avpacks.packs.Add(pack);
		Saves.profile.SetString("packs", avpacks.SaveToJson());

		foreach (GameObject go in currentPacks)
		{
			Pack curPack = go.GetComponent<PackScript>().GetPack();
			Toggle t = go.GetComponentInChildren<Toggle>();
			if (pack == curPack && go.activeSelf && t.isOn)
			{
				go.SetActive(false);
				break;
			}
		}
	}

	private void KeepInCollection(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		if (Saves.profile == null) return;

		AvailablePacks avpacks = new();
		if (!Saves.profile.HasKey("packs"))
		{
			avpacks.packs = new();
		}
		else
		{
			avpacks = AvailablePacks.CreateFromJson(Saves.profile.GetString("packs"));
		}
		DDDPlayerModel.AddToCollection(pack);
		avpacks.packs.Remove(pack);
		Saves.profile.SetString("packs", avpacks.SaveToJson());

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

	public void NextFloor()
	{
		//TODO generate next floor and load respective Data
		Debug.Log("Advancing to the Next Floor");
		int floor = Saves.current.GetInt("floor");
		EventManager.TriggerEvent("save");
		Saves.current.SetInt("floor", floor + 1);
		Saves.current.Remove("playerPos");
		Saves.current.Remove("playerRot");
		Saves.current.SaveToDisk();
		EventManager.TriggerEvent("endPause");
		Saves.LoadSceneFromSave(Saves.current);
	}

}
