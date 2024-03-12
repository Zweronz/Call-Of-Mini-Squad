using System.Collections;
using LitJson;

public class ProtocolStroreGetShopList : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.User().PVP_myData.sUUID;
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
			JsonData jsonData2 = jsonData["shopItems"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				string text = jsonData3["id"].ToString();
				string text2 = jsonData3["m"].ToString();
				string text3 = jsonData3["g"].ToString();
				string text4 = jsonData3["h"].ToString();
				string text5 = jsonData3["c"].ToString();
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
