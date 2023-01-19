using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystemSetup : MonoBehaviour
{
	public TextAsset[] lootJsons;
	public int maxPacks = 3;
	private static bool initialized = false;

	void Awake()
	{
		if (initialized) return;
		LootSystem.chestLoot = new List<LootSystem.LootItem>();
		LootSystem.maxPacks = maxPacks;
		int count = 0;
		foreach (TextAsset json in lootJsons)
		{
			string jsonString = json.ToString();
			LootSystem.LootItem li = LootSystem.LootItem.CreateFromJson(jsonString);
			LootSystem.chestLoot.Add(li);
			count++;
		}
		Debug.Log("Loaded "+count+" Loot Items");
		initialized = true;
	}
}
