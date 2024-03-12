using System;
using System.Collections;
using LitJson;

public class ProtocolDeleteMail : Protocol
{
	private string[] msgIds;

	public override string GetRequest()
	{
		msgIds = DataCenter.State().selectMailMessageIDS;
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
			for (int i = 0; i < msgIds.Length; i++)
			{
				UIConstant.glsMailReaded.Remove(msgIds[i]);
				UIConstant.gDictMailData.Remove(msgIds[i]);
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
