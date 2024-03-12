using System.Collections;
using LitJson;

public class ProtocolTeamUnlockGenius : Protocol
{
	private int _index = -1;

	public override string GetRequest()
	{
		_index = DataCenter.State().selectTalentIndex;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["index"] = _index.ToString();
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
			DataCenter.Save().teamAttributeSaveData.teamAttributeTalent[_index].state = Defined.ItemState.Available;
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
