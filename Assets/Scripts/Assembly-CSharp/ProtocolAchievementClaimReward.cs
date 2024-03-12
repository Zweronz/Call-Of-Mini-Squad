using System;
using System.Collections;
using LitJson;

public class ProtocolAchievementClaimReward : Protocol
{
	private string _id = string.Empty;

	public override string GetRequest()
	{
		_id = DataCenter.State().selectAchievementId;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["questId"] = _id;
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
			if (((IDictionary)jsonData).Contains((object)"nextQuest"))
			{
				JsonData jsonData2 = jsonData["nextQuest"];
				AchievementData achievementData = new AchievementData();
				achievementData.id = jsonData2["questId"].ToString();
				achievementData.site = UIConstant.gDictAchievementData[_id].site;
				achievementData.title = jsonData2["name"].ToString();
				achievementData.des = jsonData2["desc"].ToString();
				achievementData.money = int.Parse(jsonData2["money"].ToString());
				achievementData.crystal = int.Parse(jsonData2["crystal"].ToString());
				achievementData.honor = int.Parse(jsonData2["honor"].ToString());
				achievementData.hero = int.Parse(jsonData2["hero"].ToString());
				achievementData.state = int.Parse(jsonData2["state"].ToString());
				achievementData.scheduleMin = int.Parse(jsonData2["process"].ToString());
				achievementData.scheduleMax = int.Parse(jsonData2["maxProcess"].ToString());
				achievementData.bDaily = ((int.Parse(jsonData2["daily"].ToString()) != 0) ? true : false);
				UIConstant.gDictAchievementData.Remove(_id);
				UIConstant.gDictAchievementData.Add(achievementData.id, achievementData);
			}
			else
			{
				UIConstant.gDictAchievementData.Remove(_id);
			}
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
