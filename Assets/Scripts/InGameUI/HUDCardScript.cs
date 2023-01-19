using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCardScript : MonoBehaviour
{
	[SerializeField] private Card card;
	// static references
	private static readonly CardType[] baseTypes = { CardType.Attack, CardType.Defence, CardType.Heal, CardType.Movement };
	// resolved Attributes
	private List<GameObject> backgrounds;
	private List<GameObject> effects;

	private bool ReRender = true;

	private void Awake()
	{
		// resolve GameObjects
		// order should be same as baseType and order of Backgrounds in Editor
		backgrounds = new();
		GameObject backgroundsObj = FindChild("Backgrounds");
		backgrounds.Add(FindChild(backgroundsObj, "Background Orange"));
		backgrounds.Add(FindChild(backgroundsObj, "Background Purple"));
		backgrounds.Add(FindChild(backgroundsObj, "Background Green"));
		backgrounds.Add(FindChild(backgroundsObj, "Background Blue"));
		effects = new();
		effects.Add(FindChild("Effect 1"));
		effects.Add(FindChild("Effect 2"));
		effects.Add(FindChild("Effect 3"));
	}

    // Update is called once per frame
    void Update()
    {
		if (card != null && ReRender) RenderCard();
    }

	private GameObject FindChild(string name)
	{
		return FindChild(gameObject, name);
	}

	private GameObject FindChild(GameObject parent, string name)
	{
		Transform ownTrans = parent.transform;
		Transform child = ownTrans.Find(name);
		if (child == null)
		{
			Debug.Log("Child not found: "+name);
			return parent;
		}
		return child.gameObject;
	}

	public void ChangeCard(Card c)
	{
		card = c;
		ReRender = true;
	}

	public Card GetCard()
	{
		return card;
	}

	private void RenderCard()
	{
		if (card == null) return;
		ReRender = false;
		// set background based on card type
		// change fill state correctly!
		int nTypes = card.GetNumberOfTypes();
		int curType = nTypes;
		for (int i = 0; i < backgrounds.Count; i++)
		{
			if (card.IsOfType(baseTypes[i]))
			{
				// gather values
				float fillAmount = (1.0f * curType) / nTypes;
				curType--;
				// set values
				backgrounds[i].GetComponent<Image>().fillAmount = fillAmount;
				backgrounds[i].SetActive(true);
			}
			else
			{
				backgrounds[i].SetActive(false);
			}
		}
		// set card name
		FindChild("Card Name").GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.name;
		// set icon from card
		FindChild("Icon").GetComponent<Image>().sprite = card.iconSprite;
		// set effect 1 and set gameObject of effect active
		// repeat for effect 2 and 3 if applicable
		for (int i = 0; i < effects.Count; i++)
		{
			if(i < card.GetEffectCount())
			{
				effects[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.effects[i].keyword;
				effects[i].SetActive(true);
			}
			else
			{
				effects[i].SetActive(false);
			}
		}

	}

	public void UpdateCD(float fill)
	{
		FindChild("Play CD").GetComponent<Image>().fillAmount = fill;
	}
}
