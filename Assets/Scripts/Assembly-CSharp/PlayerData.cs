using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
	public int heroIndex;

	public int level;

	public int exp;

	public int weaponLevel;

	public int weaponMaxLevel;

	public int weaponStar;

	public int skillLevel;

	public int skillMaxLevel;

	public int skillStar;

	public int siteNum;

	public bool isNew;

	public Dictionary<Defined.EQUIP_TYPE, UserEquipData> equips;

	public UpgradeData upgradeData;

	public Defined.ItemState state;

	public int unlockNeedTeamLevel;

	public int unlockCost;

	public Defined.COST_TYPE costType;

	public int combat;

	public int Hp
	{
		get
		{
			return CalacHp(heroIndex, equips);
		}
	}

	public float Damage
	{
		get
		{
			return CalacDamage(heroIndex, weaponLevel, weaponStar, weaponMaxLevel, equips);
		}
	}

	public float Defense
	{
		get
		{
			return CalacDefense(heroIndex, equips);
		}
	}

	public int Hit
	{
		get
		{
			return CalacHit(heroIndex, equips);
		}
	}

	public int CritRate
	{
		get
		{
			return CalacCritRate(heroIndex, equips);
		}
	}

	public int Dodge
	{
		get
		{
			return CalacDodge(heroIndex, equips);
		}
	}

	public float Speed
	{
		get
		{
			return CalacSpeed(heroIndex, equips);
		}
	}

	public int Combat
	{
		get
		{
			return combat;
		}
	}

	public PlayerData()
	{
		level = 1;
		equips = new Dictionary<Defined.EQUIP_TYPE, UserEquipData>();
		upgradeData = null;
		weaponLevel = 1;
		skillLevel = 1;
		siteNum = -1;
		isNew = true;
		exp = 0;
		combat = 0;
	}

	public static List<KeyValuePair<int, int>> EquipDataDictToList(Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
		foreach (UserEquipData value in dict.Values)
		{
			if (value != null)
			{
				list.Add(new KeyValuePair<int, int>(value.currEquipIndex, value.currEquipLevel));
			}
		}
		return list;
	}

	public static int CalacHp(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacHp(_heroIndex, ls);
	}

	public static int CalacHp(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		int hp = heroDataByIndex.hp;
		float num = 0f;
		float num2 = 0f;
		foreach (KeyValuePair<int, int> l in ls)
		{
			num += (float)DataCenter.User().GetEquipHealth(l.Key, l.Value);
			num2 += DataCenter.User().GetEquipHealthPercent(l.Key, l.Value);
		}
		return Mathf.CeilToInt((float)(hp + Mathf.CeilToInt(num * heroDataByIndex.equipAdditionPercent)) * (1f + num2));
	}

	public static float CalacDamage(int _heroIndex, int _weaponLevel, int _weaponStar, int _levelMax, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacDamage(_heroIndex, _weaponLevel, _weaponStar, _levelMax, ls);
	}

	public static float CalacDamage(int _heroIndex, int _weaponLevel, int _weaponStar, int _levelMax, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(heroDataByIndex.weaponType);
		float damage = weaponDataByType.GetDamage(_weaponLevel, _weaponStar, _levelMax);
		float num = 0f;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.atkPercent;
		}
		return damage + damage * num;
	}

	public static float CalacDefense(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacDefense(_heroIndex, ls);
	}

	public static float CalacDefense(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		float num = 0f;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.def;
		}
		return num;
	}

	public static int CalacHit(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacHit(_heroIndex, ls);
	}

	public static int CalacHit(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		int num = heroDataByIndex.hitRate;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.proHit;
		}
		return num;
	}

	public static int CalacCritRate(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacCritRate(_heroIndex, ls);
	}

	public static int CalacCritRate(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		int num = heroDataByIndex.proCritical;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.proCrit;
		}
		return num;
	}

	public static int CalacDodge(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacDodge(_heroIndex, ls);
	}

	public static int CalacDodge(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		int num = heroDataByIndex.dodge;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.proDodge;
		}
		return num;
	}

	public static float CalacSpeed(int _heroIndex, Dictionary<Defined.EQUIP_TYPE, UserEquipData> dict)
	{
		List<KeyValuePair<int, int>> ls = EquipDataDictToList(dict);
		return CalacSpeed(_heroIndex, ls);
	}

	public static float CalacSpeed(int _heroIndex, List<KeyValuePair<int, int>> ls)
	{
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(_heroIndex);
		float moveSpeed = heroDataByIndex.moveSpeed;
		float num = 0f;
		foreach (KeyValuePair<int, int> l in ls)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(l.Key);
			num += equipDataByIndex.moveSpeedPercent;
		}
		return moveSpeed * (1f + num);
	}
}
