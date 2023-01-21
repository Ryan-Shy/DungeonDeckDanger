using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
	public bool start = false;
	public int target;

	// Update is called once per frame
    void Update()
    {
		Animator anim = GetComponent<Animator>();
		if (start) 
		{
			anim.SetInteger("anim", target);
			anim.SetTrigger("start");
			start = false;
		}
    }
}
