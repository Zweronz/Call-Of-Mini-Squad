using System.Collections.Generic;
using UnityEngine;

public class UINewAchievementManage : MonoBehaviour
{
	public class ACHIEVEMENTITEMINFO
	{
		public UtilUIAchievementInfo ui;

		public AchievementData data;

		public GameObject go;

		public ACHIEVEMENTITEMINFO(GameObject go, AchievementData data)
		{
			this.go = go;
			this.data = data;
			ui = go.GetComponent<UtilUIAchievementInfo>();
		}
	}

	[SerializeField]
	private UtilUIPropertyInfo m_scriptUIPropertyInfo;

	[SerializeField]
	private UIToggle[] toggles;

	[SerializeField]
	private UIGrid[] grid;

	[SerializeField]
	private AutoCreatByPrefab[] autoCreate;

	protected Dictionary<string, ACHIEVEMENTITEMINFO> dictAchievementInfo = new Dictionary<string, ACHIEVEMENTITEMINFO>();

	protected Dictionary<GameObject, string> dictMapAchievementInfo = new Dictionary<GameObject, string>();

	public UtilUIPropertyInfo UIPROPERTYINFO
	{
		get
		{
			return m_scriptUIPropertyInfo;
		}
	}

	private void Start()
	{
		UIDialogManager.Instance.SetPropertyScript(UIPROPERTYINFO);
		UIPROPERTYINFO.SetBackBtnClickDelegate(HandleBackBtnClickEvent);
		UIPROPERTYINFO.SetIAPBtnClickDelegate(HandleOpenShopBtnClickEvent);
		UpdatePropertyInfoPart("ACHIEVEMENTS", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
		RequestGetAchievementList();
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

	public void InitAchievement(Dictionary<string, AchievementData> dicts)
	{
		foreach (KeyValuePair<GameObject, string> item in dictMapAchievementInfo)
		{
			Object.DestroyImmediate(item.Key);
		}
		dictAchievementInfo.Clear();
		dictMapAchievementInfo.Clear();
		foreach (KeyValuePair<string, AchievementData> dict in dicts)
		{
			if (dict.Value.bDaily)
			{
				SerializeItem(dict.Value.site, dict.Key, dict.Value, autoCreate[0]);
			}
			else
			{
				SerializeItem(dict.Value.site, dict.Key, dict.Value, autoCreate[1]);
			}
			UpdateItemUI(dict.Key);
		}
		grid[0].repositionNow = true;
		grid[1].repositionNow = true;
	}

	protected void SerializeItem(int index, string _id, AchievementData data, AutoCreatByPrefab ac)
	{
		GameObject gameObject = ac.CreatePefab(index);
		ACHIEVEMENTITEMINFO aCHIEVEMENTITEMINFO = new ACHIEVEMENTITEMINFO(gameObject, data);
		dictAchievementInfo.Add(_id, aCHIEVEMENTITEMINFO);
		dictMapAchievementInfo.Add(gameObject, _id);
		aCHIEVEMENTITEMINFO.ui.BlindFunction(HandleClaimRewardBtnClickEvent);
	}

	protected void UpdateItemUI(string _id)
	{
		ACHIEVEMENTITEMINFO aCHIEVEMENTITEMINFO = dictAchievementInfo[_id];
		aCHIEVEMENTITEMINFO.ui.UpdateTitle(aCHIEVEMENTITEMINFO.data.title);
		aCHIEVEMENTITEMINFO.ui.UpdateContent(aCHIEVEMENTITEMINFO.data.des);
		aCHIEVEMENTITEMINFO.ui.UpdateSlider(aCHIEVEMENTITEMINFO.data.scheduleMin, aCHIEVEMENTITEMINFO.data.scheduleMax);
		aCHIEVEMENTITEMINFO.ui.UpdateClaimBtnState(aCHIEVEMENTITEMINFO.data.state);
		aCHIEVEMENTITEMINFO.ui.UpdateReward(aCHIEVEMENTITEMINFO.data.money, aCHIEVEMENTITEMINFO.data.crystal, aCHIEVEMENTITEMINFO.data.honor, aCHIEVEMENTITEMINFO.data.hero);
	}

	public void RequestGetAchievementList()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 33);
		UIDialogManager.Instance.ShowBlock(33);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Achievement_Get, OnGetAchievementListFinished);
	}

	public void OnGetAchievementListFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 33);
		UIDialogManager.Instance.HideBlock(33);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else
		{
			InitAchievement(UIConstant.gDictAchievementData);
		}
	}

	public void RequestClaimReward(string _id)
	{
		DataCenter.State().selectAchievementId = _id;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 34);
		UIDialogManager.Instance.ShowBlock(34);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Achievement_ClaimReward, OnClaimRewardFinished);
	}

	public void OnClaimRewardFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 34);
		UIDialogManager.Instance.HideBlock(34);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		InitAchievement(UIConstant.gDictAchievementData);
		UpdatePropertyInfoPart("null#", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
	}

	public void HandleOpenShopBtnClickEvent()
	{
		UIDialogManager.Instance.ShowShopDialogUI(HandleBuyIAPFinishedEvent);
	}

	public void HandleBuyIAPFinishedEvent(int code)
	{
		UpdatePropertyInfoPart("null#", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
	}

	public void HandleBackBtnClickEvent()
	{
		SceneManager.Instance.SwitchScene("UIBase");
	}

	public void HandleToggle1ValueChanged()
	{
	}

	public void HandleToggle2ValueChanged()
	{
	}

	public void HandleClaimRewardBtnClickEvent(GameObject go)
	{
		if (dictMapAchievementInfo.ContainsKey(go))
		{
			string text = dictMapAchievementInfo[go];
			ACHIEVEMENTITEMINFO aCHIEVEMENTITEMINFO = dictAchievementInfo[text];
			if (aCHIEVEMENTITEMINFO.data.state == 0)
			{
				UIDialogManager.Instance.ShowDriftMsgInfoUI("Unable to claim.");
			}
			else
			{
				RequestClaimReward(text);
			}
		}
	}
}
