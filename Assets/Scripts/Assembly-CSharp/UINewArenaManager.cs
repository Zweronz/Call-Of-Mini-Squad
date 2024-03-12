using UnityEngine;

public class UINewArenaManager : MonoBehaviour
{
	private static UINewArenaManager mInstance;

	[SerializeField]
	private UtilUIPropertyInfo m_scriptUIPropertyInfo;

	[SerializeField]
	private UtilUIArenaMainScreenInfo m_scriptArenaMainScreenInfo;

	[SerializeField]
	private UtilUIArenaTeamDetailInfo m_scriptArenaTeamDetailInfo;

	[SerializeField]
	private UtilUIArenaRankInfo m_scriptAreanRankInfo;

	[SerializeField]
	protected UIToggle[] toggleTitle;

	public static UINewArenaManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = GameObject.Find("UIManager").GetComponent<UINewArenaManager>();
			}
			return mInstance;
		}
	}

	public UtilUIPropertyInfo UIPROPERTYINFO
	{
		get
		{
			return m_scriptUIPropertyInfo;
		}
	}

	public UtilUIArenaMainScreenInfo UIARENAINFO
	{
		get
		{
			return m_scriptArenaMainScreenInfo;
		}
	}

	public UtilUIArenaTeamDetailInfo UITEAMDETAILINFO
	{
		get
		{
			return m_scriptArenaTeamDetailInfo;
		}
	}

	public UtilUIArenaRankInfo UIRANKINFO
	{
		get
		{
			return m_scriptAreanRankInfo;
		}
	}

	private void Start()
	{
		UIDialogManager.Instance.SetPropertyScript(UIPROPERTYINFO);
		DataCenter.State().isPVPMode = true;
		BlindFunction();
		UpdatePropertyInfoPart("ARENA", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
	}

	private void BlindFunction()
	{
		UIPROPERTYINFO.SetBackBtnClickDelegate(HandleBackBtnClickEvent);
		UIPROPERTYINFO.SetIAPBtnClickDelegate(HandleOpenShopBtnClickEvent);
		UIARENAINFO.SetChallengePlayerBtnClickedDelegate(HandleChallengePlayerBtnClickedEvent);
		UIARENAINFO.SetLookPlayerTeamDetailBtnClickedDelegate(HandleLookPlayerDetailBtnClickedEvent);
		UIRANKINFO.BlindFuntion(HandleShowPlayerDetailEvent);
	}

	public void UpdatePropertyInfoPart(string name, int rank, int gold, int crystal)
	{
		if (name != "null#")
		{
			UIPROPERTYINFO.UpdateName(name);
		}
		if (rank > 0)
		{
			UIPROPERTYINFO.UpdateRank(rank.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateRank(0 + string.Empty);
		}
		if (gold > 0)
		{
			UIPROPERTYINFO.UpdateGold(gold.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateGold(0 + string.Empty);
		}
		if (crystal > 0)
		{
			UIPROPERTYINFO.UpdateCrystal(crystal.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateCrystal(0 + string.Empty);
		}
	}

	public void OnLoadPVPUsersDetailInfoFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 23);
		UIDialogManager.Instance.HideBlock(23);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		UITEAMDETAILINFO.SetVisable(true);
		UITEAMDETAILINFO.Init(UIConstant.gLSTargetDetailData);
	}

	public void OnStartPVPFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 24);
		UIDialogManager.Instance.HideBlock(24);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else
		{
			SceneLoadingManager.SwitchScene("PVPMAP");
		}
	}

	public void HandleOpenShopBtnClickEvent()
	{
		UIDialogManager.Instance.ShowShopDialogUI(HandleBuyIAPFinishedEvent);
	}

	public void HandleBackBtnClickEvent()
	{
		DataCenter.State().isPVPMode = false;
		SceneManager.Instance.SwitchScene("UIBase");
	}

	public void HandleTitleArenaValueChanged()
	{
		if (toggleTitle[0].value)
		{
			UIARENAINFO.SearchPVPUsers();
		}
	}

	public void HandleTitleRankValueChanged()
	{
		if (!toggleTitle[1].value)
		{
		}
	}

	public void HandleChallengePlayerBtnClickedEvent(UtilUIArenaMainScreenInfo.TARGETARENAITEM item)
	{
		DataCenter.State().selectArenaTargetRank = item.data.iRank;
		DataCenter.User().PVP_SelectTarget = item.data;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 24);
		UIDialogManager.Instance.ShowBlock(24);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_StartPVP, OnStartPVPFinished);
	}

	public void HandleLookPlayerDetailBtnClickedEvent(UtilUIArenaMainScreenInfo.TARGETARENAITEM item)
	{
		HandleShowPlayerDetailEvent(item.data.sUUID);
	}

	public void HandleBuyIAPFinishedEvent(int code)
	{
		UpdatePropertyInfoPart("null#", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
	}

	public void HandleShowPlayerDetailEvent(string _id)
	{
		DataCenter.State().selectArenaTargetUUId = _id;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 23);
		UIDialogManager.Instance.ShowBlock(23);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Arena_LoadProfile, OnLoadPVPUsersDetailInfoFinished);
	}
}
