﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public static class UpgradeController
{
	private static int GetCrystalBreak(int stars)
	{
		switch (stars)
		{
			case 2:
				return 20;

			case 3:
				return 30;

			case 4:
				return 40;

			default:
				return 10;
		}
	}

	private static int GetLevelBreak(int stars)
	{
		switch (stars)
		{
			case 2:
				return 15;

			case 3:
				return 20;

			case 4:
				return 25;

			default:
				return 10;
		}
	}

	public static void RefreshWeapons()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		foreach (PlayerData hero in DataCenter.Save().GetHeroList())
		{
			if (hero.weaponLevel == 21)
			{
				hero.upgradeData.weaponCanUpgrade = false;
			}
			else if ((hero.weaponLevel == 5 && hero.weaponStar == 1) || (hero.weaponLevel == 9 && hero.weaponStar == 2) || (hero.weaponLevel == 13 && hero.weaponStar == 3) || (hero.weaponLevel == 17 && hero.weaponStar == 4))
			{
				hero.upgradeData.weaponCanBk = true;

				if (DataCenter.Save().GetTeamData().teamLevel >= GetLevelBreak(hero.weaponLevel))
				{
					hero.upgradeData.weaponBreakCostMoney = UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0];
					hero.upgradeData.weaponUBreakCostCrystal = GetCrystalBreak(hero.weaponLevel);
				}
				else
				{
					hero.upgradeData.weaponBkTeamLevel = GetLevelBreak(hero.weaponStar);
				}
			}
		}
	}

	public static void RefreshSkills()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		foreach (PlayerData hero in DataCenter.Save().GetHeroList())
		{
			if (hero.skillLevel == 21)
			{
				hero.upgradeData.skillCanUpgrade = false;
			}
			else if ((hero.skillLevel == 5 && hero.skillStar == 1) || (hero.skillLevel == 9 && hero.skillStar == 2) || (hero.skillLevel == 13 && hero.skillStar == 3) || (hero.skillLevel == 17 && hero.skillStar == 4))
			{
				hero.upgradeData.skillCanBk = true;

				if (DataCenter.Save().GetTeamData().teamLevel >= GetLevelBreak(hero.skillLevel))
				{
					hero.upgradeData.skillBreakCostMoney = UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1];
					hero.upgradeData.skillBreakCostCrystal = GetCrystalBreak(hero.skillLevel);
				}
				else
				{
					hero.upgradeData.skillBkTeamLevel = GetLevelBreak(hero.skillStar);
				}
			}
		}
	}

	public static int UpgradeWeapon()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		PlayerData hero = DataCenter.Save().GetHeroList()[DataCenter.State().selectHeroIndex];

		if (DataCenter.Save().Money >= UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0] && hero.weaponLevel < 21)
		{
			DataCenter.Save().Money -= UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0];

			hero.weaponLevel++;
			hero.upgradeData.weaponCombat = UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][2];

			hero.upgradeData.weaponUpgradeCost = UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0];

			if (hero.weaponLevel >= 21)
			{
				hero.upgradeData.weaponCanUpgrade = false;

				hero.upgradeData.weaponUpgradeCost = 0;
			}

			HeroListController.Refresh();
				
			return 0;
		}
		if (hero.weaponLevel >= 21)
		{
			hero.upgradeData.weaponCanUpgrade = false;

			hero.upgradeData.weaponUpgradeCost = 0;
		}

		return -1;
	}

	public static int BreakWeapon()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		PlayerData hero = DataCenter.Save().GetHeroList()[DataCenter.State().selectHeroIndex];

		if (DataCenter.Save().Money >= UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0] && DataCenter.Save().Crystal >= GetCrystalBreak(hero.weaponStar))
		{
			DataCenter.Save().Money -= UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0];
			DataCenter.Save().Crystal -= GetCrystalBreak(hero.weaponStar);

			hero.weaponStar++;
			hero.upgradeData.weaponCanBk = false;

			hero.upgradeData.weaponCanUpgrade = true;
			hero.upgradeData.weaponUpgradeCost = UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][0];

			hero.upgradeData.weaponCombat = UpgradeCalcTest.upgrades[hero.weaponLevel + (hero.weaponStar - 1)][2];

			hero.upgradeData.weaponUBreakCostCrystal = 0;
			hero.upgradeData.weaponBreakCostMoney = 0;

			hero.weaponMaxLevel += 4;

			HeroListController.Refresh();

			return 0;
		}

		return -1;
	}

	public static int UpgradeSkill()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		PlayerData hero = DataCenter.Save().GetHeroList()[DataCenter.State().selectHeroIndex];

		if (DataCenter.Save().Money >= UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1] && hero.skillLevel < 21)
		{
			DataCenter.Save().Money -= UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1];

			hero.skillLevel++;
			hero.upgradeData.skillUpgradeCost = UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1];

			if (hero.skillLevel >= 21)
			{
				hero.upgradeData.skillCanUpgrade = false;

				hero.upgradeData.skillUpgradeCost = 0;
			}

			HeroListController.Refresh();
				
			return 0;
		}

		if (hero.skillLevel >= 21)
		{
			hero.upgradeData.skillCanUpgrade = false;

			hero.upgradeData.skillUpgradeCost = 0;
		}

		return -1;
	}

	public static int BreakSkill()
	{
		if (UpgradeCalcTest.upgrades == null)
		{
			UpgradeCalcTest.Init();
		}

		PlayerData hero = DataCenter.Save().GetHeroList()[DataCenter.State().selectHeroIndex];

		if (DataCenter.Save().Money >= UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1] && DataCenter.Save().Crystal >= GetCrystalBreak(hero.skillStar))
		{
			DataCenter.Save().Money -= UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1];
			DataCenter.Save().Crystal -= GetCrystalBreak(hero.skillStar);

			hero.skillStar++;
			hero.upgradeData.skillCanBk = false;

			hero.upgradeData.skillCanUpgrade = true;
			hero.upgradeData.skillUpgradeCost = UpgradeCalcTest.upgrades[hero.skillLevel + (hero.skillStar - 1)][1];

			hero.upgradeData.skillBreakCostCrystal = 0;
			hero.upgradeData.skillBreakCostMoney = 0;

			hero.skillMaxLevel += 4;

			HeroListController.Refresh();

			return 0;
		}
		
		return -1;
	}
}
