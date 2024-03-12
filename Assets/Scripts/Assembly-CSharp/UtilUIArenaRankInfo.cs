using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilUIArenaRankInfo : MonoBehaviour
{
	[Serializable]
	public class PERSONALUIINFO
	{
		public UILabel rank;

		public UISprite icon;

		public UILabel level;

		public UILabel city;

		public UILabel point;

		public GameObject[] rewardGOS;
	}

	public class POINTITEMINFO
	{
		public UtilUIArenaRankPointInfo ui;

		public PVP_RankTargetData data;

		public GameObject go;

		public POINTITEMINFO(GameObject go, PVP_RankTargetData data)
		{
			this.go = go;
			this.data = data;
			ui = go.GetComponent<UtilUIArenaRankPointInfo>();
		}
	}

	public class CONTESTITEMINFO
	{
		public UtilUIArenaRankContestInfo ui;

		public PVP_RankTargetData data;

		public GameObject go;

		public CONTESTITEMINFO(GameObject go, PVP_RankTargetData data)
		{
			this.go = go;
			this.data = data;
			ui = go.GetComponent<UtilUIArenaRankContestInfo>();
		}
	}

	[SerializeField]
	private UIToggle[] toggles;

	[SerializeField]
	private UIGrid[] grid;

	[SerializeField]
	private AutoCreatByPrefab[] autoCreat;

	[SerializeField]
	private PERSONALUIINFO[] personalsUIInfo;

	private UtilUIArenaRankInfo_ITEMCLICK_Delegate itemClickEvent;

	protected Dictionary<int, CONTESTITEMINFO> dictContestInfo = new Dictionary<int, CONTESTITEMINFO>();

	protected Dictionary<GameObject, int> dictMapContestInfo = new Dictionary<GameObject, int>();

	protected Dictionary<int, POINTITEMINFO> dictPointGlobalInfo = new Dictionary<int, POINTITEMINFO>();

	protected Dictionary<GameObject, int> dictMapPointGlobalInfo = new Dictionary<GameObject, int>();

	protected Dictionary<int, POINTITEMINFO> dictPointLocalInfo = new Dictionary<int, POINTITEMINFO>();

	protected Dictionary<GameObject, int> dictMapPointLocalInfo = new Dictionary<GameObject, int>();

	public void BlindFuntion(UtilUIArenaRankInfo_ITEMCLICK_Delegate _itemClickEvent)
	{
		itemClickEvent = _itemClickEvent;
	}

	protected void UpdatePersonalInfoUI(PERSONALUIINFO _ui, PVP_RankTargetData data)
	{
		if (data != null)
		{
			_ui.rank.text = string.Empty + data.rank;
			_ui.rank.fontSize = 26;
			_ui.rank.spacingX = 0;
			DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(data.teamLeaderIndex);
			_ui.icon.spriteName = heroDataByIndex.iconFileName;
			_ui.level.text = "LV " + data.teamLevel;
			_ui.city.text = string.Empty + data.userName;
			if (_ui.point != null)
			{
				_ui.point.text = string.Empty + data.rewardPoint;
				_ui.point.fontSize = 26;
				_ui.point.spacingX = 0;
			}
			int num = 0;
			if (data.rewardHonor > 0 && num < _ui.rewardGOS.Length)
			{
				UISprite component = _ui.rewardGOS[num].GetComponent<UISprite>();
				UILabel component2 = _ui.rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
				component.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
				component2.text = data.rewardHonor.ToString("###, ###");
				num++;
			}
			if (data.rewardCrystal > 0 && num < _ui.rewardGOS.Length)
			{
				UISprite component3 = _ui.rewardGOS[num].GetComponent<UISprite>();
				UILabel component4 = _ui.rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
				component3.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
				component4.text = data.rewardCrystal.ToString("###, ###");
				num++;
			}
			if (data.rewardMoney > 0 && num < _ui.rewardGOS.Length)
			{
				UISprite component5 = _ui.rewardGOS[num].GetComponent<UISprite>();
				UILabel component6 = _ui.rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
				component5.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Money);
				component6.text = data.rewardMoney.ToString("###, ###");
				num++;
			}
			switch (num)
			{
			case 1:
				_ui.rewardGOS[0].SetActive(true);
				_ui.rewardGOS[0].transform.localPosition = new Vector3(-50f, 0f, 0f);
				_ui.rewardGOS[1].SetActive(false);
				break;
			case 2:
				_ui.rewardGOS[0].SetActive(true);
				_ui.rewardGOS[1].SetActive(true);
				_ui.rewardGOS[0].transform.localPosition = new Vector3(-160f, 0f, 0f);
				_ui.rewardGOS[1].transform.localPosition = new Vector3(30f, 0f, 0f);
				break;
			}
		}
		else
		{
			_ui.rank.text = "--";
			_ui.rank.fontSize = 48;
			_ui.rank.spacingX = 4;
			DataConf.HeroData heroDataByIndex2 = DataCenter.Conf().GetHeroDataByIndex(DataCenter.Save().GetTeamSiteData(Defined.TEAM_SITE.TEAM_LEADER).playerData.heroIndex);
			_ui.icon.spriteName = heroDataByIndex2.iconFileName;
			_ui.level.text = "LV " + DataCenter.Save().GetTeamData().teamLevel;
			_ui.city.text = string.Empty + DataCenter.Save().userName;
			if (_ui.point != null)
			{
				_ui.point.text = "--";
				_ui.point.fontSize = 48;
				_ui.point.spacingX = 4;
			}
			_ui.rewardGOS[0].SetActive(false);
			_ui.rewardGOS[1].SetActive(false);
		}
	}

	public void SetToggleValue(int i, bool value)
	{
		toggles[i].value = value;
	}

	public void HandleTitleContestValueChanged()
	{
		if (toggles[0].value)
		{
			UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 27);
			UIDialogManager.Instance.ShowBlock(27);
			HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_GetTopList, OnGetTopListFinished);
		}
	}

	public void HandleTitlePointGlobalValueChanged()
	{
		if (toggles[1].value)
		{
			DataCenter.State().selectArenaRankTypeByLanguage = "global";
			UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 28);
			UIDialogManager.Instance.ShowBlock(28);
			HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_GetRankList, OnGetRankListFinished);
		}
	}

	public void HandleTitlePointLocalValueChanged()
	{
		if (toggles[2].value)
		{
			DataCenter.State().selectArenaRankTypeByLanguage = UIUtil.GetProtocolLaguageCode();
			UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 28);
			UIDialogManager.Instance.ShowBlock(28);
			HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_GetRankList, OnGetRankListFinished);
		}
	}

	public void OnGetTopListFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 27);
		UIDialogManager.Instance.HideBlock(27);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else
		{
			InitContest(UIConstant.gDictTopTargetData);
		}
	}

	public void OnGetRankListFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 28);
		UIDialogManager.Instance.HideBlock(28);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else if (DataCenter.State().selectArenaRankTypeByLanguage == "global")
		{
			InitPointGlobal(UIConstant.gDictRankGlobalTargetData);
		}
		else
		{
			InitPointLocal(UIConstant.gDictRankLocalTargetData);
		}
	}

	public void InitContest(Dictionary<int, PVP_RankTargetData> _dict)
	{
		foreach (KeyValuePair<GameObject, int> item in dictMapContestInfo)
		{
			UnityEngine.Object.DestroyImmediate(item.Key);
		}
		dictContestInfo.Clear();
		dictMapContestInfo.Clear();
		foreach (KeyValuePair<int, PVP_RankTargetData> item2 in _dict)
		{
			SerializeContestItem(item2.Key, item2.Value);
			UpdateContestItemUI(item2.Key);
		}
		UpdatePersonalInfoUI(personalsUIInfo[0], UIConstant.gMyRankDataInfo);
		grid[0].repositionNow = true;
	}

	protected void SerializeContestItem(int rankID, PVP_RankTargetData data)
	{
		GameObject gameObject = autoCreat[0].CreatePefab(rankID);
		CONTESTITEMINFO cONTESTITEMINFO = new CONTESTITEMINFO(gameObject, data);
		cONTESTITEMINFO.ui.BlindFuntion(HandleContestItemClickedEvent);
		dictContestInfo.Add(rankID, cONTESTITEMINFO);
		dictMapContestInfo.Add(gameObject, rankID);
	}

	protected void UpdateContestItemUI(int rankID)
	{
		CONTESTITEMINFO cONTESTITEMINFO = dictContestInfo[rankID];
		cONTESTITEMINFO.ui.UpdateBackground(rankID % 2 == 0);
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(cONTESTITEMINFO.data.teamLeaderIndex);
		cONTESTITEMINFO.ui.UpdateIcon(heroDataByIndex.iconFileName);
		cONTESTITEMINFO.ui.UpdateRank(string.Empty + rankID);
		cONTESTITEMINFO.ui.UpdateLv("LV " + cONTESTITEMINFO.data.teamLevel);
		cONTESTITEMINFO.ui.UpdateCity(string.Empty + cONTESTITEMINFO.data.userName);
		cONTESTITEMINFO.ui.UpdateReward(cONTESTITEMINFO.data.rewardHonor, cONTESTITEMINFO.data.rewardCrystal, cONTESTITEMINFO.data.rewardMoney);
	}

	public void HandleContestItemClickedEvent(GameObject go)
	{
		int key = dictMapContestInfo[go];
		CONTESTITEMINFO cONTESTITEMINFO = dictContestInfo[key];
		if (itemClickEvent != null)
		{
			itemClickEvent(cONTESTITEMINFO.data.userId);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void InitPointGlobal(Dictionary<int, PVP_RankTargetData> _dict)
	{
		foreach (KeyValuePair<GameObject, int> item in dictMapPointGlobalInfo)
		{
			UnityEngine.Object.DestroyImmediate(item.Key);
		}
		dictPointGlobalInfo.Clear();
		dictMapPointGlobalInfo.Clear();
		foreach (KeyValuePair<int, PVP_RankTargetData> item2 in _dict)
		{
			SerializePointGlobalItem(item2.Key, item2.Value);
			UpdatePointGlobalItemUI(item2.Key);
		}
		UpdatePersonalInfoUI(personalsUIInfo[1], UIConstant.gMyRankDataInfo);
		grid[1].repositionNow = true;
	}

	protected void SerializePointGlobalItem(int rankID, PVP_RankTargetData data)
	{
		GameObject gameObject = autoCreat[1].CreatePefab(rankID);
		POINTITEMINFO pOINTITEMINFO = new POINTITEMINFO(gameObject, data);
		pOINTITEMINFO.ui.BlindFuntion(HandlePointGlobalItemClickedEvent);
		dictPointGlobalInfo.Add(rankID, pOINTITEMINFO);
		dictMapPointGlobalInfo.Add(gameObject, rankID);
	}

	protected void UpdatePointGlobalItemUI(int rankID)
	{
		POINTITEMINFO pOINTITEMINFO = dictPointGlobalInfo[rankID];
		pOINTITEMINFO.ui.UpdateBackground(rankID % 2 == 0);
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(pOINTITEMINFO.data.teamLeaderIndex);
		pOINTITEMINFO.ui.UpdateIcon(heroDataByIndex.iconFileName);
		pOINTITEMINFO.ui.UpdateRank(string.Empty + rankID);
		pOINTITEMINFO.ui.UpdateLv("LV " + pOINTITEMINFO.data.teamLevel);
		pOINTITEMINFO.ui.UpdateCity(string.Empty + pOINTITEMINFO.data.userName);
		pOINTITEMINFO.ui.UpdatePoint(string.Empty + pOINTITEMINFO.data.rewardPoint);
		pOINTITEMINFO.ui.UpdateReward(pOINTITEMINFO.data.rewardHonor, pOINTITEMINFO.data.rewardCrystal, pOINTITEMINFO.data.rewardMoney);
	}

	public void HandlePointGlobalItemClickedEvent(GameObject go)
	{
		int key = dictMapPointGlobalInfo[go];
		POINTITEMINFO pOINTITEMINFO = dictPointGlobalInfo[key];
		if (itemClickEvent != null)
		{
			itemClickEvent(pOINTITEMINFO.data.userId);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void InitPointLocal(Dictionary<int, PVP_RankTargetData> _dict)
	{
		foreach (KeyValuePair<GameObject, int> item in dictMapPointLocalInfo)
		{
			UnityEngine.Object.DestroyImmediate(item.Key);
		}
		dictPointLocalInfo.Clear();
		dictMapPointLocalInfo.Clear();
		foreach (KeyValuePair<int, PVP_RankTargetData> item2 in _dict)
		{
			SerializePointLocalItem(item2.Key, item2.Value);
			UpdatePointLocalItemUI(item2.Key);
		}
		UpdatePersonalInfoUI(personalsUIInfo[2], UIConstant.gMyRankDataInfo);
		grid[2].repositionNow = true;
	}

	protected void SerializePointLocalItem(int rankID, PVP_RankTargetData data)
	{
		GameObject gameObject = autoCreat[2].CreatePefab(rankID);
		POINTITEMINFO pOINTITEMINFO = new POINTITEMINFO(gameObject, data);
		pOINTITEMINFO.ui.BlindFuntion(HandlePointLocalItemClickedEvent);
		dictPointLocalInfo.Add(rankID, pOINTITEMINFO);
		dictMapPointLocalInfo.Add(gameObject, rankID);
	}

	protected void UpdatePointLocalItemUI(int rankID)
	{
		POINTITEMINFO pOINTITEMINFO = dictPointLocalInfo[rankID];
		pOINTITEMINFO.ui.UpdateBackground(rankID % 2 == 0);
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(pOINTITEMINFO.data.teamLeaderIndex);
		pOINTITEMINFO.ui.UpdateIcon(heroDataByIndex.iconFileName);
		pOINTITEMINFO.ui.UpdateRank(string.Empty + rankID);
		pOINTITEMINFO.ui.UpdateLv("LV " + pOINTITEMINFO.data.teamLevel);
		pOINTITEMINFO.ui.UpdateCity(string.Empty + pOINTITEMINFO.data.userName);
		pOINTITEMINFO.ui.UpdatePoint(string.Empty + pOINTITEMINFO.data.rewardPoint);
		pOINTITEMINFO.ui.UpdateReward(pOINTITEMINFO.data.rewardHonor, pOINTITEMINFO.data.rewardCrystal, pOINTITEMINFO.data.rewardMoney);
	}

	public void HandlePointLocalItemClickedEvent(GameObject go)
	{
		int key = dictMapPointLocalInfo[go];
		POINTITEMINFO pOINTITEMINFO = dictPointLocalInfo[key];
		if (itemClickEvent != null)
		{
			itemClickEvent(pOINTITEMINFO.data.userId);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}
