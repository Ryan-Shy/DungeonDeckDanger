using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DDDEnemyController
{
	private static Dictionary<string, GameObject> enemies = new();

	public static void Register(string id, GameObject enemy)
	{
		if (enemies.ContainsKey(id))
		{
			Debug.LogWarning("Enemy with id " + id + " already registered. Did not register new Model");
			return;
		}
		enemies.Add(id, enemy);
	}

	public static void Unregister(string id)
	{
		if (enemies.ContainsKey(id))
		{
			enemies.Remove(id);
		}
	}
}
