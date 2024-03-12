using System;
using System.Collections;
using LitJson;

public class ProtocolAccountRegister : Protocol
{
	private string deviceid = string.Empty;

	private string uuid = string.Empty;

	private string email = string.Empty;

	private string password = string.Empty;

	public override string GetRequest()
	{
		email = UtilUIAccountManager.mInstance.accountDataState.email;
		password = UtilUIAccountManager.mInstance.accountDataState.password;
		deviceid = UtilUIAccountManager.mInstance.accountDataState.deviceid;
		Hashtable hashtable = new Hashtable();
		hashtable["deviceId"] = deviceid;
		hashtable["email"] = email;
		hashtable["password"] = password;
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
			UtilUIAccountManager.mInstance.accountData.uuid = jsonData["tid"].ToString();
			UtilUIAccountManager.mInstance.accountData.email = email;
			UtilUIAccountManager.mInstance.accountData.password = password;
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
