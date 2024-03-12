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
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			JsonData jsonData2 = jsonData["code"];
			int code = Protocol.GetCode(jsonData);
			if (code != 0)
			{
				UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
				return code;
			}
			ushort num = ushort.Parse(jsonData["stars"].ToString());
			DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelStars[DataCenter.State().selectLevelMode][DataCenter.State().selectAreaNode] = num;
			DataCenter.Save().GetTeamData().teamExp = int.Parse(jsonData["exp"].ToString());
			DataCenter.Save().GetTeamData().teamLevel = int.Parse(jsonData["teamLevel"].ToString());
			DataCenter.Save().GetTeamData().teamMaxExp = (int)jsonData["teamMaxExp"];
			DataCenter.Save().Money = int.Parse(jsonData["money"].ToString());
			DataCenter.Save().Crystal = int.Parse(jsonData["crystal"].ToString());
			DataCenter.Save().Honor = int.Parse(jsonData["honor"].ToString());
			DataCenter.Save().selectLevelDropData.exp = int.Parse(jsonData["rewardExp"].ToString());
			DataCenter.Save().selectLevelDropData.money = int.Parse(jsonData["rewardMoney"].ToString());
			DataCenter.Save().selectLevelDropData.crystal = int.Parse(jsonData["rewardCrystal"].ToString());
			DataCenter.Save().selectLevelDropData.honor = int.Parse(jsonData["rewardHonor"].ToString());
			JsonData jsonData3 = jsonData["unlockInfos"];
			for (int i = 0; i < jsonData3.Count; i++)
			{
				string str = jsonData3[i].ToString();
				NewUnlockData.E_NewUnlockType typeByString = NewUnlockData.GetTypeByString(str);
				if (!UIConstant.gLsNewUnlockedInfo.Contains(typeByString))
				{
					UIConstant.gLsNewUnlockedInfo.Add(typeByString);
				}
			}
			GameBattle.m_instance.bGetBattleResultData = true;
			int num2 = (int)jsonData["worldNodeIndex"];
			JsonData jsonData4 = jsonData["worldNodes"];
			List<GameProgressData> list = new List<GameProgressData>();
			for (int j = 0; j < jsonData4.Count; j++)
			{
				JsonData jsonData5 = jsonData4[j];
				GameProgressData gameProgressData = new GameProgressData();
				gameProgressData.modeProgress = (ushort)(int)jsonData5["modeProgress"];
				gameProgressData.levelProgress[0] = (ushort)(int)jsonData5["normalProgress"];
				gameProgressData.levelProgress[1] = (ushort)(int)jsonData5["hardProgress"];
				gameProgressData.levelProgress[2] = (ushort)(int)jsonData5["hellProgress"];
				JsonData jsonData6 = jsonData5["normalStars"];
				gameProgressData.levelStars[Defined.LevelMode.Normal] = new ushort[jsonData6.Count];
				for (int k = 0; k < jsonData6.Count; k++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Normal][k] = (ushort)(int)jsonData6[k];
				}
				JsonData jsonData7 = jsonData5["hardStars"];
				gameProgressData.levelStars[Defined.LevelMode.Hard] = new ushort[jsonData7.Count];
				for (int l = 0; l < jsonData7.Count; l++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Hard][l] = (ushort)(int)jsonData7[l];
				}
				JsonData jsonData8 = jsonData5["hellStars"];
				gameProgressData.levelStars[Defined.LevelMode.Hell] = new ushort[jsonData8.Count];
				for (int m = 0; m < jsonData8.Count; m++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Hell][m] = (ushort)(int)jsonData8[m];
				}
				list.Add(gameProgressData);
			}
			DataCenter.Save().SetWorldNodeProgress(list);
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
