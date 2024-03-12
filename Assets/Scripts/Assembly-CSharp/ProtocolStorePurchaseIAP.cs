using System.Collections;
using LitJson;
using UnityEngine;

public class ProtocolStorePurchaseIAP : Protocol
{
	private string _iapID = string.Empty;

	private string _receipt = string.Empty;

	private string _transactionId = string.Empty;

	private string _signature = string.Empty;

	public override string GetRequest()
	{
		_iapID = DataCenter.State().selectIAPID;
		_receipt = DataCenter.State().selectIAPReceipt;
		_transactionId = DataCenter.State().selectIAPTransactionId;
		_signature = DataCenter.State().selectIAPSignature;
		Hashtable hashtable = new Hashtable();
		hashtable["userId"] = DataCenter.Save().uuid;
		hashtable["iapId"] = _iapID;
		hashtable["transactionId"] = _transactionId;
		hashtable["randPara"] = Random.Range(0, 99999).ToString();
		hashtable["sync"] = "1";
		hashtable["loginCode"] = DataCenter.Save().loginCode;
		hashtable["info"] = _receipt;
		hashtable["signature"] = _signature;
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
			int num = int.Parse(jsonData["heroIndex"].ToString());
			Defined.ItemState state = (Defined.ItemState)int.Parse(jsonData["heroStatus"].ToString());
			if (num != -1)
			{
				PlayerData playerData = DataCenter.Save().GetPlayerData(num);
				playerData.state = state;
			}
			for (int i = 0; i < UIConstant.gLSIAPItemsData.Length; i++)
			{
				if (UIConstant.gLSIAPItemsData[i].ID == _iapID)
				{
					UIConstant.gLSIAPItemsData[i].LIMITCOUNT = int.Parse(jsonData["purchaseCount"].ToString());
					break;
				}
			}
			JsonData jsonData2 = jsonData["unlockInfos"];
			for (int j = 0; j < jsonData2.Count; j++)
			{
				string str = jsonData2[j].ToString();
				NewUnlockData.E_NewUnlockType typeByString = NewUnlockData.GetTypeByString(str);
				if (!UIConstant.gLsNewUnlockedInfo.Contains(typeByString))
				{
					UIConstant.gLsNewUnlockedInfo.Add(typeByString);
				}
			}
			return code;
		}
		catch
		{
			return -1;
		}
	}
}
