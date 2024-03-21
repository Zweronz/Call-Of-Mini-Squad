using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zweronz.SavingSystem
{
	public static class Loader
	{
		public static void Load(SaveData save)
		{
			TypeLoader.Load<SaveLoader>(save);
		}
	}

	public static class TypeLoader
	{
		public static void Load<T>(object obj) where T : ILoader
		{
			Activator.CreateInstance<T>().Load(obj);
		}
	}

	public interface ILoader
	{
		void Load(object obj);
	}

	public class SaveLoader : ILoader
	{
		public void Load(object obj)
		{
			SaveData loadObject = obj as SaveData;

			TypeLoader.Load<TeamLoader>(loadObject.teamSave);
			TypeLoader.Load<HeroLoader>(loadObject.heroes);
			TypeLoader.Load<CurrencyLoader>(loadObject.currency);
			TypeLoader.Load<WorldNodeLoader>(loadObject.worldNodes);
		}

		public SaveLoader() {}
	}

	public class TeamLoader : ILoader
	{
		public void Load(object obj)
		{
			TeamSave loadObject = obj as TeamSave;

			DataCenter.Save().SetTeamData(loadObject.teamData);
			DataCenter.Save().teamAttributeSaveData = loadObject.teamAttributeSaveData;
		}

		public TeamLoader() {}
	}

	public class HeroLoader : ILoader
	{
		public void Load(object obj)
		{
			List<PlayerData> loadObject = obj as List<PlayerData>;

			DataCenter.Save().RemoveAllHeroes();

			foreach (PlayerData hero in loadObject)
			{
				DataCenter.Save().AddHero(hero);
	
				if (hero.siteNum != -1)
				{
					DataCenter.Save().SetHeroOnTeamSite(hero, (Defined.TEAM_SITE)hero.siteNum);
				}
			}
		}
	}

	public class CurrencyLoader : ILoader
	{
		public void Load(object obj)
		{
			Currency loadObject = obj as Currency;

			DataCenter.Save().Money = loadObject.money;
			DataCenter.Save().Crystal = loadObject.crystal;
		}
	}

	public class WorldNodeLoader : ILoader
	{
		public void Load(object obj)
		{
			List<GameProgressData> loadObject = obj as List<GameProgressData>;

			DataCenter.Save().SetWorldNodeProgress(loadObject);
		}
	}
}
