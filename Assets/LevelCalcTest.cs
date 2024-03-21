using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCalcTest : MonoBehaviour
{
	public List<string> levels = new List<string>();

	private const int CoinsStarting = 1500, XPStarting = 510, CoinsStartIncreasing = 10, XPStartIncreasing = 280, CoinsIncrease = 5, XPIncrease = 20;

	private static int currentCoinAdd, currentXPAdd;

	public static List<int[]> LevelRewards = new List<int[]>();

	public static void Init()
	{
		for (int i = 0; i < 100; i++)
		{
			if (i == 0)
			{
				LevelRewards.Add(new int[2] { CoinsStarting, XPStarting } );
				continue;
			}

			currentCoinAdd += CoinsStartIncreasing + (CoinsIncrease * (i - 1));
			currentXPAdd += XPStartIncreasing + (XPIncrease * (i - 1));

			LevelRewards.Add(new int[2] { CoinsStarting + currentCoinAdd,  XPStarting + currentXPAdd } );
		}
	}
}
