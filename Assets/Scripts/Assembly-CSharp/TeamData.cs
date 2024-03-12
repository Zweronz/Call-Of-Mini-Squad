using System.Collections.Generic;
using CoMDS2;

public class TeamData
{
	public TeamSiteData[] teamSitesData;

	public int teamLevel;

	public int teamMaxLevel;

	public int teamExp;

	public int teamMaxExp;

	public Dictionary<TeamSpecialAttribute.TeamAttributeType, int> talents;

	public Dictionary<TeamSpecialAttribute.TeamAttributeEvolveType, int> evolves;

	public TeamData()
	{
		teamSitesData = new TeamSiteData[5];
		for (int i = 0; i < 5; i++)
		{
			teamSitesData[i] = new TeamSiteData();
		}
		talents = new Dictionary<TeamSpecialAttribute.TeamAttributeType, int>();
		evolves = new Dictionary<TeamSpecialAttribute.TeamAttributeEvolveType, int>();
		teamLevel = 1;
		teamExp = 0;
	}

	public int GetTeamCombat()
	{
		int num = 0;
		for (int i = 0; i < teamSitesData.Length; i++)
		{
			if (teamSitesData[i].playerData != null)
			{
				num += teamSitesData[i].playerData.Combat;
			}
		}
		return num;
	}
}
