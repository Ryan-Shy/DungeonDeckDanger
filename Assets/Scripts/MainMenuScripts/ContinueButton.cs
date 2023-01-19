using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		// hide the continue button if no save exists
		bool hasSaves = Saves.current != null;
		transform.gameObject.SetActive(hasSaves);
	}


}
