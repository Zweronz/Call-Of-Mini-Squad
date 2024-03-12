using System;
using System.Collections;
using LitJson;

public class ProtocolChatGetMsgList : Protocol
{
	private string _language = string.Empty;

	public override string GetRequest()
	{
		_language = DataCenter.State().selectArenaRankTypeByLanguage;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["local"] = _language;
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
			UIConstant.gLsChatData.Clear();
			JsonData jsonData2 = jsonData["messages"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				ChatData chatData = new ChatData();
				chatData.userId = jsonData3["userId"].ToString();
				chatData.userName = jsonData3["userName"].ToString();
				chatData.userType = ChatData.GetUserType(jsonData3["userInfo"].ToString());
				chatData.msg = jsonData3["message"].ToString();
				chatData.dateSeconds = long.Parse(jsonData3["time"].ToString());
				UIConstant.gLsChatData.Add(chatData);
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
