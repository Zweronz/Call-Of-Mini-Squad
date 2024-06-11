using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelCalcTest
{
	private const int CoinsStarting = 1500, XPStarting = 510, CoinsStartIncreasing = 10, XPStartIncreasing = 280, CoinsIncrease = 5, XPIncrease = 20, CombatStarting = 350, CombatStartIncreasing = 40, CombatIncrease = 20;

	private static int currentCoinAdd, currentXPAdd, currentCombatAdd;

	public static List<int[]> LevelRewards = new List<int[]>();

	public static void Init()
	{
		for (int i = 0; i < 150; i++)
		{
			if (i == 0)
			{
				LevelRewards.Add(new int[3] { CoinsStarting, XPStarting, CombatStarting } );
				continue;
			}

			currentCoinAdd += CoinsStartIncreasing + (CoinsIncrease * (i - 1));
			currentXPAdd += XPStartIncreasing + (XPIncrease * (i - 1));
			currentCombatAdd += CombatStartIncreasing + (CombatIncrease * (i - 1));

			LevelRewards.Add(new int[3] { CoinsStarting + currentCoinAdd,  XPStarting + currentXPAdd, CombatStarting + currentCombatAdd } );
		}
	}
}
