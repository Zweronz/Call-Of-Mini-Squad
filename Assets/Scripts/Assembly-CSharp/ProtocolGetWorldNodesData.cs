using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
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
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
	
		string path = Application.persistentDataPath + "/saves/worldNodes.json";
				
		//if (!File.Exists(path))
		//{
			File.WriteAllText(path, JsonConvert.SerializeObject(new DummyProtocol(), Formatting.Indented));
		//}

		DummyProtocol dummyProtocol = JsonConvert.DeserializeObject<DummyProtocol>(File.ReadAllText(path));
		DataCenter.Save().SetWorldNodeProgress(dummyProtocol.worldNodes);

		return 0;
	}

	public class DummyProtocol
	{
		public int worldNodeIndex;

		public List<GameProgressData> worldNodes;

		public DummyProtocol()
		{
			worldNodes = new List<GameProgressData>();
			GameProgressData test = new GameProgressData();
			test.levelProgress[0] = 1;
			test.levelStars[Defined.LevelMode.Normal] = new ushort[30];
			worldNodes.Add(test);
		}
	}
}
