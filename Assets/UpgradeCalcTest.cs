using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UpgradeCalcTest : MonoBehaviour
{
	public int level;

	public float price;

	void Update()
	{
		price = (37.5f + Mathf.Pow(level, 3.537243574f) * 1f) / 100f;
	}
}
