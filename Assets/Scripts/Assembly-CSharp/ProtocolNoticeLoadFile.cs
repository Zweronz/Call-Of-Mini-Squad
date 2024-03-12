using System.Collections;
using LitJson;

public class ProtocolNoticeLoadFile : Protocol
{
	private string _fileName = string.Empty;

	public override string GetRequest()
	{
		_fileName = DataCenter.State().selectNeedFileName;
		Hashtable hashtable = new Hashtable();
		hashtable["name"] = _fileName;
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
			string gStrFileContent = jsonData["content"].ToString();
			UIConstant.gStrFileContent = gStrFileContent;
			return code;
		}
		catch
		{
			return -1;
		}
	}
}
