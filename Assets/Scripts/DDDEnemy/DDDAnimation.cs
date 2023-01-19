using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDAnimation : MonoBehaviour
{
	private Animator myAnimator;
	private DDDEnemyModel model;

	private void Awake()
    {
		myAnimator = GetComponentInChildren<Animator>();
		model = GetComponent<DDDEnemyModel>();
		if (model != null) model.anim = this;
	}

    // Update is called once per frame
    void Update()
    {
		UpdateWalkAnimations();
	}

	private void UpdateWalkAnimations()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
		float xSpeed = Mathf.Abs(localVelocity.x);
		float zSpeed = Mathf.Abs(localVelocity.z);
		if (zSpeed > xSpeed)
		{
			myAnimator.SetFloat("MovementSpeed", zSpeed+1);
			if (localVelocity.z > 1)
				myAnimator.SetInteger("Direction", 1);
			else if(localVelocity.z < -1)
				myAnimator.SetInteger("Direction", -1);
			else
				myAnimator.SetInteger("Direction", 0);
		}
		else
		{
			myAnimator.SetFloat("MovementSpeed", xSpeed+1);
			if (localVelocity.x > 1)
				myAnimator.SetInteger("Direction", 2);
			else if (localVelocity.x < -1)
				myAnimator.SetInteger("Direction", -2);
			else
				myAnimator.SetInteger("Direction", 0);
		}
	}

	public void TriggerAttack(float duration)
	{
		myAnimator.SetFloat("AttackSpeed", 1/duration);
		int i = Random.Range(0,2);
		if(i == 0)
		{
			myAnimator.SetTrigger("Attack1");
		}
		else
		{
			myAnimator.SetTrigger("Attack2");
		}
	}

	public void TriggerDefence(float duration)
	{
		myAnimator.SetFloat("DefenseSpeed", 1 / duration);
		myAnimator.SetTrigger("Defence");
	}

	public void TriggerGetHit()
	{
		myAnimator.SetTrigger("GetHit");
	}

	public void SetDead()
	{
		myAnimator.SetBool("isDead", true);
	}

	public void ToggleInCombat(bool inCombat)
	{
		myAnimator.SetBool("InCombat", inCombat);
	}
}
