using System;
using System.Collections;
using CoMDS2;
using LitJson;
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
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			JsonData jsonData2 = jsonData["code"];
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				return code;
			}
			DataCenter.Save().loginCode = jsonData["loginCode"].ToString();
			DataCenter.Save().userName = (string)jsonData["userName"];
			DataCenter.Save().tutorialStep = (Defined.TutorialStep)int.Parse(jsonData["lesson"].ToString());
			if (DataCenter.Save().tutorialStep == Defined.TutorialStep.EnterBattle)
			{
			}
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			UIConstant.MoneyExchangRate = (int)jsonData["moneyExchangeRate"];
			UIConstant.HornorExchangRate = (int)jsonData["honorExchangeRate"];
			DataCenter.Save().GetTeamData().teamLevel = (int)jsonData["teamLevel"];
			DataCenter.Save().GetTeamData().teamMaxLevel = (int)jsonData["teamMaxLevel"];
			DataCenter.Save().GetTeamData().teamExp = (int)jsonData["teamExp"];
			DataCenter.Save().GetTeamData().teamMaxExp = (int)jsonData["teamMaxExp"];
			JsonData jsonData3 = jsonData["teamSites"];
			for (int i = 0; i < jsonData3.Count; i++)
			{
				JsonData jsonData4 = jsonData3[i];
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).state = (Defined.ItemState)(int)jsonData4["state"];
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSiteLevel = (int)jsonData4["unlockTeamLevel"];
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSitePrice = (int)jsonData4["unlockCrystal"];
				DataCenter.Save().GetTeamSiteData((Defined.TEAM_SITE)i).unlockSiteMoney = (int)jsonData4["unlockMoney"];
			}
			JsonData jsonData5 = jsonData["heroes"];
			int num = 0;
			DataCenter.Save().RemoveAllHeroes();
			for (int j = 0; j < jsonData5.Count; j++)
			{
				JsonData jsonData6 = jsonData5[j];
				PlayerData playerData = new PlayerData();
				playerData.heroIndex = (int)jsonData6["heroIndex"];
				playerData.weaponLevel = (int)jsonData6["weaponLevel"];
				playerData.weaponMaxLevel = (int)jsonData6["weaponMaxLevel"];
				playerData.weaponStar = (int)jsonData6["weaponStar"];
				playerData.skillLevel = (int)jsonData6["skillLevel"];
				playerData.skillMaxLevel = (int)jsonData6["skillMaxLevel"];
				playerData.skillStar = (int)jsonData6["skillStar"];
				playerData.siteNum = (int)jsonData6["siteNum"];
				playerData.state = (Defined.ItemState)(int)jsonData6["state"];
				playerData.unlockNeedTeamLevel = (int)jsonData6["unlockNeedTeamLevel"];
				playerData.unlockCost = (int)jsonData6["unlockCost"];
				playerData.costType = GetCostType(jsonData6["unlockCostType"].ToString());
				playerData.combat = int.Parse(jsonData6["power"].ToString());
				JsonData jsonData7 = jsonData6["helmetUseInfo"];
				UserEquipData userEquipData = new UserEquipData();
				userEquipData.currEquipIndex = (int)jsonData7["currEquipIndex"];
				userEquipData.currEquipLevel = (int)jsonData7["currEquipLevel"];
				playerData.equips.Add(Defined.EQUIP_TYPE.Head, userEquipData);
				JsonData jsonData8 = jsonData6["armorUseInfo"];
				UserEquipData userEquipData2 = new UserEquipData();
				userEquipData2.currEquipIndex = (int)jsonData8["currEquipIndex"];
				userEquipData2.currEquipLevel = (int)jsonData8["currEquipLevel"];
				playerData.equips.Add(Defined.EQUIP_TYPE.Body, userEquipData2);
				JsonData jsonData9 = jsonData6["ornamentUseInfo"];
				UserEquipData userEquipData3 = new UserEquipData();
				userEquipData3.currEquipIndex = (int)jsonData9["currEquipIndex"];
				userEquipData3.currEquipLevel = (int)jsonData9["currEquipLevel"];
				playerData.equips.Add(Defined.EQUIP_TYPE.Acc, userEquipData3);
				DataCenter.Save().AddHero(playerData);
				num++;
				if (playerData.siteNum != -1)
				{
					DataCenter.Save().SetHeroOnTeamSite(playerData, (Defined.TEAM_SITE)playerData.siteNum);
				}
			}
			JsonData jsonData10 = jsonData["stuffs"];
			for (int k = 0; k < jsonData10.Count; k++)
			{
				JsonData jsonData11 = jsonData10[k];
				StuffData stuffData = new StuffData();
				stuffData.index = (int)jsonData11["stuffIndex"];
				stuffData.count = (int)jsonData11["count"];
				stuffData.cost = (int)jsonData11["cost"];
				stuffData.costType = GetCostType(jsonData11["costType"].ToString());
			}
			JsonData jsonData12 = jsonData["geniusList"];
			DataCenter.Save().GetTeamData().talents.Clear();
			for (int l = 0; l < jsonData12.Count; l++)
			{
				JsonData jsonData13 = jsonData12[l];
				int key = (int)jsonData13["index"];
				int value = (int)jsonData13["level"];
				DataCenter.Save().GetTeamData().talents.Add((TeamSpecialAttribute.TeamAttributeType)key, value);
			}
			JsonData jsonData14 = jsonData["evolutionList"];
			DataCenter.Save().GetTeamData().evolves.Clear();
			for (int m = 0; m < jsonData14.Count; m++)
			{
				JsonData jsonData15 = jsonData14[m];
				int key2 = (int)jsonData15["index"];
				int value2 = (int)jsonData15["level"];
				DataCenter.Save().GetTeamData().evolves.Add((TeamSpecialAttribute.TeamAttributeEvolveType)key2, value2);
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
