using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextToggle : MonoBehaviour
{
	[SerializeField] private string defaultText;
	[SerializeField] private string alternativeText;

	private bool state = false;

    // Start is called before the first frame update
    void Start()
    {
		gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = defaultText;
    }

    public void OnButton()
	{
		if (state)
		{
			gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = defaultText;
		}
		else
		{
			gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = alternativeText;
		}
		state = !state;
	}
}
