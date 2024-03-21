using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelRewardController
{
	public static int Level;

	public static void Get()
	{
		if (LevelCalcTest.LevelRewards.Count == 0)
		{
			LevelCalcTest.Init();
		}

		int[] rewards = LevelCalcTest.LevelRewards[DataCenter.State().selectWorldNode];

		DataCenter.Save().selectLevelDropData = new LevelDropData
		{
			money = rewards[0],
			exp = rewards[1]
		};

		Level = 1;
	}
}
