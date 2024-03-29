using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgradeCalcTest2 : MonoBehaviour
{
	public List<string> levels = new List<string>();

	void Start()
	{
		UpgradeCalcTest.Init();
		levels = (from level in UpgradeCalcTest.upgrades select level[0] + ", " + level[1] + ", " + level[2]).ToList();
	}
}
