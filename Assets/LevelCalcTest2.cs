using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelCalcTest2 : MonoBehaviour
{
	public List<string> levels = new List<string>();

	void Start()
	{
		LevelCalcTest.Init();
		levels = (from level in LevelCalcTest.LevelRewards select level[0] + ", " + level[1]).ToList();
	}
}
