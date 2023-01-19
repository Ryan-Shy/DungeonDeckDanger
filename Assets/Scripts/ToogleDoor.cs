using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToogleDoor : MonoBehaviour
{
    private Animator myAnimator;
    private bool isInZone;
	public string id;
	private bool isPaused = false;

	private void OnEnable()
	{
		EventManager.StartListening("startPause", OnStartPause);
		EventManager.StartListening("endPause", OnEndPause);
		EventManager.StartListening("save", Save);
	}

	private void OnDisable()
	{
		EventManager.StopListening("startPause", OnStartPause);
		EventManager.StopListening("endPause", OnEndPause);
		EventManager.StopListening("save", Save);
	}

	// Start is called before the first frame update
	void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused && isInZone && Input.GetKeyDown(UI_Hotkeys.GetHotkey("interact")))
        {
            bool isOpen = myAnimator.GetBool("isOpen");
            myAnimator.SetBool("isOpen", !isOpen);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInZone = true;
			EventManager.TriggerEvent("showDoorKey");
		}
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInZone = false;
			EventManager.TriggerEvent("hideDoorKey");
		}
    }

	private void OnStartPause()
	{
		isPaused = true;
	}

	private void OnEndPause()
	{
		isPaused = false;
	}

	private void Save()
	{
		if (Saves.current == null) return;
		int floor = Saves.current.GetInt("floor");
		Saves.current.SetBool("floor" + floor + "_door" + id, myAnimator.GetBool("isOpen"));
	}

	public void SetID(string id)
	{
		this.id = id;
		myAnimator = GetComponent<Animator>();
		if (Saves.current != null)
		{
			int floor = Saves.current.GetInt("floor");
			if (Saves.current.HasKey("floor" + floor + "_door" + id))
			{
				bool state = Saves.current.GetBool("floor" + floor + "_door" + id);
				myAnimator.SetBool("isOpen", state);
			}
		}
	}

}
