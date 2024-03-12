using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine;

public class UtilUIAccountManager : MonoBehaviour
{
	public enum UIScene
	{
		E_LOGIN = 0,
		E_RETRIVE = 1,
		E_ACCOUNT = 2,
		E_REGISTER = 3,
		E_LOGOUT = 4
	}

	public static UtilUIAccountManager mInstance;

	private UtilUIAccountManager_AccountServerFinished mAccountServerFinishedEvent;

	private UtilUIAccountManager_GetTAccountInfoByLocalDeviceIdFinished mGetTAccountInfoByDeviceIdFinishEvent;

	public UtilUIAccountDialogInfo accountDialogInfo;

	public UtilUIAccountUILoginInfo accountLoginInfo;

	public UtilUIAccountUIRegisterInfo accountRegisterInfo;

	public UtilUIAccountUIRetriveInfo accountRetriveInfo;

	public UtilUIAccountUIAccountInfomationInfo accountInformaitionInfo;

	public UtilUIAccountUILogoutInfo accountLogoutInfo;

	public UtilAccountData accountData;

	public UtilUIAccountDataState accountDataState;

	private void Awake()
	{
		mInstance = this;
	}

	public void SetAccountDelegates(UtilUIAccountManager_AccountServerFinished _dele, UtilUIAccountManager_GetTAccountInfoByLocalDeviceIdFinished _dele2)
	{
		mAccountServerFinishedEvent = _dele;
		mGetTAccountInfoByDeviceIdFinishEvent = _dele2;
	}

	public void GetTAccountByLocalDeviceID()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 43);
		UIDialogManager.Instance.ShowBlock(43);
		mInstance.RequestGetTAccountInfoFromServerByDeviceID(mInstance.accountData.deviceid, OnDeviceLoginFinished);
	}

	public void OnDeviceLoginFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 43);
		UIDialogManager.Instance.HideBlock(43);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			mInstance.ShowDialog(code, null);
		}
		else if (mGetTAccountInfoByDeviceIdFinishEvent != null)
		{
			mGetTAccountInfoByDeviceIdFinishEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void Begin()
	{
		HideAll();
		ShowScene(UIScene.E_LOGIN);
		accountLoginInfo.CheckAutoLogin();
	}

	public void RequestGetTAccountInfoFromServerByDeviceID(string deviceId, HttpRequestHandle.OnRequestFinish callBack)
	{
		mInstance.accountDataState.deviceid = deviceId;
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Account_DeviceLogin, callBack);
	}

	public void RequestRegister(string deviceId, string email, string password, HttpRequestHandle.OnRequestFinish callBack)
	{
		mInstance.accountDataState.deviceid = deviceId;
		mInstance.accountDataState.email = email;
		mInstance.accountDataState.password = password;
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Account_Register, callBack);
	}

	public void RequestUserLogin(string deviceId, string email, string password, HttpRequestHandle.OnRequestFinish callBack)
	{
		mInstance.accountDataState.deviceid = deviceId;
		mInstance.accountDataState.email = email;
		mInstance.accountDataState.password = password;
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Account_UserLogin, callBack);
	}

	public void RequestRemindEmailPassword(string email, HttpRequestHandle.OnRequestFinish callBack)
	{
		mInstance.accountDataState.email = email;
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Account_remindEmailPassword, callBack);
	}

	public void AccountServerIsFinished()
	{
		HideAll();
		if (mAccountServerFinishedEvent != null)
		{
			mAccountServerFinishedEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowScene(UIScene _enum)
	{
		switch (_enum)
		{
		case UIScene.E_LOGIN:
			accountLoginInfo.SetVisable(true);
			break;
		case UIScene.E_REGISTER:
			accountRegisterInfo.SetVisable(true);
			break;
		case UIScene.E_RETRIVE:
			accountRetriveInfo.SetVisable(true);
			break;
		case UIScene.E_ACCOUNT:
			accountInformaitionInfo.SetVisable(true);
			break;
		case UIScene.E_LOGOUT:
			accountLogoutInfo.SetVisable(true);
			break;
		}
	}

	public void HideScene(UIScene _enum)
	{
		switch (_enum)
		{
		case UIScene.E_LOGIN:
			accountLoginInfo.SetVisable(false);
			break;
		case UIScene.E_REGISTER:
			accountRegisterInfo.SetVisable(false);
			break;
		case UIScene.E_RETRIVE:
			accountRetriveInfo.SetVisable(false);
			break;
		case UIScene.E_ACCOUNT:
			accountInformaitionInfo.SetVisable(false);
			break;
		case UIScene.E_LOGOUT:
			accountLogoutInfo.SetVisable(false);
			break;
		}
	}

	public void HideAll()
	{
		HideScene(UIScene.E_LOGIN);
		HideScene(UIScene.E_RETRIVE);
		HideScene(UIScene.E_ACCOUNT);
		HideScene(UIScene.E_REGISTER);
	}

	public void ShowDialog(int code, UtilUIAccountDialogInfo_OnEvent _event)
	{
		string str = string.Empty;
		switch (code)
		{
		case 1056:
			str = "mcs_deviceUserAlreadBoundAnotherUser.";
			break;
		case 1057:
			str = "mcs_deviceUserAlreadBoundThisUser.";
			break;
		case 1058:
			str = "Email has already been registered. Please enter again.";
			break;
		case 1059:
			str = "Account doesn't exist. Please check email again.";
			break;
		case 1060:
			str = "Incorrect Password.";
			break;
		case 1061:
			str = "Failed to send email. Please try again later.";
			break;
		}
		ShowDialog(str, _event);
	}

	public void ShowDialog(string _str, UtilUIAccountDialogInfo_OnEvent _event)
	{
		accountDialogInfo.Show(_str, _event);
	}

	public static bool IsEmail(string _str)
	{
		try
		{
			string pattern = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";
			Regex regex = new Regex(pattern);
			MailAddress mailAddress = new MailAddress(_str);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (mailAddress.Host.Contains(".") && mailAddress.Host[mailAddress.Host.Length - 1] != '.')
			{
				flag = true;
			}
			if (!_str.Contains(" "))
			{
				flag2 = true;
			}
			Match match = regex.Match(_str);
			if (match.Success)
			{
				flag3 = true;
			}
			if (flag3 && flag && flag2)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
