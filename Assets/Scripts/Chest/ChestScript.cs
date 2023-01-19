using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
	private Animator myAnimator;
	private bool isInZone;
	private List<Pack> contents;
	public string id;
	private bool isPaused = false;

	private void OnEnable()
	{
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
		EventManager.StartListening("save", Save);
	}

	private void OnDisable()
	{
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
		EventManager.StopListening("save", Save);
	}

	// Start is called before the first frame update
	void Start()
    {
		myAnimator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
		if (!isPaused && isInZone && Input.GetKeyDown(UI_Hotkeys.GetHotkey("interact")))
		{
			// this will only be available to open the chest
			myAnimator.SetBool("isOpen", true);
			// trigger menu
			ChestMenuScript.chestPacks = contents;
			EventManager.TriggerEvent("startPause");
			EventManager.TriggerEvent("chestMenu");
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
		if (myAnimator.GetBool("isOpen"))
		{
			myAnimator.SetBool("isOpen", false);
		}
	}

	private void Save()
	{
		if (Saves.current == null) return;
		AvailablePacks toSave = new();
		toSave.packs = new(contents);
		int floor = Saves.current.GetInt("floor");
		Saves.current.SetString("floor" + floor + "_chest" + id, toSave.SaveToJson());
	}

	public void SetID(string id)
	{
		this.id = id;
		if(Saves.current != null)
		{
			int floor = Saves.current.GetInt("floor");
			if (Saves.current.HasKey("floor" + floor + "_chest" + id))
			{
				string json = Saves.current.GetString("floor" + floor + "_chest" + id);
				AvailablePacks toLoad = AvailablePacks.CreateFromJson(json);
				contents = toLoad.packs;
			}
		}
		if(contents == null)
		{
			if (LootSystem.chestLoot == null) contents = new();
			else contents = LootSystem.GetRandomLootItem();
		}
	}

}
