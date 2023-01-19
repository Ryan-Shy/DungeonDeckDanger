using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveSystem {
	private string file;
	private DataState data;

	public SaveSystem(string fileName)
	{
		file = fileName;
		if (File.Exists(GetPath()))
		{
			Load();
		}
		else 
		{ 
			data = new DataState();
		}
	}

	private string GetPath()
	{
		return Application.persistentDataPath + "/" + file;
	}

	private void Load()
	{
		data = SerializatorBinary.LoadBinary(GetPath());
		Debug.Log("[SaveGame] --> Loading the save file: " + GetPath());
	}

	private void ReplaceItem(string name, string item)
	{
		bool j = false;
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				data.items[i].Value = Crypt(item);
				j = true;
				break;
			}
		}

		if(!j) data.AddItem(new SaveData(name, Crypt(item)));
	}

	public bool HasKey(string name) // check for a key
    {
		if(string.IsNullOrEmpty(name)) return false;

		foreach(SaveData k in data.items)
		{
			if(string.Compare(name, k.Key) == 0)
			{
				return true;
			}
		}

		return false;
	}

	public List<string> GetKeys()
	{
		List<string> keys = new();
		
		foreach(SaveData d in data.items)
		{
			keys.Add(d.Key);
		}
		return keys;
	}

	public SaveSystem Copy(string fileName)
	{
		// if no new fileName, return reference to this
		if (file.Equals(fileName)) return this;
		// else create new and copy entries
		SaveSystem ret = new SaveSystem(fileName);
		ret.data.items.Clear();
		foreach (SaveData sd in data.items)
		{
			ret.ReplaceItem(sd.Key, sd.Value);
		}
		return ret;
	}

	public void Delete() 
	{
		if (File.Exists(GetPath()))
		{
			File.Delete(GetPath());
		}
	}

	public System.DateTime LastSaveTime()
	{
		return File.GetLastWriteTime(GetPath());
	}

	public string GetName()
	{
		return file;
	}

	public bool Remove(string name)
	{
		if (!HasKey(name)) return false;
		for (int i = 0; i < data.items.Count; i++)
		{
			if (string.Compare(name, data.items[i].Key) == 0)
			{
				data.items.Remove(data.items[i]);
				return true;
			} 
		}
		return false;
	}

	public void SetVector3(string name, Vector3 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y + "|" + val.z);
	}

	public void SetVector2(string name, Vector2 val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.x + "|" + val.y);
	}

	public void SetColor(string name, Color val)
	{
		if(string.IsNullOrEmpty(name)) return;
		SetString(name, val.r + "|" + val.g + "|" + val.b + "|" + val.a);
	}

	public void SetBool(string name, bool val) // set the key and value
    {
		if(string.IsNullOrEmpty(name)) return;
		string tmp = string.Empty;
		if(val) tmp = "1"; else tmp = "0";
		ReplaceItem(name, tmp);
	}

	public void SetFloat(string name, float val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public void SetInt(string name, int val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val.ToString());
	}

	public void SetString(string name, string val)
	{
		if(string.IsNullOrEmpty(name)) return;
		ReplaceItem(name, val);
	}

	public void SaveToDisk() // write data to file
    {
		if(data.items.Count == 0) return;
		SerializatorBinary.SaveBinary(data, GetPath());
		Debug.Log("[SaveGame] --> Save game data: " + GetPath());
	}

	public Vector3 GetVector3(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector3.zero;
		return iVector3(name, Vector3.zero);
	}

	public Vector3 GetVector3(string name, Vector3 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector3(name, defaultValue);
	}

	private Vector3 iVector3(string name, Vector3 defaultValue)
	{
		Vector3 vector = Vector3.zero;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 3)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					vector.z = floatParse(t[2]);
					return vector;
				}
				break;
			}
		}

		return defaultValue;
	}

	public Vector2 GetVector2(string name)
	{
		if(string.IsNullOrEmpty(name)) return Vector2.zero;
		return iVector2(name, Vector2.zero);
	}

	public Vector2 GetVector2(string name, Vector2 defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iVector2(name, defaultValue);
	}

	private Vector2 iVector2(string name, Vector2 defaultValue)
	{
		Vector2 vector = Vector2.zero;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 2)
				{
					vector.x = floatParse(t[0]);
					vector.y = floatParse(t[1]);
					return vector;
				}
				break;
			}
		}

		return defaultValue;
	}

	public Color GetColor(string name)
	{
		if(string.IsNullOrEmpty(name)) return Color.white;
		return iColor(name, Color.white);
	}

	public Color GetColor(string name, Color defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iColor(name, defaultValue);
	}

	private Color iColor(string name, Color defaultValue)
	{
		Color color = Color.clear;

		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				string[] t = Crypt(data.items[i].Value).Split(new char[]{'|'});
				if(t.Length == 4)
				{
					color.r = floatParse(t[0]);
					color.g = floatParse(t[1]);
					color.b = floatParse(t[2]);
					color.a = floatParse(t[3]);
					return color;
				}
				break;
			}
		}

		return defaultValue;
	}

	public bool GetBool(string name) // get value by key
    {
		if(string.IsNullOrEmpty(name)) return false;
		return iBool(name, false);
	}

	public bool GetBool(string name, bool defaultValue) // with the default setting
    {
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iBool(name, defaultValue);
	}

	private bool iBool(string name, bool defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				if(string.Compare(Crypt(data.items[i].Value), "1") == 0) return true; else return false;
			}
		}

		return defaultValue;
	}

	public float GetFloat(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iFloat(name, 0);
	}

	public float GetFloat(string name, float defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iFloat(name, defaultValue);
	}

	private float iFloat(string name, float defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				return floatParse(Crypt(data.items[i].Value));
			}
		}

		return defaultValue;
	}

	public int GetInt(string name)
	{
		if(string.IsNullOrEmpty(name)) return 0;
		return iInt(name, 0);
	}

	public int GetInt(string name, int defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iInt(name, defaultValue);
	}

	private int iInt(string name, int defaultValue)
	{
        if (data != null)
        {
            for (int i = 0; i < data.items.Count; i++)
            {
                if (string.Compare(name, data.items[i].Key) == 0)
                {
                    return intParse(Crypt(data.items[i].Value));
                }
            }
        }

		return defaultValue;
	}

	public string GetString(string name)
	{
		if(string.IsNullOrEmpty(name)) return string.Empty;
		return iString(name, string.Empty);
	}

	public string GetString(string name, string defaultValue)
	{
		if(string.IsNullOrEmpty(name)) return defaultValue;
		return iString(name, defaultValue);
	}

	private string iString(string name, string defaultValue)
	{
		for(int i = 0; i < data.items.Count; i++)
		{
			if(string.Compare(name, data.items[i].Key) == 0)
			{
				return Crypt(data.items[i].Value);
			}
		}

		return defaultValue;
	}

	private int intParse(string val)
	{
		int value;
		if(int.TryParse(val, out value)) return value;
		return 0;
	}

	private float floatParse(string val)
	{
		float value;
		if(float.TryParse(val, out value)) return value;
		return 0;
	}

	private string Crypt(string text)
	{
		string result = string.Empty;
		foreach(char j in text) result += (char)((int)j ^ 42);
		return result;
	}
}
