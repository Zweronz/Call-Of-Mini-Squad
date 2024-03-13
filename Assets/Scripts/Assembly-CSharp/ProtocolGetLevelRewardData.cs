using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class ProtocolGetLevelRewardData : Protocol
{
	public override string GetRequest()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["worldIndex"] = DataCenter.State().selectWorldNode;
		hashtable["areaIndex"] = DataCenter.State().selectAreaNode;
		hashtable["areaMode"] = DataCenter.State().selectLevelMode;
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

			level = dummyProtocol.monsterLevel;
			DataCenter.Save().selectLevelDropData = dummyProtocol.levelDropData;

			return 0;
	}

	public static int level;

	public class DummyProtocol
	{
		public int monsterLevel;

		public LevelDropData levelDropData;

		public DummyProtocol()
		{
			monsterLevel = 1;
			levelDropData = new LevelDropData();
		}
	}
}
