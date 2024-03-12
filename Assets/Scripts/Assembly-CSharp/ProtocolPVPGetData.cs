using System;
using System.Collections;
using CoMDS2;
using LitJson;

public class ProtocolPVPGetData : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.User().PVP_SelectTarget.sUUID;
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
			string input_data = (string)jsonData["data"];
			string content = Util.DecryptData(input_data);
			string unzipedcontent = string.Empty;
			Util.UnZipString(content, ref unzipedcontent);
			JsonData jsonData2 = JsonMapper.ToObject(unzipedcontent);
			if (DataCenter.User().PVP_SelectTarget.team == null)
			{
				DataCenter.User().PVP_SelectTarget.team = new TeamData();
			}
			DataCenter.User().PVP_SelectTarget.team.teamExp = (int)jsonData2["teamExp"];
			DataCenter.User().PVP_SelectTarget.team.teamLevel = (int)jsonData2["teamLevel"];
			JsonData jsonData3 = jsonData2["teamSitesData"];
			for (int i = 0; i < jsonData3.Count; i++)
			{
				JsonData jsonData4 = jsonData3[i];
				JsonData jsonData5 = jsonData4["playerData"];
				if (jsonData5 != null)
				{
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData = new PlayerData();
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.heroIndex = (int)jsonData5["heroIndex"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.level = (int)jsonData5["level"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.exp = (int)jsonData5["exp"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.weaponLevel = (int)jsonData5["weaponLevel"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.skillLevel = (int)jsonData5["skillLevel"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.siteNum = (int)jsonData5["siteNum"];
					DataCenter.User().PVP_SelectTarget.team.teamSitesData[i].playerData.isNew = (bool)jsonData5["isNew"];
				}
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
