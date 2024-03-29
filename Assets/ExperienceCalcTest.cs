using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExperienceCalcTest
{
	public static int[] levelTest;

	public static int XPStarting = 510, XPStartIncreasing = 240, XPIncrease = 240, XPIncreaseIncrease = 30;

	private static int currentXPAdd;

	public static void Init()
	{
		levelTest = new int[100];

		for (int i = 0; i < 100; i++)
		{
			if (i == 0)
			{
				levelTest[i] = XPStarting;
				continue;
			}

			currentXPAdd += XPStartIncreasing + (XPIncrease * (i - 1));
			XPIncrease += XPIncreaseIncrease;

			levelTest[i] = XPStarting + currentXPAdd;
		}
	}
}
