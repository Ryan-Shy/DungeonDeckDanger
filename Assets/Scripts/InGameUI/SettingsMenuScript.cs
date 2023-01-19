using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
	[SerializeField] private GameObject settingsMenu;
	[SerializeField] private GameObject BGMSlider;
	[SerializeField] private GameObject SoundSlider;
	[SerializeField] private GameObject MouseSensitivitySlider;
	[SerializeField] private GameObject KeySettings;
	[SerializeField] private GameObject KeySettingTemplate;

	private float SoundVolumeTarget = 1;
	private float MouseSensitivityTarget = 200;


	/* possible Settings are:
	 *		Hotkeys
	 *		Game volume
	 *		Controlls
	 *		
	 */

	private void OnEnable()
	{
		EventManager.StartListening("settingsMenu", ToggleActive);
	}

	private void OnDisable()
	{
		EventManager.StopListening("settingsMenu", ToggleActive);
	}

	private void Start()
	{
		// init BGMSlider here
		if (SoundSystem.IsActive())
		{
			BGMSlider.GetComponent<Slider>().value = SoundSystem.GetBGMAudioSource().volume;
		}
		// init SoundSlider here
		if(Saves.profile != null)
		{
			SoundVolumeTarget = Saves.profile.GetFloat("Sound_volume", 1);
			SoundSlider.GetComponent<Slider>().value = SoundVolumeTarget;
		}
		// init MouseSensitivitySlider here
		if (Saves.profile != null)
		{
			MouseSensitivityTarget = Saves.profile.GetFloat("MouseSensitivity", 200);
			MouseSensitivitySlider.GetComponent<Slider>().value = MouseSensitivityTarget;
		}
		// create a Key Setting for each key in UI_Hotkeys
		float posShift = -50;
		float contentHeight = 60;
		foreach (string key in UI_Hotkeys.GetKeys())
		{
			GameObject curKey = Instantiate(KeySettingTemplate);
			curKey.name = "KeySetting_" + key;
			curKey.transform.SetParent(KeySettings.transform, false);
			Vector2 pos = curKey.GetComponent<RectTransform>().anchoredPosition;
			pos.y = posShift;
			curKey.GetComponent<RectTransform>().anchoredPosition = pos;
			posShift -= 50;
			curKey.SetActive(true);
			curKey.GetComponent<KeySettingScript>().key = key;
			contentHeight += 50;
		}
		Vector2 contentSize = KeySettings.GetComponent<RectTransform>().sizeDelta;
		contentSize.y = contentHeight;
		KeySettings.GetComponent<RectTransform>().sizeDelta = contentSize;
		contentSize = KeySettings.transform.parent.GetComponent<RectTransform>().sizeDelta;
		float offsetY = Mathf.Abs(KeySettings.GetComponent<RectTransform>().anchoredPosition.y);
		contentSize.y = contentHeight + offsetY;
		KeySettings.transform.parent.GetComponent<RectTransform>().sizeDelta = contentSize;
	}

	private void ToggleActive()
	{
		bool curActive = settingsMenu.activeSelf;
		settingsMenu.SetActive(!curActive);
	}

	public void Save()
	{
		// TODO add other Settings
		UI_Hotkeys.SaveToProfile();
		SoundSystem.SaveToProfile();
		if (Saves.profile != null)
		{
			Saves.profile.SetFloat("Sound_volume", SoundVolumeTarget);
			Saves.profile.SetFloat("MouseSensitivity", MouseSensitivityTarget);
		}
	}

	public void Back()
	{
		Save();
		ToggleActive();
		EventManager.TriggerEvent("pauseMenu");
	}

	public void ResetSettings()
	{
		// TODO add other Settings
		UI_Hotkeys.ResetHotkeys();
		for (int i = 0; i < KeySettings.transform.childCount; i++)
		{
			GameObject cur = KeySettings.transform.GetChild(i).gameObject;
			if (!cur.activeSelf) continue;
			if (cur.TryGetComponent<KeySettingScript>(out KeySettingScript kss))
			{
				kss.ReRenderText();
			}
		}
		if(Saves.profile != null)
		{
			Saves.profile.SetFloat("MouseSensitivity",200);
		}
		// Sound settings are not reset, don't want to damage players hearing ;)
	}

	public void BGMSliderChanged()
	{
		float val = BGMSlider.GetComponent<Slider>().value;
		if (SoundSystem.IsActive())
		{
			SoundSystem.GetBGMAudioSource().volume = val;
		}
	}

	public void SoundSliderChanged()
	{
		SoundVolumeTarget = SoundSlider.GetComponent<Slider>().value;
	}

	public void MouseSensitivityChanged()
	{
		MouseSensitivityTarget = MouseSensitivitySlider.GetComponent<Slider>().value;
	}

}
