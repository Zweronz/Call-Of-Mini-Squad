using System.Collections;
using LitJson;

public class ProtocolCreateUser : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			JsonData jsonData2 = jsonData["code"];
			int code = Protocol.GetCode(jsonData);
			DataCenter.Save().loginCode = jsonData["loginCode"].ToString();
			if (code != 0)
			{
				return code;
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
