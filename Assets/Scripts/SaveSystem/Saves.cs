using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Saves
{
	public const int MAX_SAVES = 100;

	public static SaveSystem profile;
	public static SaveSystem current;
	public static SaveSystem dummy;

	public static bool[] slotUsed = new bool[MAX_SAVES];
	/// <summary>
	/// Gathers information about existing Saves and returns the lowest unused slotID
	/// </summary>
	/// <returns>smallest unused slotID or MAX_SAVES if no slot is unused</returns>
	public static int GetNextSlot()
	{
		int ret = 0;
		foreach(bool b in slotUsed)
		{
			if (b) ret++;
			else return ret;
		}
		return ret;
	}
	/// <summary>
	/// Gathers all correctly named save files and returns their path
	/// </summary>
	/// <returns>The path to all correctly named save files</returns>
	public static string[] GetSaveFiles()
	{
		//load available save files here
		string saveFolder = Application.persistentDataPath;
		return Directory.GetFiles(saveFolder, "save??.bin");
	}
	/// <summary>
	/// Returns the SaveSystem corresponding to the slotID
	/// A SaveSystem is stored as file in the form of "save"+slotID+".bin" where slotID is a 2 digit number
	/// If no Save File with the requested slotID exists, a new one is created
	/// To guarante a new Save File, use "createSave"
	/// </summary>
	/// <param name="slotID">The slotID of the SaveSystem, modulo is used to convert to correct range</param>
	/// <returns>The SaveSystem corresponding to the slotID</returns>
	public static SaveSystem GetSave(int slotID)
	{
		slotID = (slotID % slotUsed.Length + slotUsed.Length) % slotUsed.Length;
		string name = "save" + slotID.ToString("00") + ".bin";
		if (!slotUsed[slotID])
		{
			return CreateSave(slotID);
		}

		string[] saves = GetSaveFiles();
		foreach (string save in saves)
		{
			string[] splitsave = save.Replace("\\", "/").Split('/');
			string val = splitsave[splitsave.Length - 1].Substring(4, 2);
			int value;
			if (!int.TryParse(val, out value)) value = -1;
			if (value == slotID) return new SaveSystem(name);
		}
		// save not found
		return dummy.Copy(name);
	}
	
	/// <summary>
	/// extracts the slotID from the name or path of a save file
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static int GetSaveId(string name)
	{
		string[] splitsave = name.Replace("\\", "/").Split('/');
		string val = splitsave[splitsave.Length - 1].Substring(4, 2);
		int value;
		if (!int.TryParse(val, out value)) value = -1;
		return value;
	}

	/// <summary>
	/// checks if any save files exist
	/// returns true if there is at least 1 correctly named save file
	/// </summary>
	/// <returns>true if at least 1 correctly named save file exists, false otherwise</returns>
	public static bool HasSaves()
	{
		string[] savefiles = GetSaveFiles();
		return savefiles.Length > 0;
	}

	/// <summary>
	/// Creates a new save with the given slotID
	/// If a save already exists on that slot, it gets deleted
	/// </summary>
	/// <param name="slotID">The slot ID to use, modulo is used to convert to correct range</param>
	/// <returns>A new SaveSystem</returns>
	public static SaveSystem CreateSave(int slotID)
	{
		slotID = (slotID % slotUsed.Length + slotUsed.Length) % slotUsed.Length;
		if (slotUsed[slotID])
		{
			DeleteSave(slotID);
		}
		string name = "save" + slotID.ToString("00") + ".bin";
		slotUsed[slotID] = true;
		SaveSystem ret = dummy.Copy(name);
		ret.SaveToDisk();
		return ret;
	}
	/// <summary>
	/// Deletes an existing save, does nothing if the save doesn't exist
	/// </summary>
	/// <param name="slotID">The slotID to delete</param>
	public static void DeleteSave(int slotID)
	{
		slotID = (slotID % slotUsed.Length + slotUsed.Length) % slotUsed.Length;
		if (!slotUsed[slotID])
		{
			return;
		}
		GetSave(slotID).Delete();
		slotUsed[slotID] = false;
	}

	// scene loaders
	/// <summary>
	/// Loads the scene with save data given by the name or path
	/// </summary>
	/// <param name="name">The name or path of the save file to load</param>
	public static void LoadSceneFromName(string name) 
	{
		LoadSceneFromID(GetSaveId(name));
	}
	/// <summary>
	/// Loads the scene with save data given by the slotID
	/// </summary>
	/// <param name="slotID">The slot ID to load</param>
	public static void LoadSceneFromID(int slotID)
	{
		SaveSystem toLoad = GetSave(slotID);
		LoadSceneFromSave(toLoad);		
	}
	/// <summary>
	/// Loads the scene with save data given by the SaveSystem
	/// </summary>
	/// <param name="save">The SaveSystem to load</param>
	public static void LoadSceneFromSave(SaveSystem save)
	{
		// TODO load save

		current = save;
		current.SaveToDisk();
		SceneManager.LoadScene("TestScene");
	}

}
