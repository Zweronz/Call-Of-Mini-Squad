using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilUIArenaMainScreenInfo : MonoBehaviour
{
	private enum TimeManageType
	{
		E_ArenaGetRewardTime = 0
	}

	[Serializable]
	public class TargetAreanUIInfo
	{
		public GameObject go;

		public UILabel name;

		public UITexture icon;

		public UILabel rank;

		public UIImageButton challngeButton;

		public UIImageButton searchButton;
	}

	public class TARGETARENAITEM
	{
		public TargetAreanUIInfo ui;

		public PVP_Target data;

		public int seatID;

		public TARGETARENAITEM(TargetAreanUIInfo _ui, PVP_Target _data, int _seatID)
		{
			ui = _ui;
			data = _data;
			seatID = _seatID;
		}
	}

	public UITeamModelManager m_modelManagerScript;

	public GameObject selectPlayerEffectPrefab;

	public static int gMyRank = -1;

	public static int gMyReward = -1;

	public static long gMyLeftRewardTime = -1L;

	[SerializeField]
	protected UILabel myName;

	[SerializeField]
	protected UITexture myIcon;

	[SerializeField]
	protected UILabel myRank;

	[SerializeField]
	protected UILabel myRewards;

	[SerializeField]
	protected UILabel myLeftRewardTime;

	private int mySeatID = 10;

	public List<TargetAreanUIInfo> lsAreanUI = new List<TargetAreanUIInfo>();

	protected Dictionary<string, TARGETARENAITEM> dictTargetArenaItems = new Dictionary<string, TARGETARENAITEM>();

	protected Dictionary<GameObject, string> dictMapTargetArenaItems = new Dictionary<GameObject, string>();

	protected UtilUIArenaMainScreenInfo_ChallengeBtnClicked_Delegate challengeBtnClickDele;

	protected UtilUIStandbyPlayerAppearance_LookDetailBtnClicked_Delegate lookDetailBtnClickDele;

	public void SearchPVPUsers()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 22);
		UIDialogManager.Instance.ShowBlock(22);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_SearchPVPUsers, OnSearchPVPUsersFinished);
	}

	public void InitData(PVP_Target[] lsData)
	{
		dictTargetArenaItems.Clear();
		dictMapTargetArenaItems.Clear();
		for (int i = 0; i < lsData.Length; i++)
		{
			TARGETARENAITEM value = new TARGETARENAITEM(lsAreanUI[i], lsData[i], i);
			dictTargetArenaItems.Add(lsData[i].sUUID, value);
			dictMapTargetArenaItems.Add(lsAreanUI[i].go, lsData[i].sUUID);
		}
	}

	public void UpdateTargetUI(TARGETARENAITEM item)
	{
		item.ui.name.text = item.data.sName;
		item.ui.rank.text = "Rank:" + item.data.iRank;
		bool flag = m_modelManagerScript.AddModelInfo_Force(item.seatID, DataCenter.Conf().GetHeroDataByIndex(item.data.iFaceIndex), Defined.RANK_TYPE.WHITE, true, 10f);
		Rect texUV = m_modelManagerScript.GetTexUV(item.seatID);
		item.ui.icon.uvRect = texUV;
	}

	public void UpdateMyUI()
	{
		myName.text = string.Empty + DataCenter.Save().userName;
		myRank.text = string.Empty + gMyRank;
		myRewards.text = string.Empty + gMyReward;
		bool flag = m_modelManagerScript.AddModelInfo_Force(mySeatID, DataCenter.Conf().GetHeroDataByIndex(DataCenter.Save().GetTeamSiteData(Defined.TEAM_SITE.TEAM_LEADER).playerData.heroIndex), Defined.RANK_TYPE.WHITE, true, 10f);
		Rect texUV = m_modelManagerScript.GetTexUV(mySeatID);
		myIcon.uvRect = texUV;
		TimeManager.Instance.DestroyCalculagraph(0);
		TimeManager.Instance.Init(0, (float)gMyLeftRewardTime / 1000f, OnGetRewardTimeFinishedDelg, OnGetRewardTimeUpdatedDelg, "E_ArenaGetRewardTime");
	}

	public void OnGetRewardTimeUpdatedDelg(float _t)
	{
		myLeftRewardTime.text = UIUtil.TimeToStr_AHMS((long)_t);
	}

	public void OnGetRewardTimeFinishedDelg()
	{
		SearchPVPUsers();
	}

	public void UpdateUI()
	{
		UpdateMyUI();
		foreach (KeyValuePair<string, TARGETARENAITEM> dictTargetArenaItem in dictTargetArenaItems)
		{
			UpdateTargetUI(dictTargetArenaItem.Value);
		}
	}

	public void HandleLookDetailBtnClickedEvent(GameObject go)
	{
		TARGETARENAITEM item = dictTargetArenaItems[dictMapTargetArenaItems[go.transform.parent.gameObject]];
		if (lookDetailBtnClickDele != null)
		{
			lookDetailBtnClickDele(item);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleLookDetailBtnOnPressEvent(GameObject go)
	{
		TARGETARENAITEM tARGETARENAITEM = dictTargetArenaItems[dictMapTargetArenaItems[go]];
		GameObject model = m_modelManagerScript.GetModel(tARGETARENAITEM.seatID);
		UtilUIStandbyPlayersInfo.SetPlayerModelHaloEffectVisable(true, model, selectPlayerEffectPrefab);
		UtilUIStandbyPlayersInfo.SetPlayerModelOutLineEffectVisable(true, model);
	}

	public void HandleLookDetailBtnOnReleaseEvent(GameObject go)
	{
		TARGETARENAITEM tARGETARENAITEM = dictTargetArenaItems[dictMapTargetArenaItems[go]];
		GameObject model = m_modelManagerScript.GetModel(tARGETARENAITEM.seatID);
		UtilUIStandbyPlayersInfo.SetPlayerModelHaloEffectVisable(false, model, selectPlayerEffectPrefab);
		UtilUIStandbyPlayersInfo.SetPlayerModelOutLineEffectVisable(false, model);
		if (lookDetailBtnClickDele != null)
		{
			lookDetailBtnClickDele(tARGETARENAITEM);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleChallngeBtnClickedEvent(GameObject go)
	{
		TARGETARENAITEM item = dictTargetArenaItems[dictMapTargetArenaItems[go.transform.parent.gameObject]];
		if (challengeBtnClickDele != null)
		{
			challengeBtnClickDele(item);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnSearchPVPUsersFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 22);
		UIDialogManager.Instance.HideBlock(22);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		InitData(UIConstant.gLSTargetAreanData);
		UpdateUI();
	}

	public void SetChallengePlayerBtnClickedDelegate(UtilUIArenaMainScreenInfo_ChallengeBtnClicked_Delegate dele)
	{
		challengeBtnClickDele = dele;
	}

	public void SetLookPlayerTeamDetailBtnClickedDelegate(UtilUIStandbyPlayerAppearance_LookDetailBtnClicked_Delegate dele)
	{
		lookDetailBtnClickDele = dele;
	}
}
