using UnityEngine;
using System.Collections;
using System.IO;

public class SaveSystemSetup : MonoBehaviour {

	[SerializeField] private string profileFileName = "Profile.bin";
	[SerializeField] private string currentFileName = "save00.bin";
	[SerializeField] private bool dontDestroyOnLoad; // the object will move from one scene to another (you only need to add it once)
	private static bool initialized = false;

	void Awake()
	{
		if (initialized) return;
		// create dummy
		Saves.dummy = createDummy();

		// init Saves.profile
		initProfile();

		// check existing files
		string[] existingSaves = Saves.GetSaveFiles();
		foreach (string save in existingSaves)
		{
			string[] splitsave = save.Replace("\\", "/").Split('/');
			string val = splitsave[splitsave.Length - 1].Substring(4, 2);
			int value;
			if (!int.TryParse(val, out value)) value = 0;
			Saves.slotUsed[value] = true;
		}

		// set the save used for continue
		if(Saves.HasSaves())
		{
			// get latest saveFile
			System.DateTime latest = File.GetLastWriteTime(existingSaves[0]);
			int latestIdx = 0;
			for (int i = 1; i < existingSaves.Length; i++)
			{
				System.DateTime cur = File.GetLastWriteTime(existingSaves[i]);
				latest.CompareTo(cur);
				if (latest.CompareTo(cur) < 0)
				{
					latest = cur;
					latestIdx = i;
				}
			}
			string[] splitsave = existingSaves[latestIdx].Replace("\\", "/").Split('/');
			currentFileName = splitsave[splitsave.Length - 1];
			Saves.current = new SaveSystem(currentFileName);
		}
		initialized = true;
		// keep Object alive
		if (dontDestroyOnLoad) DontDestroyOnLoad(transform.gameObject);
	}

	private SaveSystem createDummy()
	{
		// create dummy
		// TODO create all default entries
		SaveSystem dummy = new SaveSystem("dummy.bin");
		dummy.SetString("dummy", "dummy");
		dummy.SetInt("classID", 0);
		dummy.SetInt("floor", 0);
		dummy.SaveToDisk();
		return dummy;
	}

	private void initProfile()
	{
		if (Saves.profile != null) return;
		// check if a Profile already exists
		string saveFolder = Application.persistentDataPath;
		bool hasProfile = File.Exists(saveFolder + "/" + profileFileName);
		// load profile, if not existent create new
		Saves.profile = new SaveSystem(profileFileName);
		if (!hasProfile)
		{
			// TODO create all default entries, also set default gold to 0
			Saves.profile.SetBool("hasProfile", true);
			Saves.profile.SetInt("gold", 0);
			Saves.profile.SaveToDisk();
		}
		Debug.Log("profile has keys:");
		foreach(string key in Saves.profile.GetKeys())
		{
			Debug.Log(key + " => " + Saves.profile.GetString(key));
		}
		Debug.Log("end of keys");
	}

    // if the object is present in all game scenes, auto save before exiting
    // on some platforms there may not be an exit function, see the Unity help
    void OnApplicationQuit()
	{
		Saves.profile.SaveToDisk();
		if(Saves.current != null) Saves.current.SaveToDisk();
		File.Delete(Application.persistentDataPath + "/dummy.bin");
	}
}
