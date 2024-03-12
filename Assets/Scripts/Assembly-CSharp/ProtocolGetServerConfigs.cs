using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ProtocolGetServerConfigs : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		ArrayList arrayList = new ArrayList();
		foreach (string key in DataCenter.Save().configVersion.Keys)
		{
			Hashtable hashtable2 = new Hashtable();
			hashtable2.Add("name", key);
			hashtable2.Add("version", DataCenter.Save().configVersion[key]);
			arrayList.Add(hashtable2);
		}
		hashtable.Add("versions", arrayList);
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
			JsonData jsonData2 = jsonData["result"];
			if (jsonData2.Count > 0)
			{
				CheckUpdateScript.s_instance.Phase = CheckUpdateScript.CheckPhase.DownLoading;
				CheckUpdateScript.s_instance.SetSliderBarSteps(jsonData2.Count);
				CheckUpdateScript.s_instance.SetSliderValue(0f);
			}
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				string key = jsonData3["name"].ToString();
				string value = jsonData3["version"].ToString();
				string value2 = jsonData3["content"].ToString();
				CheckUpdateScript.s_instance.needDownloadConfigList.Add(new KeyValuePair<string, string>(key, value2));
				DataCenter.Save().configVersion[key] = value;
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
