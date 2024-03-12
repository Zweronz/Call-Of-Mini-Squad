using System.Collections;
using LitJson;

public class ProtocolTeamUnlockEquipment : Protocol
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
				DataCenter.Save().GetPlayerData(_hindex).upgradeData.helmsUpgrade[_index - 1].state = (Defined.ItemState)int.Parse(jsonData["state"].ToString());
			}
			else if (_type == 1)
			{
				DataCenter.Save().GetPlayerData(_hindex).upgradeData.ArmorsUpgrade[_index - 1].state = (Defined.ItemState)int.Parse(jsonData["state"].ToString());
			}
			else
			{
				DataCenter.Save().GetPlayerData(_hindex).upgradeData.ornamentsUpgrade[_index - 1].state = (Defined.ItemState)int.Parse(jsonData["state"].ToString());
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
