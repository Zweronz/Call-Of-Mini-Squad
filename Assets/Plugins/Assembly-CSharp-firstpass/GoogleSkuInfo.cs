using System.Collections.Generic;

public class GoogleSkuInfo
{
	public string title { get; private set; }

	public string price { get; private set; }

	public string type { get; private set; }

	public string description { get; private set; }

	public string productId { get; private set; }

	public GoogleSkuInfo(Dictionary<string, object> dict)
	{
		if (dict.ContainsKey("title"))
		{
			title = dict["title"] as string;
		}
		if (dict.ContainsKey("price"))
		{
			price = dict["price"] as string;
		}
		if (dict.ContainsKey("type"))
		{
			type = dict["type"] as string;
		}
		if (dict.ContainsKey("description"))
		{
			description = dict["description"] as string;
		}
		if (dict.ContainsKey("productId"))
		{
			productId = dict["productId"] as string;
		}
	}

	public static List<GoogleSkuInfo> fromList(List<object> items)
	{
		List<GoogleSkuInfo> list = new List<GoogleSkuInfo>();
		foreach (Dictionary<string, object> item in items)
		{
			list.Add(new GoogleSkuInfo(item));
		}
		return list;
	}

	public override string ToString()
	{
		return string.Format("<GoogleSkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}", title, price, type, description, productId);
	}
}
