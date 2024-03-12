using System;
using System.Collections;
using LitJson;

public class ProtocolStoreGetIAPList : Protocol
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
			JsonData jsonData2 = jsonData["iaps"];
			UIConstant.gLSIAPItemsData = new UtilUIShopIAPItemData[jsonData2.Count];
			for (int i = 0; i < jsonData2.Count; i++)
			{
				JsonData jsonData3 = jsonData2[i];
				UIConstant.gLSIAPItemsData[i] = new UtilUIShopIAPItemData();
				UIConstant.gLSIAPItemsData[i].INDEX = i;
				UIConstant.gLSIAPItemsData[i].ID = jsonData3["id"].ToString();
				UIConstant.gLSIAPItemsData[i].CRYSTAL = int.Parse(jsonData3["crystal"].ToString());
				UIConstant.gLSIAPItemsData[i].MONEY = int.Parse(jsonData3["money"].ToString());
				UIConstant.gLSIAPItemsData[i].HERO = int.Parse(jsonData3["hero"].ToString());
				UIConstant.gLSIAPItemsData[i].PRICE = float.Parse(jsonData3["price"].ToString());
				UIConstant.gLSIAPItemsData[i].NAME = jsonData3["title"].ToString();
				UIConstant.gLSIAPItemsData[i].DESCRIBE = jsonData3["desc"].ToString();
				UIConstant.gLSIAPItemsData[i].ICONNAME = jsonData3["icon"].ToString();
				UIConstant.gLSIAPItemsData[i].LIMITCOUNT = int.Parse(jsonData3["purchaseCount"].ToString());
				UIConstant.gLSIAPItemsData[i].BACKGROUNDSTATE = int.Parse(jsonData3["bg"].ToString());
				if (UIConstant.gLSIAPItemsData[i].PRICE > 0f)
				{
					UIConstant.gLSIAPItemsData[i].COMMODITYISIAP = true;
				}
				else
				{
					UIConstant.gLSIAPItemsData[i].COMMODITYISIAP = false;
				}
			}
			return code;
		}
		catch (Exception)
		{
			return -1;
		}
	}
}
