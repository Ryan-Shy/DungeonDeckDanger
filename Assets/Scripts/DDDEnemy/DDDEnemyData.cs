using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDEnemyData
{
	// Attributes form JSON
	public string id;
	public int currentHP = 10;
	public int maxHP = 10;
	public int currentShield = 0;
	public int maxShield = 10;
	public float currentCD = 15;
	public float maxCD = 15;
	public float currentAICD = 10;
	public float maxAICD = 10;
	public int maxHand = 3;
	public bool isDead = false;
	public bool isBoss = false;
	public List<int> currentDeck = new();
	public List<int> discardPile = new();
	public List<int> currentHand = new();
	// resolved attributes

	public static DDDEnemyData CreateFromJson(string jsonString)
	{
		Debug.Log("Loading Enemy Data from string:\n" + jsonString);
		DDDEnemyData temp = JsonUtility.FromJson<DDDEnemyData>(jsonString);
		temp.ResovleAttributes();
		return temp;
	}

	private void ResovleAttributes()
	{
		
	}

	public static string SaveToJson(DDDEnemyData p)
	{
		return JsonUtility.ToJson(p);
	}

	public string SaveToJson()
	{
		return JsonUtility.ToJson(this);
	}

}
