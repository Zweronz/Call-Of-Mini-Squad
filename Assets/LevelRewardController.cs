using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelRewardController
{
	public static void Get()
	{
		if (LevelCalcTest.LevelRewards.Count == 0)
		{
			LevelCalcTest.Init();
		}

		int[] rewards = LevelCalcTest.LevelRewards[DataCenter.State().selectAreaNode];

		DataCenter.Save().selectLevelDropData = new LevelDropData
		{
			money = rewards[0],
			exp = rewards[1]
		};
	}
}
