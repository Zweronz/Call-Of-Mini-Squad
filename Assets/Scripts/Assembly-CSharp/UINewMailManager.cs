using System;
using System.Collections.Generic;
using UnityEngine;

public class UINewMailManager : MonoBehaviour
{
	public class MAILITEMINFO
	{
		public UtilUIMailInfo ui;

		public MailData data;

		public GameObject go;

		public MAILITEMINFO(GameObject go, MailData data)
		{
			this.go = go;
			this.data = data;
			ui = go.GetComponent<UtilUIMailInfo>();
		}
	}

	[SerializeField]
	private UtilUIPropertyInfo m_scriptUIPropertyInfo;

	[SerializeField]
	private GameObject noticeTitleNewMailTipsGO;

	[SerializeField]
	private UIGrid grid;

	[SerializeField]
	private AutoCreatByPrefab autoCreate;

	[SerializeField]
	private UILabel leftPartMailsCount;

	[SerializeField]
	private UILabel rightPartTitle;

	[SerializeField]
	private UILabel rightPartSender;

	[SerializeField]
	private UILabel rightPartContent;

	[SerializeField]
	private GameObject[] rightPartAccessoryGOS;

	[SerializeField]
	private UIImageButton refreshBtn;

	[SerializeField]
	private UIImageButton receiveAllBtn;

	[SerializeField]
	private UIImageButton receiveBtn;

	[SerializeField]
	private UIImageButton deleteBtn;

	protected Dictionary<string, MAILITEMINFO> dictMailInfo = new Dictionary<string, MAILITEMINFO>();

