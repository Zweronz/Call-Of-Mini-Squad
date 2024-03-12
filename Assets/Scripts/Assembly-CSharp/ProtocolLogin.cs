using System;
using System.Collections;
using LitJson;
using UnityEngine;

public class ProtocolLogin : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["playTime"] = DataCenter.Save().lastLoginTime;
		hashtable["os"] = SystemInfo.operatingSystem;
		hashtable["device"] = "PC";
		hashtable["platform"] = Application.platform.ToString();
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
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
