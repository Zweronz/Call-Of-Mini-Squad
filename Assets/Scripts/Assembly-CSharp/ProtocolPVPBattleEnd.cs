using System;
using System.Collections;
using LitJson;

public class ProtocolPVPBattleEnd : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.User().PVP_myData.sUUID;
		hashtable["targetRank"] = DataCenter.User().PVP_SelectTarget.iRank;
		hashtable["battleResult"] = (int)(GameBattle.m_instance.GameState - 3);
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
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
