using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TeamLevelController
{
	public static void OnReceivedXP()
	{
		if (ExperienceCalcTest.levelTest == null)
		{
			ExperienceCalcTest.Init();
		}

		while (DataCenter.Save().GetTeamData().teamExp >= DataCenter.Save().GetTeamData().teamMaxExp)
		{
			if (DataCenter.Save().GetTeamData().teamLevel >= DataCenter.Save().GetTeamData().teamMaxLevel)
			{
				DataCenter.Save().GetTeamData().teamLevel = DataCenter.Save().GetTeamData().teamMaxLevel;
				break;
			}

			DataCenter.Save().GetTeamData().teamLevel++;

			DataCenter.Save().GetTeamData().teamExp -= DataCenter.Save().GetTeamData().teamMaxExp;
			DataCenter.Save().GetTeamData().teamMaxExp = ExperienceCalcTest.levelTest[DataCenter.Save().GetTeamData().teamLevel - 1];
		}
	}
}
