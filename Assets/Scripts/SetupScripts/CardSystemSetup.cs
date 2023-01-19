using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemSetup : MonoBehaviour
{
	public TextAsset[] cardJsons;
	private static bool initialized = false;

	void Awake()
	{
		if (initialized) return;
		CardSystem.cards = new Dictionary<int, Card>();
		foreach (TextAsset json in cardJsons)
		{
			string jsonString = json.ToString();
			Card c = Card.CreateFromJson(jsonString);
			CardSystem.cards[c.cardID] = c;
		}
		initialized = true;
	}
}
