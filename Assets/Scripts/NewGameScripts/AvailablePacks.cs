using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvailablePacks
{
	public List<Pack> packs;

	public static AvailablePacks CreateFromJson(string jsonString)
	{
		return JsonUtility.FromJson<AvailablePacks>(jsonString);
	}

	public static string SaveToJson(AvailablePacks p)
	{
		return JsonUtility.ToJson(p);
	}

	public string SaveToJson()
	{
		return JsonUtility.ToJson(this);
	}
}
