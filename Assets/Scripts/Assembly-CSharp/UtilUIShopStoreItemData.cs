using System.Collections.Generic;
using UnityEngine;

public class UtilUIShopStoreItemData : MonoBehaviour
{
	protected int index = -1;

	protected string id = string.Empty;

	protected new string name = string.Empty;

	protected string iconName;

	protected List<KeyValuePair<Defined.COST_TYPE, int>> lsPrices = new List<KeyValuePair<Defined.COST_TYPE, int>>();

	protected int ownCount;

	protected string intro = string.Empty;

	protected bool bCanBuy = true;

	public int INDEX
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public string ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string NAME
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public string ICONNAME
	{
		get
		{
			return iconName;
		}
		set
		{
			iconName = value;
		}
	}

	public int OWNCOUNT
	{
		get
		{
			return ownCount;
		}
		set
		{
			ownCount = value;
		}
	}

	public List<KeyValuePair<Defined.COST_TYPE, int>> LSPRICES
	{
		get
		{
			return lsPrices;
		}
		set
		{
			lsPrices = value;
		}
	}

	public string INTRODUCE
	{
		get
		{
			return intro;
		}
		set
		{
			intro = value;
		}
	}

	public bool CANBUY
	{
		get
		{
			return bCanBuy;
		}
		set
		{
			bCanBuy = value;
		}
	}

	public void Init(int index, string id, string name, string iconName, List<KeyValuePair<Defined.COST_TYPE, int>> ls, int ownCount, string intro, bool bCanBuy)
	{
		INDEX = index;
		ID = id;
		NAME = name;
		LSPRICES.Clear();
		LSPRICES = ls;
		ICONNAME = iconName;
		OWNCOUNT = ownCount;
		INTRODUCE = intro;
		CANBUY = bCanBuy;
	}
}
