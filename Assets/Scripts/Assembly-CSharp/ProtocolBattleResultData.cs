using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ProtocolBattleResultData : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["worldIndex"] = DataCenter.State().selectWorldNode;
		hashtable["areaIndex"] = DataCenter.State().selectAreaNode;
		hashtable["areaMode"] = DataCenter.State().selectLevelMode;
		hashtable["battleResult"] = DataCenter.State().battleResult;
		hashtable["stars"] = DataCenter.State().battleStars;
		hashtable["addMoney"] = GameBattle.m_instance.getMoneyInBattle;
		hashtable["battleMode"] = ((!DataCenter.Save().squadMode) ? "normal" : "squad");
		hashtable["kills"] = GameBattle.m_instance.KillEnemiesCount;
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		GameBattle.m_instance.bGetBattleResultData = false;
		return JsonMapper.ToJson(hashtable);
	}

	public override int GetResponse(string response)
	{
		DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelStars[DataCenter.State().selectLevelMode][DataCenter.State().selectAreaNode] = (ushort)DataCenter.State().battleStars;
		//DataCenter.Save().GetTeamData().teamExp = int.Parse(jsonData["exp"].ToString());
		//DataCenter.Save().GetTeamData().teamLevel = int.Parse(jsonData["teamLevel"].ToString());
		//DataCenter.Save().GetTeamData().teamMaxExp = (int)jsonData["teamMaxExp"];
		//DataCenter.Save().Money = int.Parse(jsonData["money"].ToString());
		//DataCenter.Save().Crystal = int.Parse(jsonData["crystal"].ToString());
		//DataCenter.Save().Honor = int.Parse(jsonData["honor"].ToString());
		//DataCenter.Save().selectLevelDropData.exp = int.Parse(jsonData["rewardExp"].ToString());
		DataCenter.Save().selectLevelDropData.money = GameBattle.m_instance.getMoneyInBattle;//int.Parse(jsonData["rewardMoney"].ToString());
		//DataCenter.Save().selectLevelDropData.crystal = int.Parse(jsonData["rewardCrystal"].ToString());
		//DataCenter.Save().selectLevelDropData.honor = int.Parse(jsonData["rewardHonor"].ToString());
		//JsonData jsonData3 = jsonData["unlockInfos"];
		//for (int i = 0; i < jsonData3.Count; i++)
		//{
		//	string str = jsonData3[i].ToString();
		//	NewUnlockData.E_NewUnlockType typeByString = NewUnlockData.GetTypeByString(str);
		//	if (!UIConstant.gLsNewUnlockedInfo.Contains(typeByString))
		//	{
		//		UIConstant.gLsNewUnlockedInfo.Add(typeByString);
		//	}
		//}
		return 0;
	}
}
