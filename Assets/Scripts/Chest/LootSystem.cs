using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootSystem
{
	public static List<LootItem> chestLoot;
	public static int maxPacks;

	public static List<Pack> GetRandomLootItem()
	{
		int totalWeight = 0;
		chestLoot.ForEach(li => totalWeight += li.weight);
		int packnum = Random.Range(0,maxPacks) + 1;
		List<Pack> loot = new();
		for (int i = 0; i < packnum;i++)
		{
			int rand = Random.Range(0, totalWeight);
			foreach (LootItem li in chestLoot)
			{
				if (rand < li.weight)
				{
					loot.Add(li.pack);
					break;
				}
				rand -= li.weight;
			}
		}
		return loot;
	}

    public class LootItem
	{
		public Pack pack;
		public int weight = 1;

		public static LootItem CreateFromJson(string jsonString)
		{
			return JsonUtility.FromJson<LootItem>(jsonString);
		}

		public static string SaveToJson(LootItem p)
		{
			return JsonUtility.ToJson(p);
		}

		public string SaveToJson()
		{
			return JsonUtility.ToJson(this);
		}
	}

}
