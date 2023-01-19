using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
	[SerializeField] private GameObject BGMSlider;
	[SerializeField] private GameObject SoundSlider;
	[SerializeField] private GameObject MouseSensitivitySlider;
	[SerializeField] private GameObject KeySettings;
	[SerializeField] private GameObject KeySettingTemplate;

	private float SoundVolumeTarget = 1;
	private float MouseSensitivityTarget = 200;

	// Start is called before the first frame update
	void Start()
    {
		// init BGMSlider here
		if (SoundSystem.IsActive())
		{
			BGMSlider.GetComponent<Slider>().value = SoundSystem.GetBGMAudioSource().volume;
		}
		// init SoundSlider here
		if (Saves.profile != null)
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

	public void Back()
	{
		SoundSystem.SaveToProfile();
		UI_Hotkeys.SaveToProfile();
		if (Saves.profile != null)
		{
			Saves.profile.SetFloat("Sound_volume", SoundVolumeTarget);
			Saves.profile.SetFloat("MouseSensitivity", MouseSensitivityTarget);
		}
		SceneManager.LoadScene("MainMenu");
	}

	public void ResetSettings()
	{
		// reset settings
		Debug.Log("Reset settings to default");
		UI_Hotkeys.ResetHotkeys();
		for(int i=0; i < KeySettings.transform.childCount; i++)
		{
			GameObject cur = KeySettings.transform.GetChild(i).gameObject;
			if (!cur.activeSelf) continue;
			if(cur.TryGetComponent<KeySettingScript>(out KeySettingScript kss))
			{
				kss.ReRenderText();
			}
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
