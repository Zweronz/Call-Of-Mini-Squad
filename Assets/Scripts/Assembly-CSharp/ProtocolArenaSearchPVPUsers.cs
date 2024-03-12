using System;
using System.Collections;
using LitJson;

public class ProtocolArenaSearchPVPUsers : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
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
			UtilUIArenaMainScreenInfo.gMyRank = int.Parse(jsonData["rank"].ToString());
			UtilUIArenaMainScreenInfo.gMyReward = int.Parse(jsonData["reward"].ToString());
			UtilUIArenaMainScreenInfo.gMyLeftRewardTime = long.Parse(jsonData["leftRewardTime"].ToString());
			JsonData jsonData2 = jsonData["users"];
			UIConstant.gLSTargetAreanData = new PVP_Target[jsonData2.Count];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				UIConstant.gLSTargetAreanData[i] = new PVP_Target();
				UIConstant.gLSTargetAreanData[i].sUUID = jsonData3["userId"].ToString();
				UIConstant.gLSTargetAreanData[i].sName = jsonData3["name"].ToString();
				UIConstant.gLSTargetAreanData[i].iRank = int.Parse(jsonData3["rank"].ToString());
				UIConstant.gLSTargetAreanData[i].iFaceIndex = int.Parse(jsonData3["leaderIndex"].ToString());
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
