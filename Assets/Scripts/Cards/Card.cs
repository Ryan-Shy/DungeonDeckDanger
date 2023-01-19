using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
	// Attributes form JSON
	public string name;
	public string icon;
	public string audio = "audio/empty";
	public int type;
	public List<Effect> effects = new();
	public int cardID;
	// resolved attributes
	public Sprite iconSprite { get; set; }
	public AudioClip audioClip { get; set; }
	public CardType cardType { get; set; }

	public static Card CreateFromJson(string jsonString)
	{
		Card temp = JsonUtility.FromJson<Card>(jsonString);
		temp.ResovleAttributes();
		temp.effects.ForEach(e => e.ResovleAttributes());
		return temp;
	}

	public bool IsOfType(CardType otherType)
	{
		int other = (int)otherType;
		return (type & other) == other;
	}

	public int GetTotalEnemyDamage()
	{
		int val = 0;
		foreach(Effect eff in effects)
		{
			if (eff.hitsEnemy)
			{
				val += eff.damage;
			}
		}
		return val;
	}

	public int GetTotalPlayerDamage()
	{
		int val = 0;
		foreach (Effect eff in effects)
		{
			if (eff.hitsPlayer)
			{
				val += eff.damage;
			}
		}
		return val;
	}

	public int GetTotalEnemyShield()
	{
		int val = 0;
		foreach (Effect eff in effects)
		{
			if (eff.hitsEnemy)
			{
				val += eff.block;
			}
		}
		return val;
	}

	public int GetTotalPlayerShield()
	{
		int val = 0;
		foreach (Effect eff in effects)
		{
			if (eff.hitsPlayer)
			{
				val += eff.block;
			}
		}
		return val;
	}

	public int GetTotalEnemyHeal()
	{
		int val = 0;
		foreach (Effect eff in effects)
		{
			if (eff.hitsEnemy)
			{
				val += eff.heal;
			}
		}
		return val;
	}

	public int GetTotalPlayerHeal()
	{
		int val = 0;
		foreach (Effect eff in effects)
		{
			if (eff.hitsPlayer)
			{
				val += eff.heal;
			}
		}
		return val;
	}

	public int GetNumberOfTypes()
	{
		switch (cardType)
		{
			case CardType.Attack:
			case CardType.Defence:
			case CardType.Heal:
			case CardType.Movement:
				return 1;
			case CardType.AttackDefence:
			case CardType.AttackHeal:
			case CardType.AttackMovement:
			case CardType.DefenceHeal:
			case CardType.DefenceMovement:
			case CardType.HealMovement:
				return 2;
			case CardType.AttackDefenceHeal:
			case CardType.AttackDefenceMovement:
			case CardType.AttackHealMovement:
			case CardType.DefenceHealMovement:
				return 3;
			case CardType.All:
				return 4;
		}
		return 0;
	}

	public int GetEffectCount()
	{
		int c = effects.Count;
		if (c > 3) return 3;
		return c;
	}

	public void ResovleAttributes()
	{
		iconSprite = Resources.Load<Sprite>(icon);
		cardType = (CardType) type;
		audioClip = Resources.Load<AudioClip>(audio);
	}

}
