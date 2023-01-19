using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCardHand : MonoBehaviour
{
	[SerializeField] private GameObject CardTemplate;
	[SerializeField] private float cardWidth = 210;
	[SerializeField] private int maxCards = 7;

	private List<Card> handCards = new();
	private List<GameObject> handObjects = new();
	private int nameIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
		UpdateHandSize();
	}

	private void Update()
	{
		if (!DDDPlayerModel.IsActive()) return;
		float playCD = DDDPlayerModel.GetPlayCD();
		float maxCD = DDDPlayerModel.GetMaxPlayCD();
		if (maxCD <= 0) maxCD = 1;
		if (playCD < 0) playCD = 0;
		float fill = playCD / maxCD;
		if (fill > 1) fill = 1;
		foreach(GameObject cardObject in handObjects)
		{
			cardObject.GetComponent<HUDCardScript>().UpdateCD(fill);
		}
	}

	public int GetCount()
	{
		return handCards.Count;
	}

	public bool AddCard(Card c)
	{
		// add card to list and instantiate Card Template
		if (handCards.Count >= maxCards) return false;
		handCards.Add(c);
		GameObject curCard = Instantiate(CardTemplate);
		curCard.name = "Card_" + nameIndex.ToString();
		nameIndex++;
		curCard.transform.SetParent(gameObject.transform, false);
		curCard.SetActive(true);
		curCard.GetComponent<HUDCardScript>().ChangeCard(c);
		handObjects.Add(curCard);
		RenderHand();
		return true;
	}

	public void RemoveCard(Card c)
	{
		int i = handCards.IndexOf(c);
		RemoveCard(i);
	}

	public void RemoveCard(int index)
	{
		handCards.RemoveAt(index);
		GameObject obj = handObjects[index];
		handObjects.RemoveAt(index);
		Destroy(obj);
		RenderHand();
	}

	public Card GetCard(int index)
	{
		return handCards[index];
	}

	public void UpdateHandSize()
	{
		if (DDDPlayerModel.IsActive())
		{
			maxCards = DDDPlayerModel.GetMaxHand();
		}
	}

	public void RenderHand()
	{
		// position card objects and call RenderCard
		for(int i = 0; i < handObjects.Count; i++)
		{
			GameObject cur = handObjects[i];
			Vector2 pos = cur.GetComponent<RectTransform>().anchoredPosition;
			float xpos = cardWidth * (i - (handObjects.Count - 1) / 2.0f);
			pos.x = xpos;
			cur.GetComponent<RectTransform>().anchoredPosition = pos;
			// update Card Slot Indicator
			Transform cardIndicator = cur.transform.Find("CardIndicator");
			if(cardIndicator != null)
			{
				cardIndicator.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = (i+1).ToString();
			}
		}
	}
}
