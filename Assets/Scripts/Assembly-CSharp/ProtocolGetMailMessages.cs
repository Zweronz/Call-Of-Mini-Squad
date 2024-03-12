using System;
using System.Collections;
using LitJson;

public class ProtocolGetMailMessages : Protocol
{
	private string[] msgIds;

	public override string GetRequest()
	{
		msgIds = new string[UIConstant.glsMailReaded.Count];
		for (int i = 0; i < UIConstant.glsMailReaded.Count; i++)
		{
			msgIds[i] = UIConstant.glsMailReaded[i];
		}
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
			UIConstant.gMaxMailCount = int.Parse(jsonData["max"].ToString());
			UIConstant.gDictMailData.Clear();
			UIConstant.glsMailReaded.Clear();
			JsonData jsonData2 = jsonData["messages"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				MailData mailData = new MailData();
				mailData.index = i;
				mailData.msgID = jsonData3["msgId"].ToString();
				mailData.titel = jsonData3["title"].ToString();
				mailData.sender = jsonData3["sender"].ToString();
				mailData.content = jsonData3["content"].ToString();
				mailData.dateSeconds = long.Parse(jsonData3["time"].ToString());
				mailData.actionReply = int.Parse(jsonData3["actionReply"].ToString());
				mailData.read = ((int.Parse(jsonData3["read"].ToString()) != 0) ? true : false);
				string text = jsonData3["items"].ToString();
				string[] array = text.Split(',');
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(':');
					if (array2[0].Contains("honor"))
					{
						mailData.dictItems.Add(Defined.COST_TYPE.Honor, int.Parse(array2[1]));
					}
					else if (array2[0].Contains("crystal"))
					{
						mailData.dictItems.Add(Defined.COST_TYPE.Crystal, int.Parse(array2[1]));
					}
					else if (array2[0].Contains("money"))
					{
						mailData.dictItems.Add(Defined.COST_TYPE.Money, int.Parse(array2[1]));
					}
					else if (array2[0].Contains("exp"))
					{
						mailData.dictItems.Add(Defined.COST_TYPE.Exp, int.Parse(array2[1]));
					}
				}
				if (mailData.read)
				{
					UIConstant.glsMailReaded.Add(mailData.msgID);
				}
				UIConstant.gDictMailData.Add(mailData.msgID, mailData);
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
