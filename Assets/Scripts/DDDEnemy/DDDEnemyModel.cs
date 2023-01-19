using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDEnemyModel : MonoBehaviour
{
	[SerializeField] TextAsset jsonData;

	public DDDEnemyData data;
	public AIState aiState = AIState.OnCooldown;

	private const string playertag = "Player";

	public enum AIState
	{
		OnCooldown,
		CardSelection,
		Moving,
		AttackStart,
		Attacking,
		Reset
	}

	public DDDEnemyHUD hud;
	public DDDNavigation nav;
	public DDDAnimation anim;

	public bool isPaused = false;
	public bool loaded = false;
	public bool isStaggered = false;
	public float staggerCD = 0;

	private void Awake()
	{
		if (jsonData == null) data = new();
		else data = DDDEnemyData.CreateFromJson(jsonData.ToString());
	}

	private void OnEnable()
	{
		EventManager.StartListening("save", Save);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("save", Save);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	private void OnDestroy()
	{
		DDDEnemyController.Unregister(data.id);
		CardController.Unregister(data.id);
		if (data.isBoss && data.isDead && EndOfFloorStatic.eofaTemplate != null)
		{
			// spawn end of floor area
			GameObject goEndOfFloor = Instantiate(EndOfFloorStatic.eofaTemplate, transform.parent.position, Quaternion.identity, transform.parent);
			goEndOfFloor.name = "EndOfFloorArea";
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (!isPaused && loaded && !data.isDead)
		{
			UpdateCD();
			// TODO other updates
			UpdateAIState();
			UpdateCombatAI();
		}
	}

	private void UpdateAIState()
	{
		AIState nextState = aiState;
		switch (aiState)
		{
			case AIState.OnCooldown:
				if (data.currentAICD <= 0)
				{
					nextState = AIState.CardSelection;
				}
				break;
			case AIState.CardSelection:
				nextState = AIState.Moving;
				break;
			case AIState.Moving:
				if(nav == null)
				{
					nextState = AIState.AttackStart;
				}
				else
				{
					Vector3 distance = nav.navTarget - nav.ownPos;
					distance.y = 0;
					if (distance.magnitude <= 1)
					{
						nextState = AIState.AttackStart;
					}
				}
				break;
			case AIState.AttackStart:
				nextState = AIState.Attacking;
				break;
			case AIState.Attacking:
				if (data.currentAICD <= 0)
				{
					nextState = AIState.Reset;
				}
				break;
			case AIState.Reset:
				nextState = AIState.OnCooldown;
				data.currentAICD = data.maxAICD;
				break;
			default:
				break;
		}
		aiState = nextState;
	}

	private void UpdateCombatAI()
	{
		if (nav == null || !nav.seesPlayer) return;

		if(aiState == AIState.CardSelection)
		{
			// select the target for moving
			List<int> cardIDs = GetUsableCards(0, true);
			if (cardIDs.Count == 0) return;
			int rand = Random.Range(0, cardIDs.Count);
			int cardID = cardIDs[rand];
			Card target = CardSystem.cards[cardID];
			float targetDistance = -1;
			foreach (Effect effect in target.effects)
			{
				if (effect.hitsPlayer)
				{
					switch (effect.aoeType)
					{
						case AoeType.Circle:
							targetDistance = effect.range;
							break;
						case AoeType.Line:
						case AoeType.HalveCircleFront:
						case AoeType.QuarterCircleFront:
						default:
							targetDistance = effect.range / 2;
							break;
					}
					break;
				}
			}
			if (targetDistance <= 0)
			{
				Vector3 targetPos = nav.playerPos - nav.ownPos;
				targetPos.y = 0;
				targetDistance = targetPos.magnitude;
			}
			if (targetDistance < 1)
			{
				targetDistance = 1;
			}
			nav.targetDistance = targetDistance;
		}

		if (aiState == AIState.AttackStart)
		{
			Vector3 distances = nav.playerPos - nav.ownPos;
			Vector2 xzDistances = new(distances.x, distances.z);
			float range = xzDistances.magnitude;
			// select actual card
			List<int> cardIDs = GetUsableCards(range, false);

			if (cardIDs.Count == 0) return;

			int rand = Random.Range(0, cardIDs.Count);
			int cardID = cardIDs[rand];
			Card toActivate = CardSystem.cards[cardID];
			CardController.ActivateCard(data.id, toActivate);
			if (hud != null) hud.ActivateCard(toActivate);
			Discard(toActivate);
			// set cooldown
			float delay = 0;
			foreach(Effect eff in toActivate.effects)
			{
				float cur = 0;
				eff.hitDelays.ForEach(d => cur += d);
				if (cur > delay) delay = cur;
			}
			if (delay <= 0) delay = 0.1f;
			data.currentAICD = delay;
			// start animation
			if(anim != null)
			{
				if (toActivate.IsOfType(CardType.Defence) || toActivate.IsOfType(CardType.Heal))
				{
					anim.TriggerDefence(delay);
				}
				else
				{
					anim.TriggerAttack(delay);
				}
			}
		}
		
	}

	private List<int> GetUsableCards(float range, bool anyRange)
	{
		List<int> cardIDs = new();
		foreach (int id in data.currentHand)
		{
			Card card = CardSystem.cards[id];
			bool added = false;
			if (card.IsOfType(CardType.Attack))
			{
				foreach (Effect effect in card.effects)
				{
					switch (effect.aoeType)
					{
						case AoeType.Circle:
							if ((effect.range + effect.size / 2 > range
								&& effect.range - effect.size / 2 < range) 
								|| anyRange)
							{
								cardIDs.Add(id);
								added = true;
							}
							break;
						case AoeType.Line:
						case AoeType.HalveCircleFront:
						case AoeType.QuarterCircleFront:
						default:
							if (effect.range > range || anyRange)
							{
								cardIDs.Add(id);
								added = true;
							}
							break;
					}
					if (added)
					{
						break;
					}
				}
			}
			else if (card.IsOfType(CardType.Heal))
			{
				int heal = card.GetTotalEnemyHeal();
				if (data.currentHP + heal < data.maxHP)
				{
					cardIDs.Add(id);
				}
			}
			else if (card.IsOfType(CardType.Defence))
			{
				int shield = card.GetTotalEnemyShield();
				if (data.currentShield + shield < data.maxShield)
				{
					cardIDs.Add(id);
				}
			}

		}
		return cardIDs;
	}

	private void UpdateCD()
	{
		data.currentCD -= Time.deltaTime;
		if (data.currentCD < 0)
		{
			int handCount = data.currentHand.Count;
			if ((handCount < data.maxHand) && ((data.currentDeck.Count + data.discardPile.Count) > 0))
			{
				Draw();
				data.currentCD = data.maxCD;
			}
			else
			{
				data.currentCD = 0;
			}
		}
		data.currentAICD -= Time.deltaTime;
		if(data.currentAICD < 0)
		{
			data.currentAICD = 0;
		}
		staggerCD -= Time.deltaTime;
		if(staggerCD <= 0)
		{
			isStaggered = false;
			staggerCD = 0;
		}
	}

	private void Draw()
	{
		if (data.currentDeck.Count == 0)
		{
			Shuffle();
		}
		int rand = Random.Range(0, data.currentDeck.Count);
		int cardID = data.currentDeck[rand];
		//Card drawnCard = CardSystem.cards[cardID];
		data.currentDeck.RemoveAt(rand);
		data.currentHand.Add(cardID);
	}

	private void Shuffle()
	{
		data.currentDeck.AddRange(data.discardPile);
		data.discardPile.Clear();
	}

	private void Discard(Card toDiscard)
	{
		data.currentHand.Remove(toDiscard.cardID);
		data.discardPile.Add(toDiscard.cardID);
	}
	
	private void Save()
	{
		if (Saves.current == null) return;
		string json = data.SaveToJson();
		Saves.current.SetString("enemy_"+data.id, json);
		Saves.current.SetVector3("enemy_" + data.id + "_pos", transform.position);
		Saves.current.SetVector3("enemy_" + data.id + "_rot", transform.rotation.eulerAngles);
	}

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}

	public void DealDamage(int dmg)
	{
		if (data.isDead) return;
		if (dmg <= 0) return;
		if (data.currentShield > dmg)
		{
			data.currentShield -= dmg;
		}
		else
		{
			int remaining = dmg - data.currentShield;
			data.currentShield = 0;
			data.currentHP -= remaining;
			if (data.currentHP <= 0)
			{
				data.currentHP = 0;
				Die();
				return;
			}
		}
		if(anim != null)
		{
			anim.TriggerGetHit();
		}
		isStaggered = true;
		staggerCD = 1.0f;
	}

	public void ModifyHP(int increase)
	{
		if (data.isDead) return;
		data.currentHP += increase;
		if (data.currentHP > data.maxHP)
		{
			data.currentHP = data.maxHP;
		}
		else if (data.currentHP <= 0)
		{
			data.currentHP = 0;
			Die();
			return;
		}
	}

	public void Die()
	{
		DDDEnemyController.Unregister(data.id);
		CardController.Unregister(data.id);
		data.isDead = true;
		Save();
		
		if(anim != null)
		{
			anim.SetDead();
		}
		if(Saves.profile != null)
		{
			Saves.profile.SetInt("gold", Saves.profile.GetInt("gold") + 1);
		}
		StartCoroutine(DestroyInCorutine());
	}

	private IEnumerator DestroyInCorutine()
	{
		yield return new WaitForSeconds(1);
		Destroy(gameObject, 1.0f);
	}

	public void ModifyShield(int increase)
	{
		if (data.isDead) return;
		data.currentShield += increase;
		if (data.currentShield >= data.maxShield)
		{
			data.currentShield = data.maxShield;
		}
		else if (data.currentShield <= 0)
		{
			data.currentShield = 0;
		}
	}

	public void SetID(string id)
	{
		//look if a save already exists
		if (Saves.current != null)
		{
			if(Saves.current.HasKey("enemy_" + id))
			{
				string json = Saves.current.GetString("enemy_" + id);
				data = DDDEnemyData.CreateFromJson(json);
			}
			if (Saves.current.HasKey("enemy_" + id + "_pos"))
			{
				transform.position = Saves.current.GetVector3("enemy_" + data.id + "_pos");
			}
			if (Saves.current.HasKey("enemy_" + id + "_rot"))
			{
				Vector3 rot = Saves.current.GetVector3("enemy_" + id + "_rot");
				transform.rotation = Quaternion.Euler(rot);
			}
		}
		if (data.isDead)
		{
			// need to check if enemy is already dead, don't respawn enemies otherwise
			Destroy(gameObject);
			return;
		}
		if (data.id == null || data.id == "")
		{
			data.id = id;
		}

		DDDEnemyController.Register(data.id, gameObject);
		CardController.Register(data.id, gameObject);
		loaded = true;
	}

}
