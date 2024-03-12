using System.Collections;
using LitJson;
using UnityEngine;

public class ProtocolStoreVerifyIAP : Protocol
{
	private string _iapID = string.Empty;

	private string _receipt = string.Empty;

	private string _transactionId = string.Empty;

	public override string GetRequest()
	{
		_iapID = DataCenter.State().selectIAPID;
		_receipt = DataCenter.State().selectIAPReceipt;
		_transactionId = DataCenter.State().selectIAPTransactionId;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["iapId"] = _iapID;
		hashtable["receipt"] = _receipt;
		hashtable["transactionId"] = _transactionId;
		hashtable["randPara"] = Random.Range(0, 99999).ToString();
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
			DataCenter.Save().Money = (int)jsonData["money"];
			DataCenter.Save().Crystal = (int)jsonData["crystal"];
			DataCenter.Save().Honor = (int)jsonData["honor"];
			return code;
		}
		catch
		{
			return -1;
		}
	}
}
