using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This Script is attached to every Save Slot
 * When initializing the Prefab for Save Slot, this Script needs to be attached and configured (e.g. slotNumber)
 */

public class SaveSlotUIScript : MonoBehaviour
{

	public int slotNumber = 0;

	private SaveSystem save;
	private int floorNumber = 0;
	private System.DateTime lastSaved;

	private bool isSelected;

    // Start is called before the first frame update
    void Start()
    {
		save = Saves.GetSave(slotNumber);
		floorNumber = save.GetInt("floor");
		lastSaved = save.LastSaveTime();
		// TODO modify fields
		Transform slotID = transform.GetChild(0).GetChild(0);
		slotID.gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = slotNumber.ToString("00");
		Transform icon = transform.GetChild(0).GetChild(1);
		string w = icon.gameObject.GetComponent<Image>().sprite.name;
		int classID = save.GetInt("classID");
		if (ClassSystem.classes.TryGetValue(classID, out DDDClass newClass))
		{
			icon.GetComponent<Image>().sprite = newClass.iconSprite;
			w = icon.gameObject.GetComponent<Image>().sprite.name;
		}
		Debug.Log(w);
		Transform floorNum = transform.GetChild(0).GetChild(3);
		floorNum.gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = floorNumber.ToString();
		Transform saveDate = transform.GetChild(0).GetChild(5);
		saveDate.gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = lastSaved.ToString();
	}

	// Update is called once per frame
	void Update()
    {
		if(!isSelected && LoadGameScript.selected == slotNumber)
		{
			//change to be selected
			isSelected = true;
			Transform checkmark = transform.GetChild(0).GetChild(6);
			checkmark.gameObject.SetActive(true);
		}
		else if(isSelected && LoadGameScript.selected != slotNumber)
		{
			//change to no longer be selected
			isSelected = false;
			Transform checkmark = transform.GetChild(0).GetChild(6);
			checkmark.gameObject.SetActive(false);
		}
	}

	public void OnSelected()
	{
		Debug.Log("Selected slot " + slotNumber);
		LoadGameScript.selected = slotNumber;
	}
}
