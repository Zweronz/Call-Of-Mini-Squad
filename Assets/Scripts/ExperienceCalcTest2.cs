using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceCalcTest2 : MonoBehaviour
{
	public int[] levels;

	void Start()
	{
		ExperienceCalcTest.Init();
		levels = ExperienceCalcTest.levelTest;
	}
}
