using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEInstance : MonoBehaviour
{
	[SerializeField] private Material[] IndicatorMaterials;

	private Effect effect;
	private float timeAlive;
	private int hitsDone;
	private bool indicatorSet;

	public List<GameObject> targets;

	private void Awake()
	{
		targets = new();
	}

	// Update is called once per frame
	void Update()
    {
		if (hitsDone >= effect.hitDelays.Count)
		{
			Destroy(gameObject);
			return;
		}
		timeAlive += Time.deltaTime;
		if (indicatorSet)
		{
			transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Phase", timeAlive);
		}
		TryHit();
		
    }

	private void TryHit()
	{
		float nextHitTime = 0;
		for(int i = 0; i <= hitsDone; i++)
		{
			nextHitTime += effect.hitDelays[i];
		}
		if(nextHitTime < timeAlive)
		{
			DoHit();
			hitsDone++;
		}
	}

	private void DoHit()
	{
		if (transform.parent.CompareTag("Enemy") && !targets.Contains(transform.parent.gameObject)) // fix bug where enemies can't heal or block
		{
			targets.Add(transform.parent.gameObject);
		}
		foreach (GameObject target in targets)
		{
			if (target == null) continue;
			if(target.CompareTag("Player") && effect.hitsPlayer)
			{
				DDDPlayerModel.DealDamage(effect.damage);
				DDDPlayerModel.ModifyHP(-effect.heal);
				DDDPlayerModel.ModifyShield(effect.block);
			}
			else if(target.CompareTag("Enemy") && effect.hitsEnemy) 
			{
				target.GetComponent<DDDEnemyModel>().DealDamage(effect.damage);
				target.GetComponent<DDDEnemyModel>().ModifyHP(effect.heal);
				target.GetComponent<DDDEnemyModel>().ModifyShield(effect.block);
			}
		}
	}

	public void SetEffect(Effect eff)
	{
		effect = eff;
		// setup
		timeAlive = 0;
		hitsDone = 0;
		targets = new();
		// create trigger
		Mesh mesh = AOEMeshBuilder.BuildMesh(effect.aoeType);
		GetComponent<MeshCollider>().sharedMesh = mesh;
		// set scale
		SetScale();
		// create aoe indicator
		float totalTime = 0;
		foreach (float f in effect.hitDelays)
		{
			totalTime += f;
		}
		if (totalTime <= 0) return;
		int type = effect.aoe_type;
		if (type < 0 || type > IndicatorMaterials.Length) return;
		transform.GetChild(0).GetComponent<MeshRenderer>().material = IndicatorMaterials[type];
		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Expand_Time", totalTime);
		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Reset_Time", totalTime * 1.1f);
		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_Phase", timeAlive);
		indicatorSet = true;
	}

	private void SetScale()
	{
		Vector3 scale = transform.localScale;
		switch (effect.aoeType)
		{
			case AoeType.Circle:
				scale.x = effect.size;
				scale.z = effect.size;
				transform.localScale = scale;
				// range moves position forward
				Vector3 pos = transform.localPosition;
				pos += transform.forward * effect.range;
				transform.localPosition = pos;
				break;
			case AoeType.Line:
			case AoeType.HalveCircleFront:
			case AoeType.QuarterCircleFront:
			default:
				scale.x = effect.size;
				scale.z = effect.range;
				transform.localScale = scale;
				break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;
		if (targets.Contains(obj)) return;
		targets.Add(obj);
	}

	private void OnTriggerExit(Collider other)
	{
		GameObject obj = other.gameObject;
		if (!targets.Contains(obj)) return;
		targets.Remove(obj);
	}

	private void OnDestroy()
	{
		targets.Clear();
	}
}
