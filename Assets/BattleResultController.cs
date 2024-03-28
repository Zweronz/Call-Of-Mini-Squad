using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleResultController
{
	public static void Get()
	{
		DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelStars[DataCenter.State().selectLevelMode][DataCenter.State().selectAreaNode] = (ushort)DataCenter.State().battleStars;

		if (DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelProgress[DataCenter.State().selectAreaNode + 1] <= 0)
		{
			ushort worldIndex = (ushort)(DataCenter.State().selectAreaNode + 2);
			for (int i = 0; i < DataCenter.State().selectAreaNode + 2; i++)
			{
				DataCenter.Save().GetWorldProgressData(DataCenter.State().selectWorldNode).levelProgress[i] = worldIndex;
				worldIndex--;
			}
		}

		int[] baseRewards = new int[2]
		{
			LevelCalcTest.LevelRewards[DataCenter.State().selectWorldNode][0] * (DataCenter.State().battleStars == 3 ? 2 : 1),
			LevelCalcTest.LevelRewards[DataCenter.State().selectWorldNode][1] * (DataCenter.State().battleStars == 3 ? 2 : 1),
		};

		DataCenter.Save().Money += baseRewards[0];
		DataCenter.Save().GetTeamData().teamExp += baseRewards[1];

		DataCenter.Save().selectLevelDropData.money = baseRewards[0];
		DataCenter.Save().selectLevelDropData.exp = baseRewards[1];

		GameBattle.m_instance.bGetBattleResultData = true;
	}
}
