using System;
using System.Collections;
using LitJson;

public class ProtocolAccountRemindEmailPassword : Protocol
{
	private string deviceid = string.Empty;

	private string uuid = string.Empty;

	private string email = string.Empty;

	private string password = string.Empty;

	public override string GetRequest()
	{
		email = UtilUIAccountManager.mInstance.accountDataState.email;
		Hashtable hashtable = new Hashtable();
		hashtable["email"] = email;
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
