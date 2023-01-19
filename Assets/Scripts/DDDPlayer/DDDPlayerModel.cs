using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDPlayerModel : MonoBehaviour
{
	private static DDDPlayerModel instance = null;
	private static readonly object padlock = new();

	private DDDPlayerData playerData;

	private bool isPaused = false;
	private bool isDead = false;

	void Awake()
	{
		// simple way to enforce Singleton
		lock (padlock)
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				Debug.Log("instance of DDDPlayerModel loading");
				instance = this;
				LoadFromSave();
			}
		}
	}

	private void OnEnable()
	{
		EventManager.StartListening("save", Save);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
		LoadRotation();
	}

	private void OnDisable()
	{
		EventManager.StopListening("save", Save);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	private void Start()
	{
		EventManager.TriggerEvent("updateHand");
		CardController.Register("player", gameObject);
	}

	private void Update()
	{
		if (!isPaused)
		{
			if (!GameObject.FindWithTag("Player").activeInHierarchy)
			{
				// if updates could occure during generation, generation becomes unstable
				return;
			}

			UpdateCD();
			UpdatePlayCD();
			// TODO other updates
		}
	}

	private void OnDestroy()
	{
		lock (padlock)
		{
			instance = null;
		}
	}

	private void LoadFromSave()
	{
		if (instance == null) return;
		if (Saves.current == null)
		{
			Crash("No save file found, Quit Application");
			return;
		}
		Debug.Log("Loading Playerdata from " + Saves.current.GetName());
		string datastring = Saves.current.GetString("playerData");
		if(datastring.Length == 0)
		{
			// no string set, use default
			playerData = new DDDPlayerData();
			Pack defaultPack = Pack.CreateFromJson("{" +
				"\"custom\": false," +
				"\"packID\": 0," +
				"\"cardIDs\": [0,1,0,1,0,1]" +
			"}");
			playerData.packs.Add(defaultPack);
			playerData.deck.Add(defaultPack);
			foreach (int id in defaultPack.cardIDs)
			{
				playerData.currentDeck.Add(id);
			}
		}
		else
		{
			playerData = DDDPlayerData.CreateFromJson(datastring);
		}
		// load player position
		if (Saves.current.HasKey("playerPos"))
		{
			GameObject goPlayer = GameObject.FindWithTag("Player");
			goPlayer.transform.position = Saves.current.GetVector3("playerPos");
		}
		
	}

	private void LoadRotation()
	{
		if (Saves.current != null && Saves.current.HasKey("playerRot"))
		{
			GameObject goPlayer = GameObject.FindWithTag("Player");
			Vector3 rot = Saves.current.GetVector3("playerRot");
			goPlayer.transform.rotation = Quaternion.Euler(rot);
		}
	}

	private void Crash(string msg)
	{
		Debug.LogError(msg);
		Application.Quit();
	}

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}

	private void UpdateCD()
	{
		playerData.currentCD -= Time.deltaTime;
		if(playerData.currentCD < 0)
		{
			int handCount = playerData.currentHand.Count;
			if ((handCount < playerData.maxHand) && ((playerData.currentDeck.Count + playerData.discardPile.Count) > 0))
			{
				EventManager.TriggerEvent("draw");
				playerData.currentCD = playerData.maxCD;
			}
			else
			{
				playerData.currentCD = 0;
			}
		}
		
	}

	private void UpdatePlayCD()
	{
		playerData.playCD -= Time.deltaTime;
		if (playerData.playCD < 0)
		{
			playerData.playCD = 0;
		}
	}

	private void Shuffle()
	{
		playerData.currentDeck.AddRange(playerData.discardPile);
		playerData.discardPile.Clear();
		// shuffle
		int n = playerData.currentDeck.Count;
		while(n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			int val = playerData.currentDeck[k];
			playerData.currentDeck[k] = playerData.currentDeck[n];
			playerData.currentDeck[n] = val;
		}
	}

	private void Die()
	{
		if (!isDead)
		{
			Debug.Log("player died");
			EventManager.TriggerEvent("die");
			EventManager.TriggerEvent("startPause");
			EventManager.TriggerEvent("gameOverMenu");
			isDead = true;
		}
	}

	// public static Methods

	public static bool IsActive()
	{
		return instance != null;
	}

	public static void Save()
	{
		if (instance == null) return;
		if (Saves.current == null) return;
		string json = DDDPlayerData.SaveToJson(instance.playerData);
		Saves.current.SetString("playerData", json);
		GameObject goPlayer = GameObject.FindWithTag("Player");
		Saves.current.SetVector3("playerPos", goPlayer.transform.position);
		Saves.current.SetVector3("playerRot", goPlayer.transform.rotation.eulerAngles);
	}

	// getters and setters
	public static int GetCurrentHP()
	{
		return instance.playerData.currentHP;
	}

	public static void ModifyHP(int decrease)
	{
		instance.playerData.currentHP -= decrease;
		if (instance.playerData.currentHP > instance.playerData.maxHP)
		{
			instance.playerData.currentHP = instance.playerData.maxHP;
		}
		else if (instance.playerData.currentHP <= 0)
		{
			instance.playerData.currentHP = 0;
			EventManager.TriggerEvent("updateHP");
			instance.Die();
			return;
		}
		EventManager.TriggerEvent("updateHP");
	}

	public static void DealDamage(int dmg)
	{
		if (dmg <= 0) return;
		if(instance.playerData.currentShield > dmg)
		{
			instance.playerData.currentShield -= dmg;
		}
		else
		{
			int remaining = dmg - instance.playerData.currentShield;
			instance.playerData.currentShield = 0;
			instance.playerData.currentHP -= remaining;
			if(instance.playerData.currentHP <= 0)
			{
				instance.playerData.currentHP = 0;
				EventManager.TriggerEvent("updateHP");
				EventManager.TriggerEvent("updateShield");
				instance.Die();
				return;
			}
		}
		EventManager.TriggerEvent("updateHP");
		EventManager.TriggerEvent("updateShield");
	}

	public static int GetMaxHP()
	{
		return instance.playerData.maxHP;
	}

	public static void ModifyMaxHP(int increase)
	{
		instance.playerData.maxHP += increase;
		if(instance.playerData.maxHP <= 0)
		{
			// can't have 0 or less max hp, set to 1 instead
			instance.playerData.maxHP = 1;
		}
		EventManager.TriggerEvent("updateHP");
	}

	public static int GetShield()
	{
		return instance.playerData.currentShield;
	}

	public static void ModifyShield(int increase)
	{
		instance.playerData.currentShield += increase;
		if(instance.playerData.currentShield >= instance.playerData.maxShield)
		{
			instance.playerData.currentShield = instance.playerData.maxShield;
		}
		else if(instance.playerData.currentShield <= 0)
		{
			instance.playerData.currentShield = 0;
		}
		EventManager.TriggerEvent("updateShield");
	}

	public static int GetMaxShield()
	{
		return instance.playerData.maxShield;
	}

	public static void ModifyMaxShield(int increase)
	{
		instance.playerData.maxShield += increase;
		if (instance.playerData.maxShield < 0)
		{
			// can't have less than 0 max Shield, set to 0 instead
			instance.playerData.maxShield = 0;
		}
		EventManager.TriggerEvent("updateShield");
	}

	public static float GetCD()
	{
		return instance.playerData.currentCD;
	}

	public static void ModifyCD(float decrease)
	{
		instance.playerData.currentCD -= decrease;
		if(instance.playerData.currentCD > instance.playerData.maxCD)
		{
			instance.playerData.currentCD = instance.playerData.maxCD;
		}
		else if(instance.playerData.currentCD <= 0)
		{
			instance.playerData.currentCD = 0;
		}
		// no call to event system as update is done every frame anyway
	}

	public static float GetMaxCD()
	{
		return instance.playerData.maxCD;
	}

	public static void ModifyMaxCD(float increase)
	{
		instance.playerData.maxCD += increase;
		if (instance.playerData.maxCD < 0)
		{
			// can't have less than 0 max CD, set to 0 instead
			instance.playerData.maxCD = 0;
		}
		EventManager.TriggerEvent("updateShield");
	}

	public static float GetPlayCD()
	{
		return instance.playerData.playCD;
	}

	public static void SetPlayCD(float cd)
	{
		if (cd < 0) cd = 0;
		instance.playerData.playCD = cd;
	}

	public static float GetMaxPlayCD()
	{
		return instance.playerData.maxPlayCD;
	}

	public static void SetMaxPlayCD(float maxCD)
	{
		if (maxCD <= 0) maxCD = 1;
		instance.playerData.maxPlayCD = maxCD;
	}

	public static int GetMaxHand()
	{
		return instance.playerData.maxHand;
	}

	public static void ModifyHandSize(int newSize)
	{
		if (newSize > 10 || newSize < 1) return;
		instance.playerData.maxHand = newSize;
		EventManager.TriggerEvent("updateHandSize");
	}

	public static DDDClass GetClass()
	{
		return instance.playerData.dddClass;
	}

	// no modification of class allowed

	public static void AddToCollection(Pack pack)
	{
		instance.playerData.packs.Add(pack);
	}

	public static void RemoveFromCollection(Pack pack)
	{
		instance.playerData.packs.Remove(pack);
	}

	public static Pack GetPackFromCollection(int packID)
	{
		return instance.playerData.packs.Find(p => p.packID == packID);
	}

	public static List<Pack> GetPackCollection()
	{
		return new(instance.playerData.packs);
	}

	public static List<Card> GetCardCollection()
	{
		List<Card> cards = new();
		foreach (Pack p in GetPackCollection())
		{
			foreach (int i in p.cardIDs)
			{
				cards.Add(CardSystem.cards.GetValueOrDefault(i));
			}
		}
		return cards;
	}

	public static List<Pack> GetDeckAsPacks()
	{
		return new(instance.playerData.deck);
	}

	public static List<Card> GetDeckAsCards()
	{
		List<Card> cards = new();
		foreach (Pack p in GetDeckAsPacks())
		{
			foreach (int i in p.cardIDs)
			{
				cards.Add(CardSystem.cards.GetValueOrDefault(i));
			}
		}
		return cards;
	}

	public static int GetTotalDeckSize()
	{
		return GetDeckAsCards().Count;
	}

	public static void AddToDeck(Pack pack)
	{
		// Deck modification discards all cards
		EventManager.TriggerEvent("discardHand");
		instance.playerData.deck.Add(pack);
		instance.playerData.currentDeck = new();
		instance.playerData.discardPile = new();
		instance.playerData.currentHand = new();
		GetDeckAsCards().ForEach(c => instance.playerData.discardPile.Add(c.cardID));
	}

	public static void RemoveFromDeck(Pack pack)
	{
		// Deck modification discards all cards
		EventManager.TriggerEvent("discardHand");
		instance.playerData.deck.Remove(pack);
		instance.playerData.currentDeck = new();
		instance.playerData.discardPile = new();
		instance.playerData.currentHand = new();
		GetDeckAsCards().ForEach(c => instance.playerData.discardPile.Add(c.cardID));
	}

	public static List<Card> GetCurrentDeck()
	{
		List<Card> cards = new();
		instance.playerData.currentDeck.ForEach(i => cards.Add(CardSystem.cards[i]));
		return cards;
	}

	public static List<Card> GetDiscardPile()
	{
		List<Card> cards = new();
		instance.playerData.discardPile.ForEach(i => cards.Add(CardSystem.cards[i]));
		return cards;
	}

	public static List<Card> GetCurrentHand()
	{
		List<Card> cards = new();
		instance.playerData.currentHand.ForEach(i => cards.Add(CardSystem.cards[i]));
		return cards;
	}

	// Actions

	public static Card Draw()
	{
		if(instance.playerData.currentDeck.Count == 0)
		{
			instance.Shuffle();
		}
		int rand = Random.Range(0, instance.playerData.currentDeck.Count);
		int cardID = instance.playerData.currentDeck[rand];
		Card drawnCard = CardSystem.cards[cardID];
		instance.playerData.currentDeck.RemoveAt(rand);
		instance.playerData.currentHand.Add(cardID);
		return drawnCard;
	}

	public static void Play(Card toActivate)
	{
		Discard(toActivate);
	}

	public static void Discard(Card toDiscard)
	{
		instance.playerData.currentHand.Remove(toDiscard.cardID);
		instance.playerData.discardPile.Add(toDiscard.cardID);
	}
}
