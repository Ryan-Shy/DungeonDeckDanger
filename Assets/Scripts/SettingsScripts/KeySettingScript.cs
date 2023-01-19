using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeySettingScript : MonoBehaviour
{
	public string key = "";

	private GameObject ButtonText;
	private GameObject KeyText;

	private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
		KeyText = gameObject.transform.Find("Key Text").gameObject;
		ButtonText = gameObject.transform.Find("Activate Keychange").GetChild(0).gameObject;
		KeyText.GetComponent<TMPro.TextMeshProUGUI>().text = UI_Hotkeys.GetClearName(key);
		KeyCode code = UI_Hotkeys.GetHotkey(key);
		ButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = code.ToString();
    }

	public void ToggleSelected()
	{
		if (isSelected)
		{
			KeyCode code = UI_Hotkeys.GetHotkey(key);
			ButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = code.ToString();
			ButtonText.GetComponentInParent<Button>().interactable = true;
		}
		else
		{
			ButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "...";
			ButtonText.GetComponentInParent<Button>().interactable = false;
		}
		isSelected = !isSelected;
	}

    // Update is called once per frame
    void Update()
    {
		if (isSelected)
		{
			// change key setting
			if (Input.anyKeyDown)
			{
				foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
				{
					if (Input.GetKeyDown(code))
					{
						UI_Hotkeys.SetHotkey(key, code);
						ToggleSelected();
						break;
					}
				}
			}
			
		}
    }

	public void ReRenderText()
	{
		KeyCode code = UI_Hotkeys.GetHotkey(key);
		ButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = code.ToString();
		ButtonText.GetComponentInParent<Button>().interactable = true;
	}

}
