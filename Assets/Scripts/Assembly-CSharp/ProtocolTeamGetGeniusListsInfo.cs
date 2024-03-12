using System;
using System.Collections;
using LitJson;

public class ProtocolTeamGetGeniusListsInfo : Protocol
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
			DataCenter.Save().teamAttributeSaveData = new DataSave.TeamAttributeSaveData();
			DataCenter.Save().teamAttributeSaveData.teamAttributeRemainingPoints = int.Parse(jsonData["remainingPoints"].ToString());
			DataCenter.Save().teamAttributeSaveData.teamAttributeAssignedPoint = int.Parse(jsonData["AssignedPoint"].ToString());
			DataCenter.Save().teamAttributeSaveData.teamAttributeExtraPoint = int.Parse(jsonData["extraPoint"].ToString());
			DataCenter.Save().teamAttributeSaveData.teamAttributeExtraPointMax = int.Parse(jsonData["extraPointMax"].ToString());
			DataCenter.Save().teamAttributeSaveData.teamAttributeExtraPointCost = int.Parse(jsonData["extraPointCost"].ToString());
			DataCenter.Save().teamAttributeSaveData.teamGeniusUnLocked = ((int.Parse(jsonData["geniusLock"].ToString()) == 0) ? true : false);
			DataCenter.Save().teamAttributeSaveData.teamEvolutionUnLocked = ((int.Parse(jsonData["evolutionLock"].ToString()) == 0) ? true : false);
			DataCenter.Save().teamAttributeSaveData.teamGeniusUnlockCondition = jsonData["geniusUnlockCondition"].ToString();
			DataCenter.Save().teamAttributeSaveData.teamEvolutionUnlockCondition = jsonData["evolutionUnlockCondition"].ToString();
			DataCenter.Save().teamAttributeSaveData.teamGeniusfreeResetTimes = int.Parse(jsonData["freeResetTimes"].ToString());
			DataCenter.Save().Money = int.Parse(jsonData["money"].ToString());
			DataCenter.Save().Crystal = int.Parse(jsonData["crystal"].ToString());
			DataCenter.Save().Honor = int.Parse(jsonData["honor"].ToString());
			JsonData jsonData2 = jsonData["geniusList"];
			DataCenter.Save().teamAttributeSaveData.teamAttributeTalent = new TeamAttributeData[jsonData2.Count];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				string s = jsonData3["index"].ToString();
				string s2 = jsonData3["state"].ToString();
				string s3 = jsonData3["level"].ToString();
				string s4 = jsonData3["maxLevel"].ToString();
				string s5 = jsonData3["unlockPoint"].ToString();
				string s6 = jsonData3["cost"].ToString();
				string s7 = jsonData3["unlockLevel"].ToString();
				Defined.COST_TYPE costType = GetCostType(jsonData3["costType"].ToString());
				TeamAttributeData teamAttributeData = new TeamAttributeData();
				teamAttributeData.index = int.Parse(s);
				teamAttributeData.state = (Defined.ItemState)int.Parse(s2);
				teamAttributeData.level = int.Parse(s3);
				teamAttributeData.maxLevel = int.Parse(s4);
				teamAttributeData.unlockPoint = int.Parse(s5);
				teamAttributeData.cost = int.Parse(s6);
				teamAttributeData.costType = costType;
				teamAttributeData.unlockLevel = int.Parse(s7);
				DataCenter.Save().teamAttributeSaveData.teamAttributeTalent[teamAttributeData.index] = teamAttributeData;
			}
			jsonData2 = jsonData["evolutionList"];
			DataCenter.Save().teamAttributeSaveData.teamAttributeEvolve = new TeamAttributeData[jsonData2.Count];
			for (int j = 0; j < jsonData2.Count; j++)
			{
				JsonData jsonData4 = jsonData2[j];
				string s8 = jsonData4["index"].ToString();
				string s9 = jsonData4["state"].ToString();
				string s10 = jsonData4["level"].ToString();
				string s11 = jsonData4["maxLevel"].ToString();
				string s12 = jsonData4["unlockPoint"].ToString();
				string s13 = jsonData4["cost"].ToString();
				Defined.COST_TYPE costType2 = GetCostType(jsonData4["costType"].ToString());
				TeamAttributeData teamAttributeData2 = new TeamAttributeData();
				teamAttributeData2.index = int.Parse(s8);
				teamAttributeData2.state = (Defined.ItemState)int.Parse(s9);
				teamAttributeData2.level = int.Parse(s10);
				teamAttributeData2.maxLevel = int.Parse(s11);
				teamAttributeData2.unlockPoint = int.Parse(s12);
				teamAttributeData2.cost = int.Parse(s13);
				teamAttributeData2.costType = costType2;
				DataCenter.Save().teamAttributeSaveData.teamAttributeEvolve[teamAttributeData2.index] = teamAttributeData2;
			}
			if (((IDictionary)jsonData).Contains((object)"resetGeniusCostCrystal"))
			{
				JsonData jsonData5 = jsonData["resetGeniusCostCrystal"];
				DataCenter.Save().teamAttributeSaveData.teamGeniusResetCostCrystalPerTimes = int.Parse(jsonData["resetGeniusCostCrystal"].ToString());
			}
			else
			{
				DataCenter.Save().teamAttributeSaveData.teamGeniusResetCostCrystalPerTimes = 20;
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
