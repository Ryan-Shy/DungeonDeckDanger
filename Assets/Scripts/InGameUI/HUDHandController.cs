using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHandController : MonoBehaviour
{
	[SerializeField] private GameObject handHUD;
	private bool isPaused = false;
	private readonly Dictionary<int, string> hotkeymap = new Dictionary<int, string>(){
		{0,"card1"},
		{1,"card2"},
		{2,"card3"},
		{3,"card4"},
		{4,"card5"},
		{5,"card6"},
		{6,"card7"},
		{7,"card8"},
		{8,"card9"},
		{9,"card10"},
	};

	private void OnEnable()
	{
		EventManager.StartListening("toggleHand", ToggleActive);
		EventManager.StartListening("draw", Draw);
		EventManager.StartListening("discardHand", DiscardHand);
		EventManager.StartListening("updateHand", UpdateHand);
		EventManager.StartListening("updateHandSize", UpdateHandSize);
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
	}

	private void OnDisable()
	{
		EventManager.StopListening("toggleHand", ToggleActive);
		EventManager.StopListening("draw", Draw);
		EventManager.StopListening("discardHand", DiscardHand);
		EventManager.StopListening("updateHand", UpdateHand);
		EventManager.StopListening("updateHandSize", UpdateHandSize);
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
	}

	// Update is called once per frame
	void Update()
    {
		if (!isPaused)
		{
			if(DDDPlayerModel.GetPlayCD() == 0)
			{
				foreach (int i in hotkeymap.Keys)
				{
					if (i >= handHUD.GetComponent<HUDCardHand>().GetCount()) continue;
					if (Input.GetKeyDown(UI_Hotkeys.GetHotkey(hotkeymap[i])))
					{
						PlayCard(i);
					}
				}
			}
		}
		
	}

	private void PlayCard(int i)
	{
		Card toActive = handHUD.GetComponent<HUDCardHand>().GetCard(i);
		CardController.ActivateCard("player", toActive);
		DDDPlayerModel.Play(toActive);
		HUDCardHand cardHand = handHUD.GetComponent<HUDCardHand>();
		cardHand.RemoveCard(toActive);
		// lock play ability after activating
		float timeout = 0;
		foreach (Effect eff in toActive.effects)
		{
			float delay = 0;
			eff.hitDelays.ForEach(d => delay += d);
			if (timeout < delay)
			{
				timeout = delay;
			}
		}
		DDDPlayerModel.SetMaxPlayCD(timeout);
		DDDPlayerModel.SetPlayCD(timeout);
	}

	private void ToggleActive()
	{
		bool curActive = handHUD.activeSelf;
		handHUD.SetActive(!curActive);
	}

	private void Draw()
	{
		Card toAdd = DDDPlayerModel.Draw();
		handHUD.GetComponent<HUDCardHand>().AddCard(toAdd);
	}

	private void UpdateHand()
	{
		List<Card> curHand = DDDPlayerModel.GetCurrentHand();
		while (handHUD.GetComponent<HUDCardHand>().GetCount() > 0)
		{
			Card toDiscard = handHUD.GetComponent<HUDCardHand>().GetCard(0);
			handHUD.GetComponent<HUDCardHand>().RemoveCard(toDiscard);
		}
		foreach(Card c in curHand)
		{
			handHUD.GetComponent<HUDCardHand>().AddCard(c);
		}
	}

	private void DiscardHand()
	{
		while (handHUD.GetComponent<HUDCardHand>().GetCount() > 0)
		{
			Card toDiscard = handHUD.GetComponent<HUDCardHand>().GetCard(0);
			DDDPlayerModel.Discard(toDiscard);
			handHUD.GetComponent<HUDCardHand>().RemoveCard(toDiscard);
		}
	}

	private void UpdateHandSize()
	{
		handHUD.GetComponent<HUDCardHand>().UpdateHandSize();
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
