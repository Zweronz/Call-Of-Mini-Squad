public class UtilUIShopIAPItemData
{
	protected int index = -1;

	protected string id = string.Empty;

	protected string name = string.Empty;

	protected string iconName;

	protected int crystal;

	protected int money;

	protected int hero = -1;

	protected int limitCount = -1;

	protected float price;

	protected string describe = string.Empty;

	protected int backgroundState = 1;

	protected bool commodityTypeIsIAP = true;

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

	public int CRYSTAL
	{
		get
		{
			return crystal;
		}
		set
		{
			crystal = value;
		}
	}

	public int MONEY
	{
		get
		{
			return money;
		}
		set
		{
			money = value;
		}
	}

	public int HERO
	{
		get
		{
			return hero;
		}
		set
		{
			hero = value;
		}
	}

	public int LIMITCOUNT
	{
		get
		{
			return limitCount;
		}
		set
		{
			limitCount = value;
		}
	}

	public float PRICE
	{
		get
		{
			return price;
		}
		set
		{
			price = value;
		}
	}

	public string DESCRIBE
	{
		get
		{
			return describe;
		}
		set
		{
			describe = value;
		}
	}

	public int BACKGROUNDSTATE
	{
		get
		{
			return backgroundState;
		}
		set
		{
			backgroundState = value;
		}
	}

	public bool COMMODITYISIAP
	{
		get
		{
			return commodityTypeIsIAP;
		}
		set
		{
			commodityTypeIsIAP = value;
		}
	}
}
