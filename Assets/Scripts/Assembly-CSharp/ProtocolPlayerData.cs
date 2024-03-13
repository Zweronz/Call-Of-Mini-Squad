using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CoMDS2;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

public class ProtocolPlayerData : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["registerTa"] = ((!(UtilUIAccountManager.mInstance.accountData.email == string.Empty)) ? 1 : 0);
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		string text = JsonMapper.ToJson(hashtable);
		Debug.LogWarning("--------ProtocolPlayerData jsonText : " + text);
		return text;
	}

	public override int GetResponse(string response)
	{
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}

		string path = Application.persistentDataPath + "/saves/playerData.json";
			
		//if (!File.Exists(path))
		//{
			File.WriteAllText(path, JsonConvert.SerializeObject(new DummyProtocol(), Formatting.Indented));
		//}

		DummyProtocol dummyProtocol = JsonConvert.DeserializeObject<DummyProtocol>(File.ReadAllText(path));

		DataCenter.Save().Money = dummyProtocol.money;
		DataCenter.Save().Crystal = dummyProtocol.crystal;
		DataCenter.Save().Honor = dummyProtocol.honor;

		UIConstant.MoneyExchangRate = dummyProtocol.moneyExchangeRate;
		UIConstant.HornorExchangRate = dummyProtocol.honorExchangeRate;

		DataCenter.Save().GetTeamData().teamLevel = dummyProtocol.teamLevel;
		DataCenter.Save().GetTeamData().teamMaxLevel = dummyProtocol.teamMaxLevel;
		DataCenter.Save().GetTeamData().teamExp = dummyProtocol.teamExp;
		DataCenter.Save().GetTeamData().teamMaxExp = dummyProtocol.teamMaxExp;

		if (dummyProtocol.teamSites != null)
		{
			for (int i = 0; i < dummyProtocol.teamSites.Length; i++)
			{
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).state = dummyProtocol.teamSites[i].state;
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSiteLevel = dummyProtocol.teamSites[i].unlockTeamLevel;
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSitePrice = dummyProtocol.teamSites[i].unlockCrystal;
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSiteMoney = dummyProtocol.teamSites[i].unlockMoney;
			}
		}

		DataCenter.Save().RemoveAllHeroes();

		if (dummyProtocol.heroes != null)
		{
			for (int j = 0; j < dummyProtocol.heroes.Length; j++)
			{
				DataCenter.Save().AddHero(dummyProtocol.heroes[j]);
	
				if (dummyProtocol.heroes[j].siteNum != -1)
				{
					DataCenter.Save().SetHeroOnTeamSite(dummyProtocol.heroes[j], (Defined.TEAM_SITE)dummyProtocol.heroes[j].siteNum);
				}
			}
		}
		
		if (dummyProtocol.stuffs != null)
		{
			for (int k = 0; k < dummyProtocol.stuffs.Length; k++)
			{
				//uh? fix this if it screws with stuff ig??
				StuffData stuffData = dummyProtocol.stuffs[k];
			}
		}
			
		DataCenter.Save().GetTeamData().talents.Clear();

		if (dummyProtocol.geniusList != null)
		{
			for (int l = 0; l < dummyProtocol.geniusList.Length; l++)
			{
				DataCenter.Save().GetTeamData().talents.Add((TeamSpecialAttribute.TeamAttributeType)dummyProtocol.geniusList[l].index, dummyProtocol.geniusList[l].level);
			}
		}

		DataCenter.Save().GetTeamData().evolves.Clear();
		if (dummyProtocol.evolutionList != null)
		{
			for (int m = 0; m < dummyProtocol.evolutionList.Length; m++)
			{
				DataCenter.Save().GetTeamData().evolves.Add((TeamSpecialAttribute.TeamAttributeEvolveType)dummyProtocol.evolutionList[m].index, dummyProtocol.evolutionList[m].level);
			}
		}

		return 0;
	}

	public class DummyProtocol
	{
		public int money;

		public int crystal;

		public int honor;

		public int moneyExchangeRate;

		public int honorExchangeRate;

		public int teamLevel;

		public int teamMaxLevel;

		public int teamExp;

		public int teamMaxExp;

		public DummyProtocol()
		{
			teamLevel = 1;
			teamSites = new TeamSite[5] { new TeamSite() { state = Defined.ItemState.Available }, new TeamSite() { state = Defined.ItemState.Available }, new TeamSite() { state = Defined.ItemState.Available }, new TeamSite() { state = Defined.ItemState.Available }, new TeamSite() { state = Defined.ItemState.Available } };

			List<PlayerData> heroes = new List<PlayerData>();

			heroes.Add(new PlayerData()
			{
				heroIndex = 0,

				state = Defined.ItemState.Available,

				equips = new System.Collections.Generic.Dictionary<Defined.EQUIP_TYPE, UserEquipData>
				{
					{Defined.EQUIP_TYPE.Head, new UserEquipData() { currEquipIndex = 0, currEquipLevel = 0 } },
					{Defined.EQUIP_TYPE.Body, new UserEquipData() { currEquipIndex = 11, currEquipLevel = 0 } },
					{Defined.EQUIP_TYPE.Acc, new UserEquipData() { currEquipIndex = 22, currEquipLevel = 0 } }
				},

				siteNum = 0,
					
				upgradeData = new UpgradeData() 
				{ 
					ArmorsUpgrade = new EquipUpgradeData[1]{ new EquipUpgradeData() { index = 1, equipIndex = 11,  } },
					helmsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 2, equipIndex = 0 } },
					ornamentsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 3, equipIndex = 22 } }
				}
			});

			heroes.Add(new PlayerData()
			{
				heroIndex = 1,

				state = Defined.ItemState.Available,

				equips = new System.Collections.Generic.Dictionary<Defined.EQUIP_TYPE, UserEquipData>
				{
					{Defined.EQUIP_TYPE.Head, new UserEquipData() { currEquipIndex = 0, currEquipLevel = 0 } },
					{Defined.EQUIP_TYPE.Body, new UserEquipData() { currEquipIndex = 11, currEquipLevel = 0 } },
					{Defined.EQUIP_TYPE.Acc, new UserEquipData() { currEquipIndex = 22, currEquipLevel = 0 } }
				},

				siteNum = 1,
					
				upgradeData = new UpgradeData() 
				{ 
					ArmorsUpgrade = new EquipUpgradeData[1]{ new EquipUpgradeData() { index = 1, equipIndex = 11,  } },
					helmsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 2, equipIndex = 0 } },
					ornamentsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 3, equipIndex = 22 } }
				}
			});

			for (int i = 2; i < 17; i++)
			{
				if (i == 8 || i == 5 || i == 15)
				{
					continue;
				}
				
				heroes.Add(new PlayerData()
				{
					heroIndex = i,

					state = Defined.ItemState.Available,

					equips = new System.Collections.Generic.Dictionary<Defined.EQUIP_TYPE, UserEquipData>
					{
						{Defined.EQUIP_TYPE.Head, new UserEquipData() { currEquipIndex = 0, currEquipLevel = 0 } },
						{Defined.EQUIP_TYPE.Body, new UserEquipData() { currEquipIndex = 11, currEquipLevel = 0 } },
						{Defined.EQUIP_TYPE.Acc, new UserEquipData() { currEquipIndex = 22, currEquipLevel = 0 } }
					},

					siteNum = -1,
					
					upgradeData = new UpgradeData() 
					{ 
						ArmorsUpgrade = new EquipUpgradeData[1]{ new EquipUpgradeData() { index = 1, equipIndex = 11,  } },
						helmsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 2, equipIndex = 0 } },
						ornamentsUpgrade = new EquipUpgradeData[1] { new EquipUpgradeData() { index = 3, equipIndex = 22 } }
					}
				});
			}

			this.heroes = heroes.ToArray();
		}

		public class TeamSite
		{
			public Defined.ItemState state;

			public int unlockTeamLevel;

			public int unlockCrystal;

			public int unlockMoney;
		}

		public TeamSite[] teamSites;

		public PlayerData[] heroes;

		public StuffData[] stuffs;

		public class Genius
		{
			public int index;

			public int level;
		}

		public Genius[] geniusList;

		public class Evolution
		{
			public int index;

			public int level;
		}

		public Evolution[] evolutionList;
	}
}
