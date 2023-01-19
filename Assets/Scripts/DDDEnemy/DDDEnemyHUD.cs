using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDEnemyHUD : MonoBehaviour
{
	[SerializeField] private float totalFillTime = 5; // time to fill/empty the slide

	private DDDEnemyModel model;
	private Material mat;

	private float sliderPosHP;
	private float targetHP;
	private bool increaseHPSlide;
	private float sliderPosShield;
	private float targetShield;
	private bool increaseShieldSlide;

	private void Awake()
	{
		model = transform.parent.GetComponent<DDDEnemyModel>();
		model.hud = this;
		mat = gameObject.GetComponent<Renderer>().material;
	}

	private void Start()
	{
		targetHP = (1.0f * model.data.currentHP) / model.data.maxHP;
		sliderPosHP = targetHP;
		targetShield = (1.0f * model.data.currentShield) / model.data.maxShield;
		sliderPosShield = targetShield;

		mat.SetFloat("_HP_Bar", targetHP);
		mat.SetFloat("_HP_Slide", targetHP);
		mat.SetFloat("_Shield_Bar", targetShield);
		mat.SetFloat("_Shield_Slide", targetShield);
	}

	private void Update()
    {
		//if (!model.data.isDead)
		if(model != null)
		{
			UpdateHP();
			UpdateShield();
			UpdateHPSlide();
			UpdateShieldSlide();
			UpdateRotation();
		}
    }

	private void UpdateHP()
	{
		float hpPos = (1.0f * model.data.currentHP) / model.data.maxHP;
		if (targetHP < hpPos)
		{
			mat.SetFloat("_HP_Slide", hpPos);
			increaseHPSlide = true;
		}
		else if (targetHP > hpPos)
		{
			mat.SetFloat("_HP_Bar", hpPos);
			increaseHPSlide = false;
		}
		targetHP = hpPos;
	}

	private void UpdateHPSlide()
	{
		if (increaseHPSlide)
		{
			sliderPosHP += (1.0f / totalFillTime * Time.deltaTime);
			if(sliderPosHP >= targetHP)
			{
				sliderPosHP = targetHP;
			}
			mat.SetFloat("_HP_Bar", sliderPosHP);
		}
		else
		{
			sliderPosHP -= (1.0f / totalFillTime * Time.deltaTime);
			if (sliderPosHP <= targetHP)
			{
				sliderPosHP = targetHP;
			}
			mat.SetFloat("_HP_Slide", sliderPosHP);
		}
	}

	private void UpdateShield()
	{
		float shieldPos = (1.0f * model.data.currentShield) / model.data.maxShield;
		if (targetShield < shieldPos)
		{
			mat.SetFloat("_Shield_Slide", shieldPos);
			increaseShieldSlide = true;
		}
		else if (targetShield > shieldPos)
		{
			mat.SetFloat("_Shield_Bar", shieldPos);
			increaseShieldSlide = false;
		}
		targetShield = shieldPos;
	}

	private void UpdateShieldSlide()
	{
		if (increaseShieldSlide)
		{
			sliderPosShield += (1.0f / totalFillTime * Time.deltaTime);
			if (sliderPosShield >= targetShield)
			{
				sliderPosShield = targetShield;
			}
			mat.SetFloat("_Shield_Bar", sliderPosShield);
		}
		else
		{
			sliderPosShield -= (1.0f / totalFillTime * Time.deltaTime);
			if (sliderPosShield <= targetShield)
			{
				sliderPosShield = targetShield;
			}
			mat.SetFloat("_Shield_Slide", sliderPosShield);
		}
	}

	private void UpdateRotation()
	{
		// rotate Quad to face the player at all times
		//Vector3 lookPoint = transform.position - GameObject.FindWithTag("Player").transform.position;
		//lookPoint.y = GameObject.FindWithTag("Player").transform.position.y;
		//transform.LookAt(lookPoint);
		transform.LookAt(GameObject.FindWithTag("Player").transform);
		transform.Rotate(0, 180, 0);
	}

	public void ActivateCard(Card card)
	{
		// TODO if we want to show the player, which card is activated, we can do that here.
	}

}
