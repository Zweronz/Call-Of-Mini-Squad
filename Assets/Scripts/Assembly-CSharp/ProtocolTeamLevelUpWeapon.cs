using System.Collections;
using LitJson;

public class ProtocolTeamLevelUpWeapon : Protocol
{
	private int _index = -1;

	private int useCrystal;

	public override string GetRequest()
	{
		_index = DataCenter.State().selectHeroIndex;
		useCrystal = DataCenter.State().protocoUseCrystal;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = _index.ToString();
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
			DataCenter.Save().GetPlayerData(_index).weaponLevel = int.Parse(jsonData["weaponLevel"].ToString());
			DataCenter.Save().GetPlayerData(_index).weaponMaxLevel = int.Parse(jsonData["weaponMaxLevel"].ToString());
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
			DataCenter.Save().GetPlayerData(_index).combat = int.Parse(jsonData["power"].ToString());
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
