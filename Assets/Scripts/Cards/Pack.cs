using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pack
{
	// Attributes form JSON
	public bool custom;
	public int packID;
	public int[] cardIDs;

	public static Pack CreateFromJson(string jsonString)
	{
		return JsonUtility.FromJson<Pack>(jsonString);
	}

	public static string SaveToJson(Pack p)
	{
		return JsonUtility.ToJson(p);
	}

	public string SaveToJson()
	{
		return JsonUtility.ToJson(this);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Pack)) return false;
		Pack other = (Pack)obj;
		if (other.cardIDs.Length != cardIDs.Length) return false;
		for (int i = 0; i < cardIDs.Length; i++)
		{
			if (cardIDs[i] != other.cardIDs[i]) return false;
		}
		return custom == other.custom && packID == other.packID;
	}

	public override int GetHashCode()
	{
		int hash = 0;
		foreach (int i in cardIDs)
		{
			hash *= 151;
			hash += (i+19) * 13;
		}
		if (custom) hash += 701;
		hash += packID * 31;
		return hash;
	}
}
