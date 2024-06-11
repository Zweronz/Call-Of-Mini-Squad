using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EquipUpgradeCalcTest2 : MonoBehaviour
{
	public List<string> helmets, armors, ornaments;

	void Start()
	{
		EquipUpgradeCalcTest.Init();
		int index = 0;

		for (int i = 0; i < 11; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				helmets.Add(EquipUpgradeCalcTest.helmets[i][j] + ", " + EquipUpgradeCalcTest.helmets[i][j + 6]);
				armors.Add(EquipUpgradeCalcTest.armors[i][j] + ", " + EquipUpgradeCalcTest.armors[i][j + 6]);
				ornaments.Add(EquipUpgradeCalcTest.ornaments[i][j] + ", " + EquipUpgradeCalcTest.ornaments[i][j + 6]);
			}
		}
	}
}
