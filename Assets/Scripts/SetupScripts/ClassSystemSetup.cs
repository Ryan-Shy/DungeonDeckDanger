using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSystemSetup : MonoBehaviour
{
	public TextAsset[] classJsons;
	public TextAsset[] playerDataJsons;
	private static bool initialized = false;

	void Awake()
	{
		if (initialized) return;
		ClassSystem.classes = new Dictionary<int, DDDClass>();
		foreach (TextAsset json in classJsons)
		{
			string jsonString = json.ToString();
			DDDClass c = DDDClass.CreateFromJson(jsonString);
			ClassSystem.classes[c.classID] = c;
		}
		ClassSystem.defaultDataJsons = new Dictionary<int, string>();
		foreach(TextAsset json in playerDataJsons)
		{
			string jsonString = json.ToString();
			DDDPlayerData data = DDDPlayerData.CreateFromJson(jsonString);
			ClassSystem.defaultDataJsons[data.classID] = jsonString;
		}
		initialized = true;
	}
}
