using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
	[SerializeField] private List<GameObject[]> floorEnemies;
	[SerializeField] private GameObject[] enemyPrefabs;
	[SerializeField] private List<Vector3> spawnPositions = new(new Vector3[1]);
	[SerializeField] private int emptySpawns = 1;
	private DungeonGenerator myGenerator;
	private bool isCompleted;

	void Start()
	{
		myGenerator = GameObject.Find("Generator").GetComponent<DungeonGenerator>();
	}

	void Update()
	{
		if (!isCompleted && myGenerator.dungeonState == DungeonState.completed)
		{
			isCompleted = true;
			int floor = 0;
			if(Saves.current != null)
			{
				floor = Saves.current.GetInt("floor"); // returns 0 if key is not found
			}
			foreach (Vector3 pos in spawnPositions)
			{
				//TODO make multiple lists for different floors (only like max 5 lists)
				// maybe use a map (floornum)->(List<enemyPrefabs>)
				int enemyIndex = myGenerator.generatorRandom.Next(0, enemyPrefabs.Length + emptySpawns);
				if (enemyIndex < enemyPrefabs.Length)
				{
					// give proper ID
					string id = myGenerator.generatorRandom.Next().ToString();
					bool dead = false;
					bool isBoss = false;
					if (Saves.current != null && Saves.current.HasKey("enemy_" + id))
					{
						string json = Saves.current.GetString("enemy_" + id);
						DDDEnemyData data = DDDEnemyData.CreateFromJson(json);
						dead = data.isDead;
						isBoss = data.isBoss;
					}
					if (!dead)
					{
						// don't even spawn enemy if already killed previously
						GameObject goEnemy = Instantiate(enemyPrefabs[enemyIndex], transform.position + pos, Quaternion.identity, transform);
						goEnemy.name = enemyPrefabs[enemyIndex].name;
						goEnemy.GetComponentInChildren<DDDEnemyModel>().SetID(id);
					}
					else if (isBoss && EndOfFloorStatic.eofaTemplate != null)
					{
						//make sure to spawn End of Floor Area if boss is not spawned
						GameObject goEndOfFloor = Instantiate(EndOfFloorStatic.eofaTemplate, transform.position, Quaternion.identity, transform);
						goEndOfFloor.name = "EndOfFloorArea";
					}

				}
			}
		}
	}
}
