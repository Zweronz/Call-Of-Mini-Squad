using System;
using System.Collections;
using LitJson;

public class ProtocolAccountDeviceLogin : Protocol
{
	private string deviceid = string.Empty;

	private string uuid = string.Empty;

	private string email = string.Empty;

	private string password = string.Empty;

	public override string GetRequest()
	{
		deviceid = UtilUIAccountManager.mInstance.accountDataState.deviceid;
		Hashtable hashtable = new Hashtable();
		hashtable["deviceId"] = deviceid;
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
			uuid = jsonData["tid"].ToString();
			email = jsonData["email"].ToString();
			password = jsonData["password"].ToString();
			if (UtilUIAccountManager.mInstance.accountData.deviceAccountInfo == null)
			{
				UtilUIAccountManager.mInstance.accountData.deviceAccountInfo = new UtilAccountData.TAccountInfo();
			}
			UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.deviceid = deviceid;
			UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.uuid = uuid;
			UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.email = email;
			UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.password = password;
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
