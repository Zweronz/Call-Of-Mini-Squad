using System;
using System.Collections;
using LitJson;

public class ProtocolReadMails : Protocol
{
	private string[] msgIds;

	public override string GetRequest()
	{
		msgIds = new string[UIConstant.glsMailReaded.Count];
		for (int i = 0; i < UIConstant.glsMailReaded.Count; i++)
		{
			msgIds[i] = UIConstant.glsMailReaded[i];
		}
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["msgIds"] = msgIds;
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
