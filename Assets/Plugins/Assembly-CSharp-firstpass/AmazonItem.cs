using System.Collections;
using System.Collections.Generic;

public class AmazonItem
{
	public string description;

	public string type;

	public string price;

	public string sku;

	public string smallIconUrl;

	public string title;

	public AmazonItem(Hashtable ht)
	{
		description = ht["description"].ToString();
		type = ht["type"].ToString();
		price = ht["price"].ToString();
		sku = ht["sku"].ToString();
		smallIconUrl = ht["smallIconUrl"].ToString();
		title = ht["title"].ToString();
	}

	public static List<AmazonItem> fromArrayList(ArrayList array)
	{
		List<AmazonItem> list = new List<AmazonItem>();
		foreach (Hashtable item in array)
		{
			list.Add(new AmazonItem(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<AmazonItem> type: {0}, sku: {1}, price: {2}, title: {3}, description: {4}", type, sku, price, title, description);
	}
}
