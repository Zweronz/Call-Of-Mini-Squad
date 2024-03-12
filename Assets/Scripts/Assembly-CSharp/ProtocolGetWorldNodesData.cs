using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ProtocolGetWorldNodesData : Protocol
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
			int num = (int)jsonData["worldNodeIndex"];
			JsonData jsonData2 = jsonData["worldNodes"];
			List<GameProgressData> list = new List<GameProgressData>();
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				GameProgressData gameProgressData = new GameProgressData();
				gameProgressData.modeProgress = (ushort)(int)jsonData3["modeProgress"];
				gameProgressData.levelProgress[0] = (ushort)(int)jsonData3["normalProgress"];
				gameProgressData.levelProgress[1] = (ushort)(int)jsonData3["hardProgress"];
				gameProgressData.levelProgress[2] = (ushort)(int)jsonData3["hellProgress"];
				JsonData jsonData4 = jsonData3["normalStars"];
				gameProgressData.levelStars[Defined.LevelMode.Normal] = new ushort[jsonData4.Count];
				for (int j = 0; j < jsonData4.Count; j++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Normal][j] = (ushort)(int)jsonData4[j];
				}
				JsonData jsonData5 = jsonData3["hardStars"];
				gameProgressData.levelStars[Defined.LevelMode.Hard] = new ushort[jsonData5.Count];
				for (int k = 0; k < jsonData5.Count; k++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Hard][k] = (ushort)(int)jsonData5[k];
				}
				JsonData jsonData6 = jsonData3["hellStars"];
				gameProgressData.levelStars[Defined.LevelMode.Hell] = new ushort[jsonData6.Count];
				for (int l = 0; l < jsonData6.Count; l++)
				{
					gameProgressData.levelStars[Defined.LevelMode.Hell][l] = (ushort)(int)jsonData6[l];
				}
				list.Add(gameProgressData);
			}
			DataCenter.Save().SetWorldNodeProgress(list);
			return 0;
		}
		catch
		{
			return -1;
		}
	}
}
