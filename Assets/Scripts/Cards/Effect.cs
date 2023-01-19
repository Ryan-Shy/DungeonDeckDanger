using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect
{
	// Attributes form JSON
	// base parameters
	public int damage; // can never be negative.
	public int block; // can also be used to reduce shield without dealing dmg when negativ
	public int heal; // can also be used to deal piercing dmg when negativ
	public float dash; // amount of time the player can sprint? Not sure how to implement this
	// targets
	public bool hitsPlayer;
	public bool hitsEnemy;
	/// <summary>
	/// time between hits in seconds, 
	/// if no values, no hits are done, 
	/// first value is delay between activation and hit
	/// </summary>
	public List<float> hitDelays;
	// aoe parameters
	// aoes get created on damage, block and heal
	public float range;
	public float size;
	public int aoe_type;
	// visualisation
	public string keyword;
	public string description;

	// resolved attributes
	public AoeType aoeType { get; set; }

	public static Effect CreateFromJson(string jsonString)
	{
		Effect temp = JsonUtility.FromJson<Effect>(jsonString);
		temp.ResovleAttributes();
		return temp;
	}

	public void ResovleAttributes()
	{
		aoeType = (AoeType)aoe_type;
	}
}
