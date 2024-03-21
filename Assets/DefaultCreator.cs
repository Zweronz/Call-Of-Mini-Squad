using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zweronz.SavingSystem
{
	public static class Creator
	{
		public static SaveData Create()
		{
			return DefaultCreator.Create<SaveDefaultCreator>().saveData;
		}
	}

	public static class DefaultCreator
	{
		public static T Create<T>() where T : IDefaultCreator<T>
		{
			return Activator.CreateInstance<T>().Create();
		}
	}

	public interface IDefaultCreator<T>
	{
		T Create();
	}

	public class SaveDefaultCreator : IDefaultCreator<SaveDefaultCreator>
	{
		public SaveDefaultCreator Create()
		{
			return new SaveDefaultCreator
			{
				saveData = new SaveData
				{
					currency = DefaultCreator.Create<CurrencyDefaultCreator>().currency,
					teamSave = DefaultCreator.Create<TeamDefaultCreator>().teamSave,
					heroes = DefaultCreator.Create<HeroDefaultCreator>().playerData,
					worldNodes = DefaultCreator.Create<WorldNodeDefaultCreator>().gameProgressData
				}
			};
		}

		public SaveData saveData;
	}

	public class TeamDefaultCreator : IDefaultCreator<TeamDefaultCreator>
	{
		public TeamDefaultCreator Create()
		{
            TeamDefaultCreator creator = new TeamDefaultCreator
            {
                teamSave = new TeamSave()
            };

            creator.teamSave.teamData = new TeamData
			{
				teamSitesData = new TeamSiteData[]
				{
					new TeamSiteData { state = Defined.ItemState.Available }, new TeamSiteData { state = Defined.ItemState.Available },
					new TeamSiteData { state = Defined.ItemState.Purchase, unlockSitePrice = 150 }, new TeamSiteData { state = Defined.ItemState.Purchase, unlockSitePrice = 300 }, new TeamSiteData { state = Defined.ItemState.Purchase, unlockSitePrice = 300 }
				},

				teamExp = 0,
				teamLevel = 1,

				//placeholder
				teamMaxLevel = 100,
				teamMaxExp = 10000,

				talents = new Dictionary<CoMDS2.TeamSpecialAttribute.TeamAttributeType, int>
				{
				},

				evolves = new Dictionary<CoMDS2.TeamSpecialAttribute.TeamAttributeEvolveType, int>
				{
				}
			};

			TeamAttributeData[] geniusList = new TeamAttributeData[25], evolutionList = new TeamAttributeData[25];

			for (int i = 0; i < 25; i++)
			{
				geniusList[i] = new TeamAttributeData();
				evolutionList[i] = new TeamAttributeData();
			}

			creator.teamSave.teamAttributeSaveData = new DataSave.TeamAttributeSaveData
			{
				teamAttributeTalent = geniusList,
				teamAttributeEvolve = evolutionList,

				teamGeniusResetCostCrystalPerTimes = 20
			};

			return creator;
		}

		public TeamSave teamSave;
	}

	public class HeroDefaultCreator : IDefaultCreator<HeroDefaultCreator>
	{
		public HeroDefaultCreator Create()
		{
			HeroDefaultCreator creator = new HeroDefaultCreator
			{
				playerData = new List<PlayerData>
				{
					new PlayerData
					{
						heroIndex = 0,
						siteNum = 0,
						state = Defined.ItemState.Available,
					},
					new PlayerData
					{
						heroIndex = 1,
						siteNum = 1,
						state = Defined.ItemState.Available,
					},
					new PlayerData
					{
						heroIndex = 9,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = 10,
						costType = Defined.COST_TYPE.Money,
						unlockCost = 10000
					},
					new PlayerData
					{
						heroIndex = 9,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = 10,
						costType = Defined.COST_TYPE.Money,
						unlockCost = 10000
					},
					new PlayerData
					{
						heroIndex = 7,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = 20,
						costType = Defined.COST_TYPE.Money,
						unlockCost = 20000
					},
					new PlayerData
					{
						heroIndex = 3,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = 30,
						costType = Defined.COST_TYPE.Money,
						unlockCost = 30000
					},
					new PlayerData
					{
						heroIndex = 13,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 299
					},
					new PlayerData
					{
						heroIndex = 11,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 299
					},
					new PlayerData
					{
						heroIndex = 10,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 149
					},
					new PlayerData
					{
						heroIndex = 14,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 299
					},
					new PlayerData
					{
						heroIndex = 2,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = -1
					},
					new PlayerData
					{
						heroIndex = 6,
						state = Defined.ItemState.Locked,
						unlockNeedTeamLevel = -2
					},
					new PlayerData
					{
						heroIndex = 12,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 149
					},
					new PlayerData
					{
						heroIndex = 15,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 299
					},
					new PlayerData
					{
						heroIndex = 17,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Crystal,
						unlockCost = 299
					},
					new PlayerData
					{
						heroIndex = 16,
						state = Defined.ItemState.Purchase,
						costType = Defined.COST_TYPE.Money,
						unlockCost = 75000
					},
				}
			};

			foreach (PlayerData hero in creator.playerData)
			{
				hero.equips = new System.Collections.Generic.Dictionary<Defined.EQUIP_TYPE, UserEquipData>
				{
					{Defined.EQUIP_TYPE.Acc, new UserEquipData { currEquipIndex = 22, currEquipLevel = -1 } },
					{Defined.EQUIP_TYPE.Body, new UserEquipData { currEquipIndex = 11, currEquipLevel = -1 } },
					{Defined.EQUIP_TYPE.Head, new UserEquipData { currEquipIndex = 0, currEquipLevel = -1 } }
				};

                hero.upgradeData = new UpgradeData
                {
                    helmsUpgrade = new EquipUpgradeData[11],
					ArmorsUpgrade = new EquipUpgradeData[11],
                    ornamentsUpgrade = new EquipUpgradeData[10]
                };

				hero.weaponLevel = 1;
				hero.skillLevel = 1;

				//placeholder
				hero.weaponMaxLevel = 5;
				hero.skillMaxLevel = 5;

                for (int i = 0; i < 32; i++)
				{
					if (i < 11)
					{
						hero.upgradeData.helmsUpgrade[i] = new EquipUpgradeData() { index = i + 1, equipIndex = i, level = 0, state = Defined.ItemState.Locked, unlockNeedTeamLevel = 30 };
					}
					else if (i < 22)
					{
						hero.upgradeData.ArmorsUpgrade[i - 11] = new EquipUpgradeData() { index = i - 10, equipIndex = i, level = 0, state = Defined.ItemState.Locked, unlockNeedTeamLevel = 30 };
					}
					else
					{
						hero.upgradeData.ornamentsUpgrade[i - 22] = new EquipUpgradeData() { index = i - 21, equipIndex = i, level = 0, state = Defined.ItemState.Locked, unlockNeedTeamLevel = 30 };
					}
				}
			}

			return creator;
		}

		public HeroDefaultCreator() {}

		public List<PlayerData> playerData;
	}

	public class CurrencyDefaultCreator : IDefaultCreator<CurrencyDefaultCreator>
	{
		public CurrencyDefaultCreator Create()
		{
			return new CurrencyDefaultCreator
			{
				currency = new Currency
				{
					money = 0,
					crystal = 0
				}
			};
		}

		public CurrencyDefaultCreator() {}

		public Currency currency;
	}

	public class WorldNodeDefaultCreator : IDefaultCreator<WorldNodeDefaultCreator>
	{
		public WorldNodeDefaultCreator Create()
		{
			return new WorldNodeDefaultCreator
			{
				gameProgressData = new List<GameProgressData>
				{
					new GameProgressData
					{
						levelProgress = new ushort[20]
						{
							1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
						},

						modeProgress = 0,

						levelStars = new Dictionary<Defined.LevelMode, ushort[]>
						{
							{Defined.LevelMode.Normal, new ushort[20]},
							{Defined.LevelMode.Hard, new ushort[20]},
							{Defined.LevelMode.Hell, new ushort[20]}
						}
					}
				}
			};
		}

		public List<GameProgressData> gameProgressData;
	}
}