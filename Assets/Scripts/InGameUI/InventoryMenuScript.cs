using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// This script should be attached to the Inventory Manager
/// It takes care of opening/closing the Inventory menu
/// as well as all the buttons within the InventoryMenu
/// </summary>
public class InventoryMenuScript : MonoBehaviour
{
	[SerializeField] private GameObject inventoryMenu;
	[SerializeField] private GameObject contentPane;
	[SerializeField] private GameObject PackTemplate;
	private bool isPaused = false;
	private Queue<GameObject> currentPacks = new();

	private void OnEnable()
	{
		EventManager.StartListening("inventoryMenu", ToggleActive);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("inventoryMenu", ToggleActive);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("inventory")))
		{
			if (isPaused && inventoryMenu.activeSelf)
			{
				EventManager.TriggerEvent("endPause"); // tell other components to resume
				EventManager.TriggerEvent("inventoryMenu"); // close inventory menu
			}
			else if (!isPaused)
			{
				EventManager.TriggerEvent("startPause"); // tell other components to pause
				EventManager.TriggerEvent("inventoryMenu"); // open inventory menu
			}
		}
		if (Input.GetKeyDown(UI_Hotkeys.GetHotkey("pause")) && isPaused && inventoryMenu.activeSelf)
		{
			EventManager.TriggerEvent("endPause"); // tell other components to resume
			EventManager.TriggerEvent("inventoryMenu"); // close inventory menu
		}
	}

	private void ToggleActive()
	{
		bool curActive = inventoryMenu.activeSelf;
		// create view of packs / remove when toggling off
		if (DDDPlayerModel.IsActive())
		{
			if (curActive)
			{
				// turn off
				while(currentPacks.TryDequeue(out GameObject slot))
				{
					Destroy(slot);
				}
			}
			else
			{
				// turn on
				List<Pack> packs = DDDPlayerModel.GetPackCollection();
				Dictionary<int, int> packCountsInDeck = new();
				foreach (Pack p in DDDPlayerModel.GetDeckAsPacks())
				{
					if (packCountsInDeck.ContainsKey(p.packID))
					{
						int count = packCountsInDeck[p.packID];
						packCountsInDeck[p.packID] = count + 1;
					}
					else
					{
						packCountsInDeck.Add(p.packID, 1);
					}
				}
				foreach (Pack pack in packs)
				{
					GameObject slot = Instantiate(PackTemplate);
					slot.transform.SetParent(contentPane.transform);
					PackScript packScript = slot.GetComponent<PackScript>();
					packScript.SetID(pack.packID.ToString());
					packScript.SetPack(pack);
					packScript.addToList = AddToDeck;
					packScript.removeFromList = RemoveFromDeck;
					if (packCountsInDeck.ContainsKey(pack.packID))
					{
						int count = packCountsInDeck[pack.packID];
						if (count > 1)
						{
							packCountsInDeck[pack.packID] = count - 1;
						}
						else
						{
							packCountsInDeck.Remove(pack.packID);
						}
						packScript.SetInDeck(true);
					}
					else
					{
						packScript.SetInDeck(false);
					}
					slot.SetActive(true);
					currentPacks.Enqueue(slot);
				}
			}
		}

		inventoryMenu.SetActive(!curActive);
	}

	private void AddToDeck(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		DDDPlayerModel.AddToDeck(pack);
	}

	private void RemoveFromDeck(Pack pack)
	{
		if (!DDDPlayerModel.IsActive()) return;
		DDDPlayerModel.RemoveFromDeck(pack);
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
