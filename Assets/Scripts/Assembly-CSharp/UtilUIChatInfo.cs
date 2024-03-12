using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilUIChatInfo : MonoBehaviour
{
	public UIInput mInput;

	public UITextList textList;

	public bool fillWithDummyData;

	public bool bChatRoomIsUsing;

	public GameObject ChatBtnNewTipsGO;

	public float lastSendMsgTime;

	public void SetVisable(bool v)
	{
		bChatRoomIsUsing = v;
		base.gameObject.SetActive(v);
		if (bChatRoomIsUsing)
		{
			ChatBtnNewTipsGO.SetActive(false);
			if (UIConstant.gLsChatData.Count > 0)
			{
				UIConstant.gLastChatMsgTime = UIConstant.gLsChatData[UIConstant.gLsChatData.Count - 1].dateSeconds;
			}
		}
	}

	private void Start()
	{
		mInput.label.maxLineCount = 1;
		if (fillWithDummyData && textList != null)
		{
			for (int i = 0; i < 30; i++)
			{
				textList.Add(((i % 2 != 0) ? "[AA00AA]" : "[FFCCFF]") + "This is an example paragraph for the text list, testing line " + i + "[-]");
			}
		}
	}

	public void UpdateChatListInfo(List<ChatData> LScd)
	{
		textList.Clear();
		for (int i = 0; i < LScd.Count; i++)
		{
			AddChatInfo(LScd[i]);
		}
	}

	public void AddChatInfo(ChatData cd)
	{
		try
		{
			string empty = string.Empty;
			Color uIWhiteColor = UIUtil._UIWhiteColor;
			if (cd.userType == ChatData.EUSERTYPE.E_GM)
			{
				uIWhiteColor = UIUtil._UIGreenColor;
				empty = cd.userName + ": " + cd.msg;
			}
			else if (cd.userType == ChatData.EUSERTYPE.E_SystemInfo)
			{
				uIWhiteColor = UIUtil._UIRedColor;
				empty = cd.msg;
			}
			else
			{
				uIWhiteColor = UIUtil._UIWhiteColor;
				empty = cd.userName + ": " + cd.msg;
			}
			empty = UIUtil.GetCombinationString(uIWhiteColor, empty);
			textList.Add(empty);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void OnSubmit()
	{
		RequestSendChatMsg();
	}

	public void OnEnterBtnClickEvent()
	{
		RequestSendChatMsg();
	}

	public void RequestSendChatMsg()
	{
		try
		{
			if (Time.time - lastSendMsgTime < UIConstant.gSendMsgPerSeconds)
			{
				ChatData chatData = new ChatData();
				chatData.userType = ChatData.EUSERTYPE.E_SystemInfo;
				chatData.msg = "Please do not send messages frequently.";
				AddChatInfo(chatData);
				mInput.value = string.Empty;
				mInput.isSelected = false;
				return;
			}
			string text = NGUIText.StripSymbols(mInput.value);
			if (!string.IsNullOrEmpty(text))
			{
				UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 36);
				UIDialogManager.Instance.ShowBlock(36);
				DataCenter.State().selectArenaRankTypeByLanguage = UIUtil.GetProtocolLaguageCode();
				DataCenter.State().needSendMsgContent = text;
				HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Chat_SendMsg, OnSendChatMsgFinished);
				lastSendMsgTime = Time.time;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			mInput.value = string.Empty;
			mInput.isSelected = false;
		}
	}

	public void OnSendChatMsgFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 36);
		UIDialogManager.Instance.HideBlock(36);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		UpdateChatListInfo(UIConstant.gLsChatData);
		mInput.value = string.Empty;
		mInput.isSelected = false;
		if (UIConstant.gLsChatData.Count > 0)
		{
			UIConstant.gLastChatMsgTime = UIConstant.gLsChatData[UIConstant.gLsChatData.Count - 1].dateSeconds;
		}
	}
}
