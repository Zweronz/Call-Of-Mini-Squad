using System;
using System.Collections;
using LitJson;

public class ProtocolReceiveMailAccessory : Protocol
{
	private string[] msgIds;

	public override string GetRequest()
	{
		msgIds = DataCenter.State().selectMailMessageIDS;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["msgIds"] = msgIds;
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
			UIConstant.gDictMailRewardStatistics.Clear();
			for (int i = 0; i < msgIds.Length; i++)
			{
				UIConstant.gDictMailData[msgIds[i]].actionReply = 1;
				UIConstant.gDictMailData[msgIds[i]].read = true;
			}
			if (((IDictionary)jsonData).Contains((object)"items"))
			{
				string text = jsonData["items"].ToString();
				string[] array = text.Split(',');
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(':');
					if (array2[0].Contains("honor"))
					{
						UIConstant.gDictMailRewardStatistics.Add(Defined.COST_TYPE.Honor, int.Parse(array2[1]));
					}
					else if (array2[0].Contains("crystal"))
					{
						UIConstant.gDictMailRewardStatistics.Add(Defined.COST_TYPE.Crystal, int.Parse(array2[1]));
					}
					else if (array2[0].Contains("money"))
					{
						UIConstant.gDictMailRewardStatistics.Add(Defined.COST_TYPE.Money, int.Parse(array2[1]));
					}
				}
			}
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
