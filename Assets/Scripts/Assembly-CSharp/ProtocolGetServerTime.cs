using System;
using System.Collections;
using LitJson;
using UnityEngine;

public class ProtocolGetServerTime : Protocol
{
	public override string GetRequest()
	{
		Hashtable obj = new Hashtable();
		return JsonMapper.ToJson(obj);
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
			HttpRequestHandle.instance.serverTimeSeconds = (int)jsonData["serverTime"];
			HttpRequestHandle.instance.serverTimeSeconds -= Mathf.CeilToInt(Time.realtimeSinceStartup);
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
