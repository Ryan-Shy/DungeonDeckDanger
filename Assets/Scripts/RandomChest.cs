using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChest : MonoBehaviour
{
    [SerializeField] private GameObject[] chestPrefabs;
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
            int chestIndex = myGenerator.generatorRandom.Next(0, chestPrefabs.Length + 1);
            if (chestIndex != chestPrefabs.Length)
            {
                GameObject goChest = Instantiate(chestPrefabs[chestIndex], transform.position,
                    Quaternion.identity, transform) as GameObject;
                goChest.name = chestPrefabs[chestIndex].name;
				// give proper ID
				goChest.GetComponentInChildren<ChestScript>().SetID(myGenerator.generatorRandom.Next().ToString());
            }
        }
    }
}
