using System;
using System.Collections;
using LitJson;

public class ProtocolPVPGetTops : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
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
			JsonData jsonData2 = jsonData["tops"];
			DataCenter.User().PVP_TopList.Clear();
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				PVP_Target pVP_Target = new PVP_Target();
				pVP_Target.sUUID = (string)jsonData3["userId"];
				pVP_Target.sName = (string)jsonData3["name"];
				pVP_Target.iFaceIndex = (int)jsonData3["faceIndex"];
				pVP_Target.iTeamLevel = (int)jsonData3["teamLevel"];
				pVP_Target.iHonor = (int)jsonData3["honor"];
				pVP_Target.iPower = (int)jsonData3["power"];
				pVP_Target.iNextActionTime = (int)jsonData3["nextActionTime"];
				pVP_Target.iPowerCost = (int)jsonData3["powerCost"];
				DataCenter.User().PVP_TopList.Add(pVP_Target);
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