	protected Dictionary<GameObject, string> dictMapMailInfo = new Dictionary<GameObject, string>();

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
		RequestRefreshMailList();
		UpdatePropertyInfoPart("MAILBOX", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
		UpdateRightPartUI(null);
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

	public void UpdateNewMailCount(int newMailCount)
	{
		if (newMailCount > 0)
		{
			noticeTitleNewMailTipsGO.SetActive(true);
			noticeTitleNewMailTipsGO.transform.Find("Label").GetComponent<UILabel>().text = string.Empty + newMailCount;
		}
		else
		{
			noticeTitleNewMailTipsGO.SetActive(false);
		}
	}

	public void InitLeftPartInfo(Dictionary<string, MailData> dicts, int maxMailCount)
	{
		foreach (KeyValuePair<GameObject, string> item in dictMapMailInfo)
		{
			UnityEngine.Object.DestroyImmediate(item.Key);
		}
		dictMailInfo.Clear();
		dictMapMailInfo.Clear();
		foreach (KeyValuePair<string, MailData> dict in dicts)
		{
			SerializeItem(dict.Key, dict.Value.index, dict.Value);
			UpdateItemUI(dict.Key);
		}
		grid.repositionNow = true;
		leftPartMailsCount.text = "Inbox " + dictMailInfo.Count + "/" + maxMailCount;
		UpdateRightPartUI(null);
	}

	protected void SerializeItem(string _id, int _index, MailData data)
	{
		GameObject gameObject = autoCreate.CreatePefab(_index);
		MAILITEMINFO mAILITEMINFO = new MAILITEMINFO(gameObject, data);
		mAILITEMINFO.ui.BlindFuntion(HandleMailToggleValueChanged);
		dictMailInfo.Add(_id, mAILITEMINFO);
		dictMapMailInfo.Add(gameObject, _id);
	}

	protected void UpdateItemUI(string _id)
	{
		MAILITEMINFO mAILITEMINFO = dictMailInfo[_id];
		mAILITEMINFO.ui.SetMailBReadState(mAILITEMINFO.data.read);
		mAILITEMINFO.ui.SetAccessoryTipsVisable((mAILITEMINFO.data.dictItems.Count > 0 && mAILITEMINFO.data.actionReply == 0) ? true : false);
		mAILITEMINFO.ui.UpdateTitle(mAILITEMINFO.data.titel);
		DateTime dateTime = new DateTime(new DateTime(1970, 1, 1, 0, 0, 0).Ticks + TimeSpan.FromSeconds(mAILITEMINFO.data.dateSeconds).Ticks);
		mAILITEMINFO.ui.UpdateDate(dateTime.Year + "/" + dateTime.Month + "/" + dateTime.Day);
	}

	public void UpdateRightPartUI(MAILITEMINFO mii)
	{
		if (mii != null)
		{
			rightPartTitle.text = mii.data.titel;
			rightPartSender.text = "From: " + mii.data.sender;
			rightPartContent.text = mii.data.content;
			for (int i = 0; i < rightPartAccessoryGOS.Length; i++)
			{
				if (mii.data.actionReply == 0)
				{
					rightPartAccessoryGOS[i].transform.parent.gameObject.SetActive(true);
					if (i < mii.data.dictItems.Count)
					{
						rightPartAccessoryGOS[i].SetActive(true);
					}
					else
					{
						rightPartAccessoryGOS[i].SetActive(false);
					}
				}
				else
				{
					rightPartAccessoryGOS[i].transform.parent.gameObject.SetActive(false);
					rightPartAccessoryGOS[i].SetActive(false);
				}
			}
			int num = 0;
			foreach (KeyValuePair<Defined.COST_TYPE, int> dictItem in mii.data.dictItems)
			{
				UISprite component = rightPartAccessoryGOS[num].transform.Find("Icon").GetComponent<UISprite>();
				UILabel component2 = rightPartAccessoryGOS[num].transform.Find("Count").Find("Label").GetComponent<UILabel>();
				component.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(dictItem.Key);
				component2.text = "x" + dictItem.Value;
				num++;
			}
			if (mii.data.dictItems.Count > 0 && mii.data.actionReply == 0)
			{
				receiveBtn.gameObject.SetActive(true);
				deleteBtn.gameObject.SetActive(false);
			}
			else
			{
				receiveBtn.gameObject.SetActive(false);
				deleteBtn.gameObject.SetActive(true);
			}
		}
		else
		{
			rightPartTitle.text = string.Empty;
			rightPartSender.text = "From: ";
			rightPartContent.text = string.Empty;
			rightPartAccessoryGOS[0].transform.parent.gameObject.SetActive(false);
			receiveBtn.gameObject.SetActive(false);
			deleteBtn.gameObject.SetActive(false);
		}
	}

	public void RequestRefreshMailList()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 29);
		UIDialogManager.Instance.ShowBlock(29);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Mail_Get, OnGetMailListFinished);
	}

	public void OnGetMailListFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 29);
		UIDialogManager.Instance.HideBlock(29);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		InitLeftPartInfo(UIConstant.gDictMailData, UIConstant.gMaxMailCount);
		UpdateNewMailCount(UIConstant.gDictMailData.Count - UIConstant.glsMailReaded.Count);
	}

	public void RequestReceiveMailAccessory()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 30);
		UIDialogManager.Instance.ShowBlock(30);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Mail_ReceiveAccessory, OnReceiveMailAccessoryFinished);
	}

	public void OnReceiveMailAccessoryFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 30);
		UIDialogManager.Instance.HideBlock(30);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		for (int i = 0; i < DataCenter.State().selectMailMessageIDS.Length; i++)
		{
			string text = DataCenter.State().selectMailMessageIDS[i];
			MAILITEMINFO mii = dictMailInfo[text];
			UpdateItemUI(text);
			UpdateRightPartUI(mii);
		}
		UpdatePropertyInfoPart("MAILBOX", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
		if (UIConstant.gDictMailRewardStatistics.Count <= 0)
		{
			return;
		}
		string text2 = string.Empty;
		foreach (KeyValuePair<Defined.COST_TYPE, int> gDictMailRewardStatistic in UIConstant.gDictMailRewardStatistics)
		{
			string text3 = text2;
			text2 = text3 + gDictMailRewardStatistic.Key.ToString() + " x" + gDictMailRewardStatistic.Value + "\n";
		}
		UIDialogManager.Instance.ShowPopupA(text2, UIWidget.Pivot.Center, true);
	}

	public void RequestRemoveMail()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 31);
		UIDialogManager.Instance.ShowBlock(31);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Mail_Deleted, OnRemoveMailFinished);
	}

	public void OnRemoveMailFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 31);
		UIDialogManager.Instance.HideBlock(31);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		InitLeftPartInfo(UIConstant.gDictMailData, UIConstant.gMaxMailCount);
		UpdateNewMailCount(UIConstant.gDictMailData.Count - UIConstant.glsMailReaded.Count);
	}

	public void RequestMailReadedStateUpdate()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 32);
		UIDialogManager.Instance.ShowBlock(32);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Mail_Readed, OnMailReadedStateUpdateFinished);
	}

	public void OnMailReadedStateUpdateFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 32);
		UIDialogManager.Instance.HideBlock(32);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else
		{
			SceneManager.Instance.SwitchScene("UIBase");
		}
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
		RequestMailReadedStateUpdate();
	}

	public void HandleMailToggleValueChanged(GameObject go, bool value)
	{
		if (value)
		{
			string text = dictMapMailInfo[go];
			MAILITEMINFO mAILITEMINFO = dictMailInfo[text];
			mAILITEMINFO.data.read = true;
			if (!UIConstant.glsMailReaded.Contains(text))
			{
				UIConstant.glsMailReaded.Add(text);
			}
			DataCenter.State().selectMailMessageIDS = new string[1];
			DataCenter.State().selectMailMessageIDS[0] = text;
			UpdateItemUI(text);
			UpdateNewMailCount(UIConstant.gDictMailData.Count - UIConstant.glsMailReaded.Count);
			UpdateRightPartUI(mAILITEMINFO);
		}
	}

	public void HandleRefrshBtnClicked()
	{
		RequestRefreshMailList();
	}

	public void HandleReceiveAllMailAccessoryBtnClicked()
	{
		DataCenter.State().selectMailMessageIDS = new string[dictMailInfo.Count];
		int num = 0;
		foreach (KeyValuePair<string, MAILITEMINFO> item in dictMailInfo)
		{
			DataCenter.State().selectMailMessageIDS[num] = item.Key;
			num++;
		}
		RequestReceiveMailAccessory();
	}

	public void HandleReceiveMailAccessoryBtnClicked()
	{
		RequestReceiveMailAccessory();
	}

	public void HandleDeleteMailBtnClicked()
	{
		RequestRemoveMail();
	}
}
