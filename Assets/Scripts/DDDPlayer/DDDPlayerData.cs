using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDPlayerData
{
	// Attributes form JSON
	public int currentHP = 50;
	public int maxHP = 50;
	public int currentShield = 0;
	public int maxShield = 50;
	public float currentCD = 3;
	public float maxCD = 3;
	public float playCD = 0;
	public float maxPlayCD = 1;
	public int maxHand = 7;
	public int classID = 0;
	public List<Pack> packs = new();
	public List<Pack> deck = new();
	public List<int> currentDeck = new();
	public List<int> discardPile = new();
	public List<int> currentHand = new();
	// resolved attributes
	public DDDClass dddClass;

	public static DDDPlayerData CreateFromJson(string jsonString)
	{
		Debug.Log("Loading Player Data from string:\n"+jsonString);
		DDDPlayerData temp = JsonUtility.FromJson<DDDPlayerData>(jsonString);
		temp.ResovleAttributes();
		return temp;
	}

	private void ResovleAttributes()
	{
		if (!ClassSystem.classes.TryGetValue(classID, out dddClass))
		{
			if (ClassSystem.classes.Count == 0)
			{
				Crash("No classes loaded, Quit Application");
				return;
			}
			foreach (DDDClass emergencyClass in ClassSystem.classes.Values)
			{
				dddClass = emergencyClass;
				break;
			}
		}
	}

	private void Crash(string msg)
	{
		Debug.LogError(msg);
		Application.Quit();
	}

	public static string SaveToJson(DDDPlayerData p)
	{
		return JsonUtility.ToJson(p);
	}

	public string SaveToJson()
	{
		return JsonUtility.ToJson(this);
	}

}
