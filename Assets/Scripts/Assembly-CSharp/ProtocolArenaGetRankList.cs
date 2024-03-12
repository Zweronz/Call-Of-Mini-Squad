using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ProtocolArenaGetRankList : Protocol
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

	private Dictionary<int, PVP_RankTargetData> GetDict(string _str)
	{
		if (_str == "global")
		{
			return UIConstant.gDictRankGlobalTargetData;
		}
		return UIConstant.gDictRankLocalTargetData;
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
			GetDict(_language).Clear();
			JsonData jsonData2 = jsonData["ranks"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				PVP_RankTargetData pVP_RankTargetData = new PVP_RankTargetData();
				pVP_RankTargetData.userId = jsonData3["userId"].ToString();
				pVP_RankTargetData.userName = jsonData3["userName"].ToString();
				pVP_RankTargetData.rank = int.Parse(jsonData3["rank"].ToString());
				pVP_RankTargetData.teamLeaderIndex = int.Parse(jsonData3["teamLeaderIndex"].ToString());
				pVP_RankTargetData.teamLevel = int.Parse(jsonData3["teamLevel"].ToString());
				pVP_RankTargetData.rewardHonor = int.Parse(jsonData3["rewardHonor"].ToString());
				pVP_RankTargetData.rewardCrystal = int.Parse(jsonData3["rewardCrystal"].ToString());
				pVP_RankTargetData.rewardMoney = int.Parse(jsonData3["rewardMoney"].ToString());
				pVP_RankTargetData.rewardPoint = int.Parse(jsonData3["point"].ToString());
				GetDict(_language).Add(pVP_RankTargetData.rank, pVP_RankTargetData);
			}
			if (((IDictionary)jsonData).Contains((object)"myRank"))
			{
				JsonData jsonData4 = jsonData["myRank"];
				PVP_RankTargetData pVP_RankTargetData2 = new PVP_RankTargetData();
				pVP_RankTargetData2.userId = jsonData4["userId"].ToString();
				pVP_RankTargetData2.userName = jsonData4["userName"].ToString();
				pVP_RankTargetData2.rank = int.Parse(jsonData4["rank"].ToString());
				pVP_RankTargetData2.teamLeaderIndex = int.Parse(jsonData4["teamLeaderIndex"].ToString());
				pVP_RankTargetData2.teamLevel = int.Parse(jsonData4["teamLevel"].ToString());
				pVP_RankTargetData2.rewardHonor = int.Parse(jsonData4["rewardHonor"].ToString());
				pVP_RankTargetData2.rewardCrystal = int.Parse(jsonData4["rewardCrystal"].ToString());
				pVP_RankTargetData2.rewardMoney = int.Parse(jsonData4["rewardMoney"].ToString());
				pVP_RankTargetData2.rewardPoint = int.Parse(jsonData4["point"].ToString());
				UIConstant.gMyRankDataInfo = pVP_RankTargetData2;
			}
			else
			{
				UIConstant.gMyRankDataInfo = null;
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
