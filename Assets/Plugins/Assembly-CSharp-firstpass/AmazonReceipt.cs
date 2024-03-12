using System.Collections;
using System.Collections.Generic;

public class AmazonReceipt
{
	public string type;

	public string token;

	public string sku;

	public string subscriptionStartDate;

	public string subscriptionEndDate;

	public AmazonReceipt(Hashtable ht)
	{
		type = ht["type"].ToString();
		token = ht["token"].ToString();
		sku = ht["sku"].ToString();
		if (ht.ContainsKey("subscriptionStartDate"))
		{
			subscriptionStartDate = ht["subscriptionStartDate"].ToString();
		}
		if (ht.ContainsKey("subscriptionEndDate"))
		{
			subscriptionStartDate = ht["subscriptionEndDate"].ToString();
		}
	}

	public static List<AmazonReceipt> fromArrayList(ArrayList array)
	{
		List<AmazonReceipt> list = new List<AmazonReceipt>();
		foreach (Hashtable item in array)
		{
			list.Add(new AmazonReceipt(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<AmazonReceipt> type: {0}, token: {1}, sku: {2}", type, token, sku);
	}
}
