using System.Collections;
using LitJson;

public class ProtocolStoreCrystalExchangMoney : Protocol
{
	private string _iapID = string.Empty;

	public override string GetRequest()
	{
		_iapID = DataCenter.State().selectIAPID;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["iapId"] = _iapID;
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
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return code;
		}
		catch
		{
			return -1;
		}
	}
}
