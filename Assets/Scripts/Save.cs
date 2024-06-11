using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Zweronz.SavingSystem;

public static class Save
{
	public static string Path
	{
		get
		{
			return Application.persistentDataPath + "/game.save";
		}
	}

	public static bool TestingCreate
	{
		get
		{
			return false;
		}
	}

	public static void Write()
	{
		File.WriteAllText(Path, JsonConvert.SerializeObject(saveInstance = Saver.Save()));
	}

	public static void Load()
	{
		Loader.Load(saveInstance = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(Path)));
	}

	public static void Create()
	{
		Loader.Load(saveInstance = Creator.Create());
	}

	public static void Delete()
	{
		File.Delete(Path);
		Application.Quit();
	}

	public static bool NeedsCreate
	{
		get
		{
			return !File.Exists(Path);
		}
	}

	private static SaveData saveInstance;

	public static SaveData SaveData
	{
		get
		{
			if (saveInstance == null)
			{
				Load();
			}

			return saveInstance;
		}
	}
}

public class SaveData
{
	public Currency currency;

	public TeamSave teamSave;

	public List<PlayerData> heroes;

	public List<GameProgressData> worldNodes;
}

public class Currency
{
	public int money, crystal;
}

public class TeamSave
{
	public TeamData teamData;

	public DataSave.TeamAttributeSaveData teamAttributeSaveData;
}