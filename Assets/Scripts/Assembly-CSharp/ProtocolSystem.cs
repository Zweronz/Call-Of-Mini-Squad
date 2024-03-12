using System.Collections;
using LitJson;

public class ProtocolSystem : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["Game"] = "TestCoC";
		hashtable["Version"] = "1.0";
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
			JsonData jsonData2 = jsonData["Servers"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				string server = (string)jsonData3["Name"];
				string url = (string)jsonData3["Addr"];
				HttpClient.Instance().AddServer(server, url, 10f, "0123456789");
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
