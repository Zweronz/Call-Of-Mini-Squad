using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ProtocolGetLevelRewardData : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["worldIndex"] = DataCenter.State().selectWorldNode;
		hashtable["areaIndex"] = DataCenter.State().selectAreaNode;
		hashtable["areaMode"] = DataCenter.State().selectLevelMode;
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			JsonData jsonData2 = jsonData["code"];
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				return code;
			}
			if (DataCenter.Save().selectLevelDropData == null)
			{
				DataCenter.Save().selectLevelDropData = new LevelDropData();
			}
			if (DataCenter.Save().selectLevelDropData.stuffs == null)
			{
				DataCenter.Save().selectLevelDropData.stuffs = new List<StuffData>();
			}
			DataCenter.Conf().GetCurrentGameLevelData().level = (int)jsonData["monsterlevel"];
			DataCenter.Save().selectLevelDropData.money = (int)jsonData["money"];
			DataCenter.Save().selectLevelDropData.exp = (int)jsonData["exp"];
			DataCenter.Save().selectLevelDropData.honor = (int)jsonData["honor"];
			DataCenter.Save().selectLevelDropData.extraHonor = (int)jsonData["extraHonor"];
			DataCenter.Save().selectLevelDropData.extraMoney = (int)jsonData["extraMoney"];
			DataCenter.Save().selectLevelDropData.extraCrystal = (int)jsonData["extraCrystal"];
			DataCenter.Save().selectLevelDropData.recommendCombat = (int)jsonData["battlepoints"];
			JsonData jsonData3 = jsonData["stuffs"];
			for (int i = 0; i < jsonData3.Count; i++)
			{
				JsonData jsonData4 = jsonData3[i];
				StuffData stuffData = new StuffData();
				stuffData.index = (int)jsonData4["stuffIndex"];
				stuffData.count = (int)jsonData4["stuffCount"];
				DataCenter.Save().selectLevelDropData.stuffs.Add(stuffData);
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
