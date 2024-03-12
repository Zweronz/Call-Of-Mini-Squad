using System;
using System.Collections;
using LitJson;

public class ProtocolAchievementGetList : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
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
			UIConstant.gDictAchievementData.Clear();
			JsonData jsonData2 = jsonData["quests"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				AchievementData achievementData = new AchievementData();
				achievementData.id = jsonData3["questId"].ToString();
				achievementData.site = i;
				achievementData.title = jsonData3["name"].ToString();
				achievementData.des = jsonData3["desc"].ToString();
				achievementData.money = int.Parse(jsonData3["money"].ToString());
				achievementData.crystal = int.Parse(jsonData3["crystal"].ToString());
				achievementData.honor = int.Parse(jsonData3["honor"].ToString());
				achievementData.hero = int.Parse(jsonData3["hero"].ToString());
				achievementData.state = int.Parse(jsonData3["state"].ToString());
				achievementData.scheduleMin = int.Parse(jsonData3["process"].ToString());
				achievementData.scheduleMax = int.Parse(jsonData3["maxProcess"].ToString());
				achievementData.bDaily = ((int.Parse(jsonData3["daily"].ToString()) != 0) ? true : false);
				UIConstant.gDictAchievementData.Add(achievementData.id, achievementData);
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
