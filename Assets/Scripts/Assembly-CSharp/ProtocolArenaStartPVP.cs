using System;
using System.Collections;
using CoMDS2;
using LitJson;

public class ProtocolArenaStartPVP : Protocol
{
	public int _rank;

	public override string GetRequest()
	{
		_rank = DataCenter.State().selectArenaTargetRank;
		Hashtable hashtable = new Hashtable();
		hashtable["rank"] = _rank.ToString();
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
			if (DataCenter.User().PVP_SelectTarget.team == null)
			{
				DataCenter.User().PVP_SelectTarget.team = new TeamData();
			}
			DataCenter.User().PVP_SelectTarget.team.teamExp = (int)jsonData["teamExp"];
			DataCenter.User().PVP_SelectTarget.team.teamLevel = (int)jsonData["teamLevel"];
			JsonData jsonData2 = jsonData["geniusList"];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				DataCenter.User().PVP_SelectTarget.team.talents.Add((TeamSpecialAttribute.TeamAttributeType)int.Parse(jsonData3["index"].ToString()), int.Parse(jsonData3["level"].ToString()));
			}
			JsonData jsonData4 = jsonData["evolutionList"];
			for (int j = 0; j < jsonData4.Count; j++)
			{
				JsonData jsonData5 = jsonData4[j];
				DataCenter.User().PVP_SelectTarget.team.evolves.Add((TeamSpecialAttribute.TeamAttributeEvolveType)int.Parse(jsonData5["index"].ToString()), int.Parse(jsonData5["level"].ToString()));
			}
			JsonData jsonData6 = jsonData["heroes"];
			for (int k = 0; k < jsonData6.Count; k++)
			{
				JsonData jsonData7 = jsonData6[k];
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData = new PlayerData();
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.heroIndex = (int)jsonData7["heroIndex"];
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.weaponLevel = (int)jsonData7["weaponLevel"];
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.skillLevel = (int)jsonData7["skillLevel"];
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.siteNum = (int)jsonData7["siteNum"];
				JsonData jsonData8 = jsonData7["helmetUseInfo"];
				int currEquipIndex = int.Parse(jsonData8["currEquipIndex"].ToString());
				int currEquipLevel = int.Parse(jsonData8["currEquipLevel"].ToString());
				UserEquipData userEquipData = new UserEquipData();
				userEquipData.currEquipIndex = currEquipIndex;
				userEquipData.currEquipLevel = currEquipLevel;
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.equips.Add(Defined.EQUIP_TYPE.Head, userEquipData);
				JsonData jsonData9 = jsonData7["armorUseInfo"];
				int currEquipIndex2 = int.Parse(jsonData9["currEquipIndex"].ToString());
				int currEquipLevel2 = int.Parse(jsonData9["currEquipLevel"].ToString());
				UserEquipData userEquipData2 = new UserEquipData();
				userEquipData2.currEquipIndex = currEquipIndex2;
				userEquipData2.currEquipLevel = currEquipLevel2;
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.equips.Add(Defined.EQUIP_TYPE.Body, userEquipData2);
				JsonData jsonData10 = jsonData7["ornamentUseInfo"];
				int currEquipIndex3 = int.Parse(jsonData10["currEquipIndex"].ToString());
				int currEquipLevel3 = int.Parse(jsonData10["currEquipLevel"].ToString());
				UserEquipData userEquipData3 = new UserEquipData();
				userEquipData3.currEquipIndex = currEquipIndex3;
				userEquipData3.currEquipLevel = currEquipLevel3;
				DataCenter.User().PVP_SelectTarget.team.teamSitesData[k].playerData.equips.Add(Defined.EQUIP_TYPE.Acc, userEquipData3);
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
