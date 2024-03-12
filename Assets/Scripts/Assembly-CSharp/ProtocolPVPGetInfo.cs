using System;
using System.Collections;
using LitJson;

public class ProtocolPVPGetInfo : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.User().PVP_myData.sUUID;
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
			DataCenter.User().PVP_myData.iRank = (int)jsonData["rank"];
			DataCenter.User().PVP_myData.iPower = (int)jsonData["power"];
			DataCenter.User().PVP_myData.iNextActionTime = (int)jsonData["nextActionTime"];
			DataCenter.User().PVP_myData.iPowerCost = (int)jsonData["powerCost"];
			DataCenter.User().PVP_SettleTime = (int)jsonData["settleTime"];
			DataCenter.User().PVP_myData.iWin = (int)jsonData["win"];
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
