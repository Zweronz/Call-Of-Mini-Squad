using System;
using System.Collections;
using LitJson;

public class ProtocolHeartBeat : Protocol
{
	private string _language = string.Empty;

	public override string GetRequest()
	{
		_language = DataCenter.State().selectArenaRankTypeByLanguage;
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
			UIConstant.gProbeNewAchievementCount = int.Parse(jsonData["newAchievement"].ToString());
			UIConstant.gProbeUnreadMsgsCount = int.Parse(jsonData["unreadMsgs"].ToString());
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
