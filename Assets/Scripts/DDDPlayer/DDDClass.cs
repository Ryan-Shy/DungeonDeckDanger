using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DDDClass
{
	// Attributes form JSON
	public string name;
	public string icon;
	public string description;
	public int classID;
	// resolved attributes
	public Sprite iconSprite { get; set; }

	public static DDDClass CreateFromJson(string jsonString)
	{
		DDDClass temp = JsonUtility.FromJson<DDDClass>(jsonString);
		temp.ResovleAttributes();
		return temp;
	}

	public void ResovleAttributes()
	{
		Debug.Log("Loading icon for class" + classID.ToString());
		iconSprite = Resources.Load<Sprite>(icon);
	}
}
