using System;
using System.Collections;
using CoMDS2;
using LitJson;

public class ProtocolPVPUpload : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.User().PVP_myData.sUUID;
		hashtable["name"] = DataCenter.User().PVP_myData.sName;
		hashtable["faceIndex"] = DataCenter.Save().GetTeamSiteData(Defined.TEAM_SITE.TEAM_LEADER).playerData.heroIndex;
		hashtable["teamLevel"] = DataCenter.Save().GetTeamData().teamLevel;
		hashtable["honor"] = DataCenter.User().PVP_myData.iHonor;
		string content = JsonMapper.ToJson(DataCenter.Save().GetTeamData());
		string zipedcontent = string.Empty;
		Util.ZipString(content, ref zipedcontent);
		string value = Util.EncryptData(zipedcontent);
		hashtable["data"] = value;
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
			int iRank = (int)jsonData["rank"];
			int iPower = (int)jsonData["power"];
			int iNextActionTime = (int)jsonData["nextActionTime"];
			DataCenter.User().PVP_myData.iRank = iRank;
			DataCenter.User().PVP_myData.iPower = iPower;
			DataCenter.User().PVP_myData.iNextActionTime = iNextActionTime;
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
