using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
	private static SoundSystem instance = null;
	private static readonly object padlock = new();

	void Awake()
    {
		// simple way to enforce Singleton
		lock (padlock)
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				DontDestroyOnLoad(gameObject);
				instance = this;
				LoadFromProfile();
			}
		}
	}

	private void LoadFromProfile()
	{
		if (instance == null) return;
		if (Saves.profile == null) return;
		if (!Saves.profile.HasKey("BGM_volume")) return;
		float volume = Saves.profile.GetFloat("BGM_volume");
		GetBGMAudioSource().volume = volume;
	}

	public static AudioSource GetBGMAudioSource()
	{
		return instance.gameObject.GetComponent<AudioSource>();
	}

	public static bool IsActive()
	{
		return instance != null;
	}

	public static void SaveToProfile()
	{
		if (instance == null) return;
		if (Saves.profile == null) return;
		AudioSource source = GetBGMAudioSource();
		Saves.profile.SetFloat("BGM_volume", source.volume);
	}
}
