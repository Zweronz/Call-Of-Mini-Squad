using System.Collections;
using LitJson;

public class ProtocolTeamBreakthroughEquipment : Protocol
{
	private int _index = -1;

	private Defined.TeamEquipmentBreakType _type;

	private int useCrystal;

	public override string GetRequest()
	{
		_index = DataCenter.State().selectHeroIndex;
		_type = DataCenter.State().selectEquipBreakType;
		useCrystal = DataCenter.State().protocoUseCrystal;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = _index.ToString();
		if (_type == Defined.TeamEquipmentBreakType.Weapon)
		{
			hashtable["equipmentType"] = "weapon";
		}
		else if (_type == Defined.TeamEquipmentBreakType.Skill)
		{
			hashtable["equipmentType"] = "skill";
		}
		hashtable["useCrystal"] = useCrystal.ToString();
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int code = Protocol.GetCode(jsonData);
			if (((IDictionary)jsonData).Contains((object)"shortMoney"))
			{
				UIConstant.TradeMoneyNotEnough = (int)jsonData["shortMoney"];
			}
			else
			{
				UIConstant.TradeMoneyNotEnough = 0;
			}
			if (((IDictionary)jsonData).Contains((object)"shortHornor"))
			{
				UIConstant.TradeHornorNotEnough = (int)jsonData["shortHornor"];
			}
			else
			{
				UIConstant.TradeHornorNotEnough = 0;
			}
			if (code != 0)
			{
				return code;
			}
			if (_type == Defined.TeamEquipmentBreakType.Weapon)
			{
				DataCenter.Save().GetPlayerData(_index).weaponLevel = int.Parse(jsonData["weaponLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).weaponMaxLevel = int.Parse(jsonData["weaponMaxLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).weaponStar = int.Parse(jsonData["weaponStar"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUpgradeCost = int.Parse(jsonData["weaponUpgradeCost"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUpgradeCostType = GetCostType(jsonData["weaponUpgradeCostType"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponBreakCostMoney = int.Parse(jsonData["weaponBkMoney"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUBreakCostHonor = int.Parse(jsonData["weaponBkHonor"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponUBreakCostCrystal = int.Parse(jsonData["weaponBkCrystal"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponBkTeamLevel = int.Parse(jsonData["weaponBkTeamLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponCanUpgrade = ((int.Parse(jsonData["weaponCanUpgrade"].ToString()) != 0) ? true : false);
				DataCenter.Save().GetPlayerData(_index).upgradeData.weaponCanBk = ((int.Parse(jsonData["weaponCanBk"].ToString()) != 0) ? true : false);
			}
			else if (_type == Defined.TeamEquipmentBreakType.Skill)
			{
				DataCenter.Save().GetPlayerData(_index).skillLevel = int.Parse(jsonData["skillLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).skillMaxLevel = int.Parse(jsonData["skillMaxLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).skillStar = int.Parse(jsonData["skillStar"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillUpgradeCost = int.Parse(jsonData["skillUpgradeCost"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillUpgradeCostType = GetCostType(jsonData["skillUpgradeCostType"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostMoney = int.Parse(jsonData["skillBkMoney"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostHonor = int.Parse(jsonData["skillBkHonor"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillBreakCostCrystal = int.Parse(jsonData["skillBkCrystal"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillBkTeamLevel = int.Parse(jsonData["skillBkTeamLevel"].ToString());
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillCanUpgrade = ((int.Parse(jsonData["skillCanUpgrade"].ToString()) != 0) ? true : false);
				DataCenter.Save().GetPlayerData(_index).upgradeData.skillCanBk = ((int.Parse(jsonData["skillCanBk"].ToString()) != 0) ? true : false);
			}
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
