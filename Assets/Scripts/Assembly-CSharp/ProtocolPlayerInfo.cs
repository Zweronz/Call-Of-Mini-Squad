using System.Collections;
using LitJson;

public class ProtocolPlayerInfo : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["AccountId"] = DataCenter.Device().GetAccountId();
		hashtable["AccountToken"] = DataCenter.Device().GetAccountToken();
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int num = (int)jsonData["Code"];
			if (num != 0)
			{
				return num;
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

	public string SetPlayerInfoRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["AccountId"] = DataCenter.Device().GetAccountId();
		hashtable["AccountToken"] = DataCenter.Device().GetAccountToken();
		hashtable["Foo"] = "Bar2";
		return JsonMapper.ToJson(hashtable);
	}

	public int SetPlayerInfoResponse(string response)
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
}
