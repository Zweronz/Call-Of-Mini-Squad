using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
	private static Localization mInstance;

	public static string[] knownLanguages;

	public static bool localizationHasBeenSet = false;

	[HideInInspector]
	public string startingLanguage = "English";

	[HideInInspector]
	public TextAsset[] languages;

	private static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	private static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	private static int mLanguageIndex = -1;

	private static string mLanguage;

	public static Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!localizationHasBeenSet)
			{
				language = PlayerPrefs.GetString("Language", "English");
			}
			return mDictionary;
		}
	}

	public static bool isActive
	{
		get
		{
			return mInstance != null;
		}
	}

	public static Localization instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = UnityEngine.Object.FindObjectOfType(typeof(Localization)) as Localization;
				if (mInstance == null)
				{
					GameObject gameObject = new GameObject("_Localization");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					mInstance = gameObject.AddComponent<Localization>();
				}
			}
			return mInstance;
		}
	}

	[Obsolete("Use Localization.language instead")]
	public string currentLanguage
	{
		get
		{
			return language;
		}
		set
		{
			language = value;
		}
	}

	public static string language
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (!(mLanguage != value))
			{
				return;
			}
			if (!string.IsNullOrEmpty(value))
			{
				if (mDictionary.Count == 0)
				{
					TextAsset textAsset = ((!localizationHasBeenSet) ? (Resources.Load("Localization", typeof(TextAsset)) as TextAsset) : null);
					localizationHasBeenSet = true;
					if (textAsset == null || !LoadCSV(textAsset))
					{
						textAsset = Resources.Load(value, typeof(TextAsset)) as TextAsset;
						if (textAsset != null)
						{
							Load(textAsset);
							return;
						}
					}
				}
				if (mDictionary.Count != 0 && SelectLanguage(value))
				{
					return;
				}
				if (mInstance != null && mInstance.languages != null)
				{
					int i = 0;
					for (int num = mInstance.languages.Length; i < num; i++)
					{
						TextAsset textAsset2 = mInstance.languages[i];
						if (textAsset2 != null && textAsset2.name == value)
						{
							Load(textAsset2);
							return;
						}
					}
				}
			}
			mOldDictionary.Clear();
			PlayerPrefs.DeleteKey("Language");
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (mOldDictionary.Count == 0 && mDictionary.Count == 0)
			{
				language = PlayerPrefs.GetString("Language", startingLanguage);
			}
			if (string.IsNullOrEmpty(mLanguage) && languages != null && languages.Length > 0)
			{
				language = languages[0].name;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
	}

	private void OnDisable()
	{
		localizationHasBeenSet = false;
		mLanguageIndex = -1;
		mDictionary.Clear();
		mOldDictionary.Clear();
	}

	private void OnDestroy()
	{
		if (mInstance == this)
		{
			mInstance = null;
		}
	}

	public static void Load(TextAsset asset)
	{
		ByteReader byteReader = new ByteReader(asset);
		Set(asset.name, byteReader.ReadDictionary());
	}

	public static bool LoadCSV(TextAsset asset)
	{
		ByteReader byteReader = new ByteReader(asset);
		BetterList<string> betterList = byteReader.ReadCSV();
		if (betterList.size < 2)
		{
			return false;
		}
		betterList[0] = "KEY";
		if (!string.Equals(betterList[0], "KEY"))
		{
			return false;
		}
		knownLanguages = new string[betterList.size - 1];
		for (int i = 0; i < knownLanguages.Length; i++)
		{
			knownLanguages[i] = betterList[i + 1];
		}
		mDictionary.Clear();
		while (betterList != null)
		{
			AddCSV(betterList);
			betterList = byteReader.ReadCSV();
		}
		return true;
	}

	private static bool SelectLanguage(string language)
	{
		mLanguageIndex = -1;
		if (mDictionary.Count == 0)
		{
			return false;
		}
		string[] value;
		if (mDictionary.TryGetValue("KEY", out value))
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == language)
				{
					mOldDictionary.Clear();
					mLanguageIndex = i;
					mLanguage = language;
					PlayerPrefs.SetString("Language", mLanguage);
					UIRoot.Broadcast("OnLocalize");
					return true;
				}
			}
		}
		return false;
	}

	private static void AddCSV(BetterList<string> values)
	{
		if (values.size >= 2)
		{
			string[] array = new string[values.size - 1];
			for (int i = 1; i < values.size; i++)
			{
				array[i - 1] = values[i];
			}
			mDictionary.Add(values[0], array);
		}
	}

	public static void Set(string languageName, Dictionary<string, string> dictionary)
	{
		mLanguage = languageName;
		PlayerPrefs.SetString("Language", mLanguage);
		mOldDictionary = dictionary;
		localizationHasBeenSet = false;
		mLanguageIndex = -1;
		knownLanguages = new string[1] { languageName };
		UIRoot.Broadcast("OnLocalize");
	}

	public static string Get(string key)
	{
		if (!localizationHasBeenSet)
		{
			language = PlayerPrefs.GetString("Language", "English");
		}
		string key2 = key + " Mobile";
		string[] value;
		string value2;
		if (mLanguageIndex != -1 && mDictionary.TryGetValue(key2, out value))
		{
			if (mLanguageIndex < value.Length)
			{
				return value[mLanguageIndex];
			}
		}
		else if (mOldDictionary.TryGetValue(key2, out value2))
		{
			return value2;
		}
		if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out value))
		{
			if (mLanguageIndex < value.Length)
			{
				return value[mLanguageIndex];
			}
		}
		else if (mOldDictionary.TryGetValue(key, out value2))
		{
			return value2;
		}
		return key;
	}

	[Obsolete("Use Localization.Get instead")]
	public static string Localize(string key)
	{
		return Get(key);
	}

	public static bool Exists(string key)
	{
		if (mLanguageIndex != -1)
		{
			return mDictionary.ContainsKey(key);
		}
		return mOldDictionary.ContainsKey(key);
	}
}
