using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TeamBuffController
{
	public static void Refresh()
	{
		foreach (TeamAttributeData talent in DataCenter.Save().teamAttributeSaveData.teamAttributeTalent)
		{
			if (DataCenter.Save().teamAttributeSaveData.teamAttributeAssignedPoint >= talent.unlockPoint && talent.state != Defined.ItemState.Available)
			{
				talent.state = Defined.ItemState.Available;
			}
		}

		foreach (TeamAttributeData evolution in DataCenter.Save().teamAttributeSaveData.teamAttributeEvolve)
		{
			if (DataCenter.Save().teamAttributeSaveData.teamAttributeAssignedPoint >= evolution.unlockPoint && evolution.state != Defined.ItemState.Available)
			{
				evolution.state = Defined.ItemState.Available;
			}
		}
	}
}
