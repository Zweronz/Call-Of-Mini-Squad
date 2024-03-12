using System;
using System.Collections;
using LitJson;

public class ProtocolTeamLevelUpEquipment : Protocol
{
	private int _hindex = -1;

	private int _type;

	private int _index;

	private int useCrystal;

	public override string GetRequest()
	{
		_hindex = DataCenter.State().selectHeroIndex;
		_index = DataCenter.State().selectEquipIndex;
		_type = (int)DataCenter.State().selectEquipType;
		useCrystal = DataCenter.State().protocoUseCrystal;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = _hindex.ToString();
		hashtable["type"] = _type.ToString();
		hashtable["index"] = _index.ToString();
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
			if (_type == 0)
			{
				if (DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Head].currEquipIndex == DataCenter.Save().GetPlayerData(_hindex).upgradeData.helmsUpgrade[_index - 1].equipIndex)
				{
					DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Head].currEquipLevel = int.Parse(jsonData["level"].ToString());
				}
			}
			else if (_type == 1)
			{
				if (DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Body].currEquipIndex == DataCenter.Save().GetPlayerData(_hindex).upgradeData.ArmorsUpgrade[_index - 1].equipIndex)
				{
					DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Body].currEquipLevel = int.Parse(jsonData["level"].ToString());
				}
			}
			else if (DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Acc].currEquipIndex == DataCenter.Save().GetPlayerData(_hindex).upgradeData.ornamentsUpgrade[_index - 1].equipIndex)
			{
				DataCenter.Save().GetPlayerData(_hindex).equips[Defined.EQUIP_TYPE.Acc].currEquipLevel = int.Parse(jsonData["level"].ToString());
			}
			DataCenter.Save().GetPlayerData(_hindex).combat = int.Parse(jsonData["power"].ToString());
			if (((IDictionary)jsonData).Contains((object)"equipments"))
			{
				JsonData jsonData2 = jsonData["equipments"];
				DataCenter.State().lsUpdatedEquipmentEquipIndexs.Clear();
				for (int i = 0; i < jsonData2.Count; i++)
				{
					JsonData jsonData3 = jsonData2[i];
					EquipUpgradeData equipUpgradeData = null;
					if (_type == 0)
					{
						equipUpgradeData = DataCenter.Save().GetPlayerData(_hindex).upgradeData.helmsUpgrade[int.Parse(jsonData3["index"].ToString()) - 1];
						DataCenter.Save().GetPlayerData(_hindex).upgradeData.helmsUpgrade[equipUpgradeData.index - 1] = equipUpgradeData;
					}
					else if (_type == 1)
					{
						equipUpgradeData = DataCenter.Save().GetPlayerData(_hindex).upgradeData.ArmorsUpgrade[int.Parse(jsonData3["index"].ToString()) - 1];
					}
					else if (_type == 2)
					{
						equipUpgradeData = DataCenter.Save().GetPlayerData(_hindex).upgradeData.ornamentsUpgrade[int.Parse(jsonData3["index"].ToString()) - 1];
					}
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
					DataCenter.State().lsUpdatedEquipmentEquipIndexs.Add(equipUpgradeData.equipIndex);
				}
			}
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			DataCenter.State().protocoUseCrystal = 0;
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
