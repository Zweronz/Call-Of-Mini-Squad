using System;
using System.Collections;
using LitJson;

public class ProtocolTeamEquipHero : Protocol
{
	private int _index;

	private int _teamSite;

	public override string GetRequest()
	{
		_index = DataCenter.State().selectHeroIndex;
		_teamSite = DataCenter.State().selectTeamSiteIndex;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["currHeroInBagList"] = DataCenter.Save().GetPlayerData(_index).heroIndex.ToString();
		hashtable["siteNum"] = _teamSite.ToString();
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
			DataCenter.State().selectTeamSiteIndex = int.Parse(jsonData["siteNum"].ToString());
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
