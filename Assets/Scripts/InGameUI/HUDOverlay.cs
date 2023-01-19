using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDOverlay : MonoBehaviour
{
	[SerializeField] private GameObject HudOverlay;
	[SerializeField] private float totalFillTime = 5; // time to fill/empty the slide
	private bool isPaused = false;

	private float targetHP = 0;
	private float targetShield = 0;
	private bool moveHPSlide = true;
	private bool moveShieldSlide = true;

	private void OnEnable()
	{
		EventManager.StartListening("updateHP", UpdateHP);
		EventManager.StartListening("updateShield", UpdateShield);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("updateHP", UpdateHP);
		EventManager.StopListening("updateShield", UpdateShield);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	private void Start()
	{
		HudOverlay.transform.Find("HP Bar").Find("Display").GetComponent<Image>().fillAmount = targetHP;
		HudOverlay.transform.Find("HP Bar").Find("Slide").GetComponent<Image>().fillAmount = targetHP;
		HudOverlay.transform.Find("Shield Bar").Find("Display").GetComponent<Image>().fillAmount = targetShield;
		HudOverlay.transform.Find("Shield Bar").Find("Slide").GetComponent<Image>().fillAmount = targetShield;
		UpdateHP();
		UpdateShield();
		if(Saves.current != null && Saves.current.HasKey("classID"))
		{
			Sprite classIcon = ClassSystem.classes[Saves.current.GetInt("classID")].iconSprite;
			HudOverlay.transform.Find("Class").Find("Icon").GetComponent<Image>().sprite = classIcon;
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (!isPaused)
		{
			UpdateCD();
			UpdateHPSlide();
			UpdateShieldSlide();
		}
    }

	private void UpdateHP()
	{
		if (!DDDPlayerModel.IsActive()) return;
		int hp = DDDPlayerModel.GetCurrentHP();
		int maxHP = DDDPlayerModel.GetMaxHP();
		float filled = (1.0f * hp) / maxHP;
		GameObject text = HudOverlay.transform.Find("HP Bar").Find("Text").gameObject;
		text.GetComponent<TMPro.TextMeshProUGUI>().text = hp.ToString() + " / " + maxHP.ToString();
		// switch imidate Update base on current filled level and target
		GameObject displayFront = HudOverlay.transform.Find("HP Bar").Find("Display").gameObject;
		GameObject displayBack = HudOverlay.transform.Find("HP Bar").Find("Slide").gameObject;
		if(targetHP < filled)
		{
			displayBack.GetComponent<Image>().fillAmount = filled;
			moveHPSlide = false;
		}
		else
		{
			displayFront.GetComponent<Image>().fillAmount = filled;
			moveHPSlide = true;
		}
		// set new target
		targetHP = filled;
	}

	private void UpdateHPSlide()
	{
		if (moveHPSlide)
		{
			GameObject display = HudOverlay.transform.Find("HP Bar").Find("Slide").gameObject;
			float curVal = display.GetComponent<Image>().fillAmount;
			curVal -= (1.0f / totalFillTime * Time.deltaTime);
			if(curVal <= targetHP)
			{
				curVal = targetHP;
			}
			display.GetComponent<Image>().fillAmount = curVal;
		}
		else
		{
			GameObject display = HudOverlay.transform.Find("HP Bar").Find("Display").gameObject;
			float curVal = display.GetComponent<Image>().fillAmount;
			curVal += (1.0f / totalFillTime * Time.deltaTime);
			if (curVal >= targetHP)
			{
				curVal = targetHP;
			}
			display.GetComponent<Image>().fillAmount = curVal;
		}
		
	}

	private void UpdateShield()
	{
		if (!DDDPlayerModel.IsActive()) return;
		int shield = DDDPlayerModel.GetShield();
		int maxShield = DDDPlayerModel.GetMaxShield();
		float filled = (1.0f * shield) / maxShield;
		GameObject text = HudOverlay.transform.Find("Shield Bar").Find("Text").gameObject;
		text.GetComponent<TMPro.TextMeshProUGUI>().text = shield.ToString() + " / " + maxShield.ToString();
		// switch imidate Update base on current filled level and target
		GameObject displayFront = HudOverlay.transform.Find("Shield Bar").Find("Display").gameObject;
		GameObject displayBack = HudOverlay.transform.Find("Shield Bar").Find("Slide").gameObject;
		if(targetShield <= filled)
		{
			displayBack.GetComponent<Image>().fillAmount = filled;
			moveShieldSlide = false;
		}
		else
		{
			displayFront.GetComponent<Image>().fillAmount = filled;
			moveShieldSlide = true;
		}
		targetShield = filled;
	}

	private void UpdateShieldSlide()
	{
		if (moveShieldSlide)
		{
			GameObject display = HudOverlay.transform.Find("Shield Bar").Find("Slide").gameObject;
			float curVal = display.GetComponent<Image>().fillAmount;
			curVal -= (1.0f / totalFillTime * Time.deltaTime);
			if (curVal <= targetShield)
			{
				curVal = targetShield;
			}
			display.GetComponent<Image>().fillAmount = curVal;
		}
		else
		{
			GameObject display = HudOverlay.transform.Find("Shield Bar").Find("Display").gameObject;
			float curVal = display.GetComponent<Image>().fillAmount;
			curVal += (1.0f / totalFillTime * Time.deltaTime);
			if (curVal >= targetShield)
			{
				curVal = targetShield;
			}
			display.GetComponent<Image>().fillAmount = curVal;
		}

	}

	private void UpdateCD()
	{
		if (!DDDPlayerModel.IsActive()) return;
		float cd = DDDPlayerModel.GetCD();
		float maxCD = DDDPlayerModel.GetMaxCD();
		float filled = cd / maxCD;
		GameObject display = HudOverlay.transform.Find("Cooldown").Find("Display").gameObject;
		display.GetComponent<Image>().fillAmount = filled;
	}

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}

}
