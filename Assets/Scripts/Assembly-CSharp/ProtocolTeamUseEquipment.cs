using System.Collections;
using LitJson;

public class ProtocolTeamUseEquipment : Protocol
{
	private int _hindex = -1;

	private int _type;

	private int _index;

	public override string GetRequest()
	{
		_hindex = DataCenter.State().selectHeroIndex;
		_index = DataCenter.State().selectEquipIndex;
		_type = (int)DataCenter.State().selectEquipType;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = _hindex.ToString();
		hashtable["type"] = _type.ToString();
		hashtable["index"] = _index.ToString();
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
			Defined.EQUIP_TYPE key = (Defined.EQUIP_TYPE)int.Parse(jsonData["type"].ToString());
			int currEquipIndex = int.Parse(jsonData["equipIndex"].ToString());
			int currEquipLevel = int.Parse(jsonData["level"].ToString());
			DataCenter.Save().GetPlayerData(_hindex).equips[key].currEquipIndex = currEquipIndex;
			DataCenter.Save().GetPlayerData(_hindex).equips[key].currEquipLevel = currEquipLevel;
			DataCenter.Save().GetPlayerData(_hindex).combat = int.Parse(jsonData["power"].ToString());
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
