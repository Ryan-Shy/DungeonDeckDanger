using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewGameScript : MonoBehaviour
{
	// class selection
	public GameObject classSelection;
	public int classID = 0;
	public GameObject descriptionText;
	public GameObject iconSprite;
	// floor selection
	public GameObject floorSelection;
	public int floor = 0;
	public GameObject floorNumber;
	public GameObject costNumber;
	public GameObject goldNumber;
	public int gold;
	// pack selection
	public GameObject packSelection;
	public GameObject packTemplate;
	private List<Pack> availablePacks;
	private List<Pack> startingPacks;

	private bool state = false;

    // Start is called before the first frame update
    void Start()
    {
		classSelection.SetActive(!state);
		floorSelection.SetActive(!state);
		packSelection.SetActive(state);
		//
		gold = Saves.profile.GetInt("gold");
		goldNumber.GetComponent<TMPro.TextMeshProUGUI>().text = gold.ToString("0");
		UpdateClass();
		GeneratePacks();
	}

	public void NextClass()
	{
		int nextID = classID;
		foreach(int key in ClassSystem.classes.Keys)
		{
			if (key > classID && (key < nextID || nextID == classID))
			{
				nextID = key;
			}
		}
		classID = nextID;
		UpdateClass();
		Debug.Log("value changed to " + classID);
	}

	public void PreviousClass()
	{
		int nextID = classID;
		foreach (int key in ClassSystem.classes.Keys)
		{
			if (key < classID && (key > nextID || nextID == classID))
			{
				nextID = key;
			}
		}
		classID = nextID;
		UpdateClass();
		Debug.Log("value changed to " + classID);
	}

	private void UpdateClass()
	{
		if (ClassSystem.classes.TryGetValue(classID, out DDDClass newClass))
		{
			descriptionText.GetComponent<TMPro.TextMeshProUGUI>().text = newClass.description;
			iconSprite.GetComponent<Image>().sprite = newClass.iconSprite;
		}
	}

	public void IncFloor()
	{
		if (gold >= GetCostForFloor(floor+1))
		{
			floor++;
		}
		UpdateFloorNumber();
		UpdateCostNumber(GetCostForFloor(floor));
	}

	public void DecFloor()
	{
		if (floor <= 0) floor = 0;
		else floor--;
		UpdateFloorNumber();
		UpdateCostNumber(GetCostForFloor(floor));
	}

	public void Launch()
	{
		int slotID = Saves.GetNextSlot();
		Debug.Log("Start new game with slot " + slotID);
		SaveSystem curSave = Saves.CreateSave(slotID);
		curSave.SetInt("floor", floor);
		curSave.SetInt("classID", classID);
		curSave.SetString("playerData", GeneratePlayerData().SaveToJson());
		curSave.SetInt("genSeed", (int)System.DateTime.Now.Ticks);
		Saves.profile.SetInt("gold", gold - GetCostForFloor(floor));
		Saves.profile.SaveToDisk();
		Saves.LoadSceneFromSave(curSave);
	}

	private DDDPlayerData GeneratePlayerData()
	{
		DDDPlayerData data = new();
		if (ClassSystem.defaultDataJsons.TryGetValue(classID, out string dataJson))
		{
			data = DDDPlayerData.CreateFromJson(dataJson);
			return AddStartingPacks(data);
		}
		Debug.LogWarning("No data found for class "+classID.ToString());
		// backup code
		Pack defaultPack = Pack.CreateFromJson("{" +
				"\"custom\": false," +
				"\"packID\": 0," +
				"\"cardIDs\": [0,1,0,1]" +
			"}");
		data.packs.Add(defaultPack);
		data.deck.Add(defaultPack);
		data.currentDeck.AddRange(defaultPack.cardIDs);
		return AddStartingPacks(data);
	}

	private DDDPlayerData AddStartingPacks(DDDPlayerData data)
	{
		data.packs.AddRange(startingPacks);
		data.deck.AddRange(startingPacks);
		foreach (Pack p in startingPacks)
		{
			data.currentDeck.AddRange(p.cardIDs);
			// also remove from available Packs
			availablePacks.Remove(p);
		}
		if (Saves.profile == null) return data;
		AvailablePacks avp = new();
		avp.packs = availablePacks;
		Saves.profile.SetString("packs", avp.SaveToJson());
		return data;
	}

	public void Back()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void SelectionSwitch()
	{
		state = !state;
		classSelection.SetActive(!state);
		floorSelection.SetActive(!state);
		packSelection.SetActive(state);
	}

	private void GeneratePacks()
	{
		startingPacks = new();
		availablePacks = new();
		if (Saves.profile == null) return;
		if (!Saves.profile.HasKey("packs")) 
		{
			return;
		}
		string jsonString = Saves.profile.GetString("packs");
		availablePacks = JsonUtility.FromJson<AvailablePacks>(jsonString).packs;
		
		foreach (Pack pack in availablePacks)
		{
			Debug.Log("Generating Pack with id " + pack.packID);
			GameObject slot = Instantiate(packTemplate);
			slot.transform.SetParent(packSelection.transform);
			PackScript packScript = slot.GetComponent<PackScript>();
			packScript.SetID(pack.packID.ToString());
			packScript.SetPack(pack);
			packScript.addToList = AddToList;
			packScript.removeFromList = RemoveFromList;
			slot.SetActive(true);
		}
	}

	private void AddToList(Pack pack)
	{
		Debug.Log("Add pack with id " + pack.packID + " to List");
		startingPacks.Add(pack);
	}

	private void RemoveFromList(Pack pack)
	{
		Debug.Log("Remove pack with id " + pack.packID + " from List");
		startingPacks.Remove(pack);
	}

	private void UpdateFloorNumber()
	{
		floorNumber.GetComponent<TMPro.TextMeshProUGUI>().text = floor.ToString("00");
	}

	private void UpdateCostNumber(int newCost)
	{
		costNumber.GetComponent<TMPro.TextMeshProUGUI>().text = newCost.ToString("0");
	}

	private int GetCostForFloor(int floor)
	{
		return floor * floor;
	}

}
