using System;
using System.Collections;
using LitJson;

public class ProtocolProbeMails : Protocol
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
			UIConstant.gProbeNewMailsCount = int.Parse(jsonData["newMessages"].ToString());
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
