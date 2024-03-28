using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCalcTest : MonoBehaviour
{
	public static int[] levelTest;

	private const int XPStarting = 510, XPStartIncreasing = 250, XPIncrease = 550;

	private static int currentXPAdd;

	public static void Init()
	{
		for (int i = 0; i < 100; i++)
		{
			if (i == 0)
			{
				levelTest[i] = XPStarting;
				continue;
			}

			currentXPAdd += XPStartIncreasing + (XPIncrease * (i - 1));

			levelTest[i] = XPStarting + currentXPAdd;
		}
	}
}
