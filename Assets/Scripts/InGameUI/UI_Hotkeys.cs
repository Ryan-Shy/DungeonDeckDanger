using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class UI_Hotkeys
{
	private Dictionary<string, KeyCode> hotkeys;
	private Dictionary<string, string> clearNames;
	private static UI_Hotkeys instance = null;
	private static readonly object padlock = new();

	UI_Hotkeys() 
	{
		// TODO generate default hotkeys
		hotkeys = new Dictionary<string, KeyCode>();
		clearNames = new Dictionary<string, string>();
		if (Saves.profile != null)
		{
			AddFromProfile("pause");
			AddFromProfile("inventory");
			AddFromProfile("interact");
			AddFromProfile("dash");
			AddFromProfile("card1");
			AddFromProfile("card2");
			AddFromProfile("card3");
			AddFromProfile("card4");
			AddFromProfile("card5");
			AddFromProfile("card6");
			AddFromProfile("card7");
			AddFromProfile("card8");
			AddFromProfile("card9");
			AddFromProfile("card10");
		}
		AddDefaults();
	}

	private void AddDefaults()
	{
		AddDefault("Pause Key", "pause", KeyCode.Escape);
		AddDefault("Inventory Key","inventory", KeyCode.I);
		AddDefault("Interaction Key", "interact", KeyCode.F);
		AddDefault("Dash Key", "dash", KeyCode.LeftShift);
		AddDefault("Card 1", "card1", KeyCode.Alpha1);
		AddDefault("Card 2", "card2", KeyCode.Alpha2);
		AddDefault("Card 3", "card3", KeyCode.Alpha3);
		AddDefault("Card 4", "card4", KeyCode.Alpha4);
		AddDefault("Card 5", "card5", KeyCode.Alpha5);
		AddDefault("Card 6", "card6", KeyCode.Alpha6);
		AddDefault("Card 7", "card7", KeyCode.Alpha7);
		AddDefault("Card 8", "card8", KeyCode.Alpha8);
		AddDefault("Card 9", "card9", KeyCode.Alpha9);
		AddDefault("Card 10", "card10", KeyCode.Alpha0);
	}

	private void AddDefault(string clearName, string key, KeyCode defaultValue)
	{
		if (!hotkeys.ContainsKey(key)) hotkeys.Add(key, defaultValue);
		if (!clearNames.ContainsKey(key)) clearNames.Add(key, clearName);
	}

	private void AddFromProfile(string key)
	{
		string profileKey = key + "_Hotkey";
		if (Saves.profile.HasKey(profileKey))
		{
			KeyCode code = (KeyCode)Saves.profile.GetInt(profileKey);
			hotkeys[key] = code;
		}
	}

	public static void SaveToProfile()
	{
		if (Saves.profile == null) return;
		foreach (string key in Instance.hotkeys.Keys)
		{
			string profileKey = key + "_Hotkey";
			int code = (int)Instance.hotkeys[key];
			Saves.profile.SetInt(profileKey, code);
		}
		Saves.profile.SaveToDisk();
	}

	public static UI_Hotkeys Instance
	{
		get
		{
			lock (padlock)
			{
				if(instance == null)
				{
					instance = new UI_Hotkeys();
				}
				return instance;
			}
		}
	}

	public static void SetHotkey(string key, KeyCode code)
	{
		if (key.Length == 0) return;
		if (Instance.hotkeys.ContainsKey(key))
			Instance.hotkeys[key] = code;
		else
			Instance.hotkeys.Add(key, code);
	}

	public static KeyCode GetHotkey(string key)
	{
		if (!Instance.hotkeys.ContainsKey(key)) return KeyCode.None;
		return Instance.hotkeys[key];
	}

	public static void ResetHotkeys()
	{
		Instance.hotkeys.Clear();
		Instance.AddDefaults();
	}

	public static string GetClearName(string key)
	{
		if (!Instance.clearNames.ContainsKey(key)) return key;
		return Instance.clearNames[key];
	}

	public static List<string> GetKeys()
	{
		List<string> ret = new();
		foreach(string key in Instance.hotkeys.Keys)
		{
			ret.Add(key);
		}
		return ret;
	}

}
