using System;
using System.Collections;
using LitJson;

public class ProtocolArenaLoadProfile : Protocol
{
	public string _uuid = string.Empty;

	public override string GetRequest()
	{
		_uuid = DataCenter.State().selectArenaTargetUUId;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = _uuid;
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
			JsonData jsonData2 = jsonData["heroes"];
			UIConstant.gLSTargetDetailData = new UtilUIArenaTeamDetailInfo.TargetPlayerDetailDataInfo[jsonData2.Count];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				UIConstant.gLSTargetDetailData[i] = new UtilUIArenaTeamDetailInfo.TargetPlayerDetailDataInfo();
				UIConstant.gLSTargetDetailData[i].teamName = jsonData["userName"].ToString();
				UIConstant.gLSTargetDetailData[i].teamCombat = int.Parse(jsonData["teamPower"].ToString());
				UIConstant.gLSTargetDetailData[i].userId = _uuid;
				UIConstant.gLSTargetDetailData[i].heroIndex = int.Parse(jsonData3["heroIndex"].ToString());
				UIConstant.gLSTargetDetailData[i].weaponStar = int.Parse(jsonData3["weaponStar"].ToString());
				UIConstant.gLSTargetDetailData[i].weaponLevel = int.Parse(jsonData3["weaponLevel"].ToString());
				UIConstant.gLSTargetDetailData[i].weaponMaxLevel = int.Parse(jsonData3["weaponMaxLevel"].ToString());
				UIConstant.gLSTargetDetailData[i].skillStar = int.Parse(jsonData3["skillStar"].ToString());
				UIConstant.gLSTargetDetailData[i].siteNum = int.Parse(jsonData3["siteNum"].ToString());
				UIConstant.gLSTargetDetailData[i].state = int.Parse(jsonData3["state"].ToString());
				UIConstant.gLSTargetDetailData[i].combat = int.Parse(jsonData3["power"].ToString());
				JsonData jsonData4 = jsonData3["helmetUseInfo"];
				UIConstant.gLSTargetDetailData[i].helmsEquipIndex = int.Parse(jsonData4["currEquipIndex"].ToString());
				UIConstant.gLSTargetDetailData[i].helmsEquipLevel = int.Parse(jsonData4["currEquipLevel"].ToString());
				JsonData jsonData5 = jsonData3["armorUseInfo"];
				UIConstant.gLSTargetDetailData[i].armorEquipIndex = int.Parse(jsonData5["currEquipIndex"].ToString());
				UIConstant.gLSTargetDetailData[i].armorEquipLevel = int.Parse(jsonData5["currEquipLevel"].ToString());
				JsonData jsonData6 = jsonData3["ornamentUseInfo"];
				UIConstant.gLSTargetDetailData[i].ornamentEquipIndex = int.Parse(jsonData6["currEquipIndex"].ToString());
				UIConstant.gLSTargetDetailData[i].ornamentEquipLevel = int.Parse(jsonData6["currEquipLevel"].ToString());
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
