using System.Collections;
using LitJson;

public class ProtocolAccount : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["DeviceToken"] = DataCenter.Device().GetDeviceToken();
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
			string accountId = (string)jsonData["AccountId"];
			string accountToken = (string)jsonData["AccountToken"];
			DataCenter.Device().SetAccountId(accountId);
			DataCenter.Device().SetAccountToken(accountToken);
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	public string GetGameCenterAccountRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["GameCenterToken"] = DataCenter.Device().GetGameCenterToken();
		return JsonMapper.ToJson(hashtable);
	}

	public int GetGameCenterAccountResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				return code;
			}
			string gameCenterAccountId = (string)jsonData["AccountId"];
			DataCenter.Device().SetGameCenterAccountId(gameCenterAccountId);
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	public string BindGameCenterAccountRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["GameCenterToken"] = DataCenter.Device().GetGameCenterToken();
		hashtable["AccountId"] = DataCenter.Device().GetAccountId();
		hashtable["AccountToken"] = DataCenter.Device().GetAccountToken();
		return JsonMapper.ToJson(hashtable);
	}

	public int BindGameCenterAccountResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				return code;
			}
			string gameCenterAccountId = (string)jsonData["AccountId"];
			DataCenter.Device().SetGameCenterAccountId(gameCenterAccountId);
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	public string UseGameCenterAccountRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["DeviceToken"] = DataCenter.Device().GetDeviceToken();
		hashtable["GameCenterToken"] = DataCenter.Device().GetGameCenterToken();
		return JsonMapper.ToJson(hashtable);
	}

	public int UseGameCenterAccountResponse(string response)
	{
		return CommonResponse(response);
	}
}
