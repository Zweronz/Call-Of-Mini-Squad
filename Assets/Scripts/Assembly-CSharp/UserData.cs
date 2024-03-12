using System.Collections.Generic;
using CoMDS2;
using UnityEngine;

public class UserData
{
	private int m_PVPSelectTargetIndex;

	private List<PVP_Target> PVP_TargetList;

	public List<PVP_Target> PVP_TopList;

	public List<PVP_AttackLogPlayerData> PVP_AttackLog;

	public PVP_Target PVP_myData { get; set; }

	public int PVP_SettleTime { get; set; }

	public int PVP_SelectedTargetIndex
	{
		get
		{
			return m_PVPSelectTargetIndex;
		}
		set
		{
			m_PVPSelectTargetIndex = value;
		}
	}

	public PVP_Target PVP_SelectTarget { get; set; }

	public UserData()
	{
		PVP_TargetList = new List<PVP_Target>();
		PVP_TopList = new List<PVP_Target>();
		PVP_AttackLog = new List<PVP_AttackLogPlayerData>();
		PVP_myData = new PVP_Target();
		PVP_myData.sUUID = SystemInfo.deviceUniqueIdentifier;
		PVP_myData.sName = SystemInfo.deviceName;
		PVP_myData.iHonor = 0;
		PVP_myData.iNextActionTime = 0;
		PVP_myData.iPower = 5;
		PVP_myData.iPowerCost = 0;
		if (Util.s_debug)
		{
			CreateDummyTargets();
		}
	}

	public PVP_Target[] PVP_GetTargetList()
	{
		return PVP_TargetList.ToArray();
	}

	public void PVP_AddTarget(PVP_Target target)
	{
		PVP_TargetList.Add(target);
	}

	public void RemoveAllTarget()
	{
		PVP_TargetList.Clear();
	}

	public void CreateDummyTargets()
	{
		PVP_TargetList.Clear();
		for (int i = 0; i < 4; i++)
		{
			PVP_Target pVP_Target = new PVP_Target();
			pVP_Target.sName = "test0" + i;
			pVP_Target.iFaceIndex = 6;
			pVP_Target.team = new TeamData();
			pVP_Target.team.teamSitesData[0] = new TeamSiteData();
			pVP_Target.team.teamSitesData[0].playerData = new PlayerData();
			pVP_Target.team.teamSitesData[0].playerData.heroIndex = 10;
			pVP_Target.team.teamSitesData[0].playerData.siteNum = 0;
			pVP_Target.team.teamSitesData[1] = new TeamSiteData();
			pVP_Target.team.teamSitesData[1].playerData = new PlayerData();
			pVP_Target.team.teamSitesData[1].playerData.heroIndex = 10;
			pVP_Target.team.teamSitesData[1].playerData.siteNum = 1;
			pVP_Target.team.teamSitesData[2] = new TeamSiteData();
			pVP_Target.team.teamSitesData[2].playerData = new PlayerData();
			pVP_Target.team.teamSitesData[2].playerData.heroIndex = 0;
			pVP_Target.team.teamSitesData[2].playerData.siteNum = 2;
			pVP_Target.team.teamSitesData[3] = new TeamSiteData();
			pVP_Target.team.teamSitesData[3].playerData = new PlayerData();
			pVP_Target.team.teamSitesData[3].playerData.heroIndex = 2;
			pVP_Target.team.teamSitesData[3].playerData.siteNum = 3;
			pVP_Target.team.teamSitesData[4] = new TeamSiteData();
			pVP_Target.team.teamSitesData[4].playerData = new PlayerData();
			pVP_Target.team.teamSitesData[4].playerData.heroIndex = 1;
			pVP_Target.team.teamSitesData[4].playerData.siteNum = 3;
			PVP_TargetList.Add(pVP_Target);
		}
	}

	public int GetEquipHealth(int equipIndex, int equipLevel)
	{
		DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(equipIndex);
		if (equipLevel < 0 || equipLevel >= equipDataByIndex.hp.Length)
		{
			return 0;
		}
		return equipDataByIndex.hp[equipLevel];
	}

	public float GetEquipHealthPercent(int equipIndex, int equipLevel)
	{
		DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(equipIndex);
		if (equipLevel < 0 || equipLevel >= equipDataByIndex.hp.Length)
		{
			return 0f;
		}
		return equipDataByIndex.hpPercent;
	}
}
