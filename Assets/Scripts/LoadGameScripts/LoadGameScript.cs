using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScript : MonoBehaviour
{
	public static int selected = 0;
	public GameObject SaveSlotTemplate;
	public GameObject ContentPane;

	private bool validSelection = false;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log(Application.persistentDataPath);
		//list all saves
		string[] saves = Saves.GetSaveFiles();
		float posShift = 95;
		float contentHeight = 10;
		foreach (string s in saves)
		{
			Debug.Log(s);
			GameObject curSlot = Instantiate(SaveSlotTemplate);//,ContentPane.transform);
			curSlot.name = "SaveSlot_"+s;
			curSlot.transform.SetParent(ContentPane.transform, false);
			Vector3 pos = curSlot.transform.position;
			pos.y = posShift;
			curSlot.transform.position = pos;
			posShift -= 100;
			curSlot.SetActive(true);
			int slotID = Saves.GetSaveId(s);
			curSlot.GetComponent<SaveSlotUIScript>().slotNumber = slotID;
			contentHeight += 100;
			if (!validSelection)
			{
				selected = slotID;
				validSelection = true;
			}
		}
		Vector2 contentSize = ContentPane.GetComponent<RectTransform>().sizeDelta;
		contentSize.y = contentHeight;
		ContentPane.GetComponent<RectTransform>().sizeDelta = contentSize;
	}

	public void Back()
	{
		Debug.Log("Back");
		SceneManager.LoadScene("MainMenu");
	}


	public void Load()
	{
		if (!validSelection) return;
		Debug.Log("Load "+selected);
		Saves.LoadSceneFromID(selected);
	}

}
