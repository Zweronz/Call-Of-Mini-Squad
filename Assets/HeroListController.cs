using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class HeroListController
{
	public static void Refresh()
	{
		UpgradeController.RefreshWeapons();
		UpgradeController.RefreshSkills();

		foreach (PlayerData hero in DataCenter.Save().GetHeroList())
		{
			if (hero.state == Defined.ItemState.Locked && hero.unlockNeedTeamLevel > 0 && DataCenter.Save().GetTeamData().teamLevel >= hero.unlockNeedTeamLevel)
			{
				hero.state = Defined.ItemState.Purchase; 
			}

			hero.combat = hero.upgradeData.weaponCombat;
		}
	}

	public static int BuyHero()
	{
		PlayerData hero = DataCenter.Save().GetHeroList().ToList().Find(x => x.heroIndex == DataCenter.State().selectHeroIndex);

		if (hero.costType == Defined.COST_TYPE.Money && DataCenter.Save().Money >= hero.unlockCost)
		{
			DataCenter.Save().Money -= hero.unlockCost;
			hero.state = Defined.ItemState.Available;

			return 0;
		}
		if (hero.costType == Defined.COST_TYPE.Crystal && DataCenter.Save().Crystal >= hero.unlockCost)
		{
			DataCenter.Save().Crystal -= hero.unlockCost;
			hero.state = Defined.ItemState.Available;

			return 0;
		}

		return -1;
	}

	public static int BuySite()
	{
		if (DataCenter.Save().Crystal >= DataCenter.Save().GetTeamData().teamSitesData[DataCenter.State().selectTeamSiteIndex].unlockSitePrice)
		{
			DataCenter.Save().Crystal -= DataCenter.Save().GetTeamData().teamSitesData[DataCenter.State().selectTeamSiteIndex].unlockSitePrice;
			DataCenter.Save().GetTeamData().teamSitesData[DataCenter.State().selectTeamSiteIndex].state = Defined.ItemState.Available;

			return 0;
		}

		return -1;
	}
}
