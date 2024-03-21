using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleResultController
{
	public static void Get()
	{
		DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelStars[DataCenter.State().selectLevelMode][DataCenter.State().selectAreaNode] = (ushort)DataCenter.State().battleStars;
		DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelProgress[DataCenter.State().selectAreaNode + 1] = 1;

		int[] baseRewards = LevelCalcTest.LevelRewards[DataCenter.State().selectWorldNode];
		baseRewards[0] += GameBattle.m_instance.getMoneyInBattle;

		if (DataCenter.State().battleStars == 3)
		{
			baseRewards[1] *= 2;
		}

		DataCenter.Save().Money += baseRewards[0];
		DataCenter.Save().GetTeamData().teamExp += baseRewards[1];

		DataCenter.Save().selectLevelDropData.money = baseRewards[0];
		DataCenter.Save().selectLevelDropData.exp = baseRewards[1];

		GameBattle.m_instance.bGetBattleResultData = true;
	}
}
