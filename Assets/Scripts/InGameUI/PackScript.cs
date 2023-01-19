using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackScript : MonoBehaviour
{
	[SerializeField] private GameObject IDText;
	[SerializeField] private GameObject toggleButton;
	[SerializeField] private GameObject toggleText;
	[SerializeField] private string onText = "Remove From Deck";
	[SerializeField] private string offText = "Add To Deck";
	[SerializeField] private GameObject[] cardObjects;
	public AddToList addToList;
	public RemoveFromList removeFromList;

	private Pack pack;
	private bool ignoreDeckUpdate = false;

	public void SetPack(Pack newPack)
	{
		pack = newPack;
		int i = 0;
		foreach (GameObject cardSlot in cardObjects)
		{
			if (i < pack.cardIDs.Length)
			{
				cardSlot.GetComponent<HUDCardScript>().ChangeCard(CardSystem.cards[pack.cardIDs[i]]);
				cardSlot.SetActive(true);
				i++;
			}
			else
			{
				cardSlot.SetActive(false);
			}
		}
	}

	public Pack GetPack()
	{
		return pack;
	}

	public void SetInDeck(bool inDeck)
	{
		ignoreDeckUpdate = inDeck != toggleButton.GetComponent<Toggle>().isOn;
		toggleButton.GetComponent<Toggle>().isOn = inDeck;
	}

	public void SetID(string id)
	{
		IDText.GetComponent<TMPro.TextMeshProUGUI>().text = id;
	}

	public void ToggleText(bool on)
	{
		if (on) toggleText.GetComponent<TMPro.TextMeshProUGUI>().text = onText;
		else toggleText.GetComponent<TMPro.TextMeshProUGUI>().text = offText;
	}

	public void ToggleAddToList(bool on)
	{
		if (ignoreDeckUpdate)
		{
			ignoreDeckUpdate = false;
			return;
		}
		if (on) addToList(pack);
		else removeFromList(pack);
	}

	public delegate void AddToList(Pack pack);
	public delegate void RemoveFromList(Pack pack);

}
