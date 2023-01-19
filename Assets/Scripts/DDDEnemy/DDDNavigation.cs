using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDDNavigation : MonoBehaviour
{
	[SerializeField] private float movementForce = 50;

	private Rigidbody rb;
	private DDDEnemyModel model;

	// variables we set from model
	public float targetDistance = 5;
	public float maxSpeed = 3;

	// variables we get from model
	public bool seesPlayer = false;
	public Vector3 playerPos;
	public Vector3 ownPos;
	public Vector3 navTarget;
	private void Awake()
    {
		rb = GetComponent<Rigidbody>();
		model = GetComponent<DDDEnemyModel>();
		if (model != null) model.nav = this;
	}

    // Update is called once per frame
    void Update()
    {
		if (model != null && !model.isPaused && model.loaded && !model.data.isDead)
		{
			UpdateSeesPlayer();
			UpdateEnemyPos();
		}
	}

	private void UpdateEnemyPos()
	{
		if (!seesPlayer) return;
		// move towards player
		Vector3 dist = playerPos - ownPos;
		navTarget = playerPos - dist.normalized * targetDistance;
		Vector3 targetdist = navTarget - ownPos;
		targetdist.y = 0;
		if (targetdist.magnitude > 1 && model.aiState == DDDEnemyModel.AIState.Moving && !model.isStaggered)
		{
			rb.AddForce(targetdist.normalized * movementForce, ForceMode.Acceleration);
			//Debug.Log("Applied force to Enemy: " + targetdist.normalized * movementForce);
		}
		if (rb.velocity.magnitude > maxSpeed)
		{
			// clamp max speed
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
		//turn player instant
		Vector3 targetpos = new(playerPos.x, playerPos.y - 1.0f, playerPos.z);
		//Debug.DrawLine(transform.position, targetpos, Color.green, 2);
		transform.LookAt(targetpos);


	}

	private void UpdateSeesPlayer()
	{
		bool oldSees = seesPlayer;
		seesPlayer = SeesPlayer();
		if (seesPlayer != oldSees)
		{
			if (seesPlayer)
			{
				Debug.Log("enemy now sees Player");
			}
			else
			{
				Debug.Log("enemy no longer sees Player");
			}
		}
	}

	private bool SeesPlayer()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		playerPos = player.transform.position;
		ownPos = transform.position + transform.up;
		Vector3 distances = playerPos - ownPos;
		if (Physics.Raycast(ownPos, distances, out RaycastHit hit, float.PositiveInfinity))
		{
			if (hit.transform.gameObject.CompareTag("Player"))
			{
				return true;
			}
			else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Player"))
			{
				return true;
			}
			else
			{
				//Debug.Log("enemy hit raycast on Object: " + hit.transform.gameObject.name);
			}
		}
		else
		{
			//Debug.Log("No Object hit on raycast from enemy");
		}
		return false;
	}

}
