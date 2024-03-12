using CoMDS2;
using UnityEngine;

public class UtilUIStandbyPlayerAppearanceData : MonoBehaviour
{
	protected PlayerData playData;

	protected DataConf.HeroData heroConf;

	protected GameObject modelGO;

	public PlayerData PLAYERDATA
	{
		get
		{
			return playData;
		}
	}

	public DataConf.HeroData HEROCONF
	{
		get
		{
			return heroConf;
		}
	}

	public int ID
	{
		get
		{
			return playData.heroIndex;
		}
	}

	public int INDEX
	{
		get
		{
			return heroConf.index;
		}
	}

	public int SITEINDEX
	{
		get
		{
			return playData.siteNum;
		}
	}

	public string NAME
	{
		get
		{
			return heroConf.name;
		}
	}

	public Defined.ItemState STATE
	{
		get
		{
			return playData.state;
		}
	}

	public int INACTIVEMONEY
	{
		get
		{
			return playData.unlockCost;
		}
	}

	public int UNLOCKLEVEL
	{
		get
		{
			return playData.unlockNeedTeamLevel;
		}
	}

	public string MODELFILENAME
	{
		get
		{
			return heroConf.modelFileName;
		}
	}

	public Weapon.WeaponType WEAPONTYPE
	{
		get
		{
			return heroConf.weaponType;
		}
	}

	public GameObject MODELGO
	{
		get
		{
			return modelGO;
		}
		set
		{
			modelGO = value;
		}
	}

	public Player.CharacterType CHARACTERTYPE
	{
		get
		{
			return heroConf.characterType;
		}
	}

	public void Init(PlayerData pd, DataConf.HeroData heroConf)
	{
		playData = pd;
		this.heroConf = heroConf;
	}
}
