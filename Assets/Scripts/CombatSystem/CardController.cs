using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CardController : MonoBehaviour
{
	[SerializeField] private GameObject AOETemplate;
	[SerializeField] private Color playerColor;
	[SerializeField] private Color enemyColor;

	private static CardController instance = null;
	private static readonly object padlock = new();

	private Dictionary<string, GameObject> registry;
	private int aoeNameCounter;

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
				instance = this;
				registry = new();
			}
		}
	}

	private void OnDestroy()
	{
		lock (padlock)
		{
			instance = null;
		}
	}

	public static void Register(string key, GameObject obj)
	{
		// the GameObject is responsible for position and rotation
		if (instance.registry.ContainsKey(key))
		{
			Debug.Log("The Key \"" + key + "\" is already registered, new object not registered.");
			return;
		}
		instance.registry.Add(key, obj);
	}

	public static void Unregister(string key)
	{
		if (instance != null && instance.registry.ContainsKey(key))
		{
			instance.registry.Remove(key);
		}
	}

	public static void UnregisterAll()
	{
		instance.registry.Clear();
	}

	public static void ActivateCard(string key, Card card)
	{
		if (!instance.registry.ContainsKey(key))
		{
			Debug.Log("Key '" + key + "' is not registered, cannot play Card '" + card.name + "'");
			return;
		}
		GameObject obj = instance.registry[key];
		foreach (Effect eff in card.effects)
		{
			if (eff.damage > 0 || eff.block > 0 || eff.heal > 0)
			{
				// call AOEController
				instance.CreateAOE(eff, obj);
			}
			else if (eff.dash > 0)
			{
				// TODO implement dash
			}
			else
			{
				// no effect, just log it
				Debug.Log("No effect to active on Card: '" + card.name + "', effect: '" + eff.keyword + "'.");
			}
		}
		// make sure that registered game object has Audio Source, set volume based on savegame
		AudioSource audioSource = obj.GetComponent<AudioSource>();
		if (audioSource != null && Saves.profile != null)
		{
			audioSource.volume = Saves.profile.GetFloat("Sound_volume", 1);
			audioSource.PlayOneShot(card.audioClip);
		}
		else
		{
			Debug.Log("No Audio Source on registered Object: " + key);
		}
	}

	private void CreateAOE(Effect eff, GameObject obj)
	{
		// create an aoe based on this effect
		Vector3 rot = obj.transform.rotation.eulerAngles;
		rot.x = 0;
		Quaternion qrot = Quaternion.Euler(rot);
		if (eff.aoeType == AoeType.Circle) qrot = Quaternion.identity; // fix rotation of circle aoes
		Vector3 pos = obj.transform.position;

		if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, Mathf.Infinity))
		{
			pos.y -= hit.distance - 0.05f;
		}
		GameObject AOE = Instantiate(AOETemplate, pos, qrot, obj.transform);
		AOEInstance instance = AOE.GetComponent<AOEInstance>();
		instance.SetEffect(eff);
		// TODO set right material for AOEs based on Tag of obj
		// also we can change mat based on effect
		// indicator material is now set, change color parameter
		Color toSet = obj.CompareTag("Player") ? playerColor : enemyColor;
		AOE.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", toSet);
		AOE.SetActive(true);
	}
}
