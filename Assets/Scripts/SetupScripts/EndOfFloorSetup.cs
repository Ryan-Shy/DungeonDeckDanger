using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfFloorSetup : MonoBehaviour
{
	public GameObject endOfFloorAreaTemplate;
	private static bool initialized = false;

	private void Awake()
	{
		if (initialized) return;
		EndOfFloorStatic.eofaTemplate = endOfFloorAreaTemplate;
		initialized = true;
	}
}
