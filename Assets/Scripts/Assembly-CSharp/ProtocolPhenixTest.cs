using System.Collections;
using LitJson;

public class ProtocolPhenixTest : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["money"] = UIConstant.MONEY.ToString();
		hashtable["honor"] = UIConstant.HORNOR.ToString();
		hashtable["exp"] = UIConstant.EXP.ToString();
		hashtable["crystal"] = UIConstant.CRYSTAL.ToString();
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
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
