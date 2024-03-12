using System;
using System.Collections;
using LitJson;

public class ProtocolTeamLoadEquipmentInfo : Protocol
{
	private int _index = -1;

	public override string GetRequest()
	{
		_index = DataCenter.State().selectHeroIndex;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = _index.ToString();
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				return code;
			}
			DataCenter.Save().GetPlayerData(_index).upgradeData = new UpgradeData();
			DataCenter.Save().GetPlayerData(_index).weaponLevel = int.Parse(jsonData["weaponLevel"].ToString());
			DataCenter.Save().GetPlayerData(_index).weaponStar = int.Parse(jsonData["weaponStar"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUpgradeCost = int.Parse(jsonData["weaponUpgradeCost"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUpgradeCostType = GetCostType(jsonData["weaponUpgradeCostType"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponBreakCostMoney = int.Parse(jsonData["weaponBkMoney"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUBreakCostHonor = int.Parse(jsonData["weaponBkHonor"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUBreakCostCrystal = int.Parse(jsonData["weaponBkCrystal"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponBkTeamLevel = int.Parse(jsonData["weaponBkTeamLevel"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponCombat = int.Parse(jsonData["weaponPower"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponCanUpgrade = ((int.Parse(jsonData["weaponCanUpgrade"].ToString()) != 0) ? true : false);
			DataCenter.Save().GetPlayerData(_index).upgradeData.weaponCanBk = ((int.Parse(jsonData["weaponCanBk"].ToString()) != 0) ? true : false);
			DataCenter.Save().GetPlayerData(_index).skillLevel = int.Parse(jsonData["skillLevel"].ToString());
			DataCenter.Save().GetPlayerData(_index).skillStar = int.Parse(jsonData["skillStar"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillUpgradeCost = int.Parse(jsonData["skillUpgradeCost"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillUpgradeCostType = GetCostType(jsonData["skillUpgradeCostType"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostMoney = int.Parse(jsonData["skillBkMoney"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostHonor = int.Parse(jsonData["skillBkHonor"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostCrystal = int.Parse(jsonData["skillBkCrystal"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillBkTeamLevel = int.Parse(jsonData["skillBkTeamLevel"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillCombat = int.Parse(jsonData["skillPower"].ToString());
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillCanUpgrade = ((int.Parse(jsonData["skillCanUpgrade"].ToString()) != 0) ? true : false);
			DataCenter.Save().GetPlayerData(_index).upgradeData.skillCanBk = ((int.Parse(jsonData["skillCanBk"].ToString()) != 0) ? true : false);
			JsonData jsonData2 = jsonData["armor"];
			DataCenter.Save().GetPlayerData(_index).upgradeData.ArmorsUpgrade = new EquipUpgradeData[jsonData2.Count];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				EquipUpgradeData equipUpgradeData = new EquipUpgradeData();
				equipUpgradeData.index = int.Parse(jsonData3["index"].ToString());
				equipUpgradeData.equipIndex = int.Parse(jsonData3["equipIndex"].ToString());
				equipUpgradeData.level = int.Parse(jsonData3["level"].ToString());
				equipUpgradeData.maxLevel = int.Parse(jsonData3["maxLevel"].ToString());
				equipUpgradeData.state = (Defined.ItemState)int.Parse(jsonData3["state"].ToString());
				equipUpgradeData.unlockNeedTeamLevel = int.Parse(jsonData3["unlockNeedTeamLevel"].ToString());
				equipUpgradeData.needPreLevel = int.Parse(jsonData3["needPreLevel"].ToString());
				equipUpgradeData.cost = int.Parse(jsonData3["cost"].ToString());
				equipUpgradeData.costType = GetCostType(jsonData3["costType"].ToString());
				equipUpgradeData.unlockMoney = int.Parse(jsonData3["unlockMoney"].ToString());
				equipUpgradeData.unlockHonor = int.Parse(jsonData3["unlockHonor"].ToString());
				equipUpgradeData.unlockCrystal = int.Parse(jsonData3["unlockCrystal"].ToString());
				equipUpgradeData.combat = int.Parse(jsonData3["power"].ToString());
				equipUpgradeData.canUpgrade = ((int.Parse(jsonData3["canUpgrade"].ToString()) != 0) ? true : false);
				DataCenter.Save().GetPlayerData(_index).upgradeData.ArmorsUpgrade[i] = equipUpgradeData;
			}
			JsonData jsonData4 = jsonData["helms"];
			DataCenter.Save().GetPlayerData(_index).upgradeData.helmsUpgrade = new EquipUpgradeData[jsonData4.Count];
			for (int j = 0; j < jsonData4.Count; j++)
			{
				JsonData jsonData5 = jsonData4[j];
				EquipUpgradeData equipUpgradeData2 = new EquipUpgradeData();
				equipUpgradeData2.index = int.Parse(jsonData5["index"].ToString());
				equipUpgradeData2.equipIndex = int.Parse(jsonData5["equipIndex"].ToString());
				equipUpgradeData2.level = int.Parse(jsonData5["level"].ToString());
				equipUpgradeData2.maxLevel = int.Parse(jsonData5["maxLevel"].ToString());
				equipUpgradeData2.state = (Defined.ItemState)int.Parse(jsonData5["state"].ToString());
				equipUpgradeData2.unlockNeedTeamLevel = int.Parse(jsonData5["unlockNeedTeamLevel"].ToString());
				equipUpgradeData2.needPreLevel = int.Parse(jsonData5["needPreLevel"].ToString());
				equipUpgradeData2.cost = int.Parse(jsonData5["cost"].ToString());
				equipUpgradeData2.costType = GetCostType(jsonData5["costType"].ToString());
				equipUpgradeData2.unlockMoney = int.Parse(jsonData5["unlockMoney"].ToString());
				equipUpgradeData2.unlockHonor = int.Parse(jsonData5["unlockHonor"].ToString());
				equipUpgradeData2.unlockCrystal = int.Parse(jsonData5["unlockCrystal"].ToString());
				equipUpgradeData2.combat = int.Parse(jsonData5["power"].ToString());
				equipUpgradeData2.canUpgrade = ((int.Parse(jsonData5["canUpgrade"].ToString()) != 0) ? true : false);
				DataCenter.Save().GetPlayerData(_index).upgradeData.helmsUpgrade[j] = equipUpgradeData2;
			}
			JsonData jsonData6 = jsonData["ornaments"];
			DataCenter.Save().GetPlayerData(_index).upgradeData.ornamentsUpgrade = new EquipUpgradeData[jsonData6.Count];
			for (int k = 0; k < jsonData6.Count; k++)
			{
				JsonData jsonData7 = jsonData6[k];
				EquipUpgradeData equipUpgradeData3 = new EquipUpgradeData();
				equipUpgradeData3.index = int.Parse(jsonData7["index"].ToString());
				equipUpgradeData3.equipIndex = int.Parse(jsonData7["equipIndex"].ToString());
				equipUpgradeData3.level = int.Parse(jsonData7["level"].ToString());
				equipUpgradeData3.maxLevel = int.Parse(jsonData7["maxLevel"].ToString());
				equipUpgradeData3.state = (Defined.ItemState)int.Parse(jsonData7["state"].ToString());
				equipUpgradeData3.unlockNeedTeamLevel = int.Parse(jsonData7["unlockNeedTeamLevel"].ToString());
				equipUpgradeData3.needPreLevel = int.Parse(jsonData7["needPreLevel"].ToString());
				equipUpgradeData3.cost = int.Parse(jsonData7["cost"].ToString());
				equipUpgradeData3.costType = GetCostType(jsonData7["costType"].ToString());
				equipUpgradeData3.unlockMoney = int.Parse(jsonData7["unlockMoney"].ToString());
				equipUpgradeData3.unlockHonor = int.Parse(jsonData7["unlockHonor"].ToString());
				equipUpgradeData3.unlockCrystal = int.Parse(jsonData7["unlockCrystal"].ToString());
				equipUpgradeData3.combat = int.Parse(jsonData7["power"].ToString());
				equipUpgradeData3.canUpgrade = ((int.Parse(jsonData7["canUpgrade"].ToString()) != 0) ? true : false);
				DataCenter.Save().GetPlayerData(_index).upgradeData.ornamentsUpgrade[k] = equipUpgradeData3;
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
