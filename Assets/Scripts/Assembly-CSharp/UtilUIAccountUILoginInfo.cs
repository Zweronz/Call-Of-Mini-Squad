using UnityEngine;

public class UtilUIAccountUILoginInfo : MonoBehaviour
{
	public UIInput mEmailInput;

	public UIInput mPasswordInput;

	public TweenPosition tweenPos;

	public UIWidget uiWidget;

	public UIImageButton quickStartBtn;

	private string email = string.Empty;

	private string password = string.Empty;

	private void Awake()
	{
		UIRoot component = NGUITools.GetRoot(base.gameObject).GetComponent<UIRoot>();
		tweenPos.to = new Vector3(0f, (float)(component.activeHeight - uiWidget.height) / 2f, 0f);
	}

	private void Update()
	{
		if (tweenPos.to == base.transform.localPosition && UIInput.selection == null)
		{
			tweenPos.PlayReverse();
		}
	}

	private void Start()
	{
		mEmailInput.value = UtilUIAccountManager.mInstance.accountData.email;
		mPasswordInput.value = UtilUIAccountManager.mInstance.accountData.password;
		mEmailInput.label.maxLineCount = 1;
		mPasswordInput.label.maxLineCount = 1;
		email = mEmailInput.value;
		password = mPasswordInput.value;
		mEmailInput.keyboardType = UIInput.KeyboardType.EmailAddress;
		mPasswordInput.keyboardType = UIInput.KeyboardType.ASCIICapable;
	}

	public void Init()
	{
	}

	public void CheckAutoLogin()
	{
		if (UtilUIAccountManager.mInstance.accountData.uuid != string.Empty)
		{
			UtilUIAccountManager.mInstance.AccountServerIsFinished();
		}
		else
		{
			UtilUIAccountManager.mInstance.accountData.email = UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.email;
		}
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		if (bShow)
		{
			if (UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.email != string.Empty)
			{
				quickStartBtn.gameObject.SetActive(false);
			}
			else
			{
				quickStartBtn.gameObject.SetActive(true);
			}
		}
		base.transform.localPosition = tweenPos.from;
	}

	public void OnSubmitEmail()
	{
		string text = NGUIText.StripSymbols(mEmailInput.value);
		email = text;
		mEmailInput.isSelected = false;
	}

	public void OnSubmitPassword()
	{
		string text = NGUIText.StripSymbols(mPasswordInput.value);
		password = text;
		mPasswordInput.isSelected = false;
	}

	public void OnForgetPasswordBtnClickedEvent()
	{
		UtilUIAccountManager.mInstance.HideAll();
		UtilUIAccountManager.mInstance.ShowScene(UtilUIAccountManager.UIScene.E_RETRIVE);
	}

	public void OnRegisterBtnClickedEvent()
	{
		UtilUIAccountManager.mInstance.HideAll();
		UtilUIAccountManager.mInstance.ShowScene(UtilUIAccountManager.UIScene.E_REGISTER);
		UtilUIAccountManager.mInstance.accountRegisterInfo.Init(false);
	}

	public void OnLoginBtnClickedEvent()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 42);
		UIDialogManager.Instance.ShowBlock(42);
		OnSubmitEmail();
		OnSubmitPassword();
		UtilUIAccountManager.mInstance.RequestUserLogin(UtilUIAccountManager.mInstance.accountData.deviceid, email, password, OnLoginFinished);
	}

	public void OnQuickStartBtnClickedEvent()
	{
		UtilUIAccountManager.mInstance.accountData.uuid = UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.uuid;
		UtilUIAccountManager.mInstance.accountData.email = UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.email;
		UtilUIAccountManager.mInstance.accountData.password = UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.password;
		UtilUIAccountManager.mInstance.AccountServerIsFinished();
	}

	public void OnLoginFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 42);
		UIDialogManager.Instance.HideBlock(42);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			UtilUIAccountManager.mInstance.ShowDialog(code, null);
		}
		else
		{
			UtilUIAccountManager.mInstance.AccountServerIsFinished();
		}
	}

	public void OnDeviceLoginFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 43);
		UIDialogManager.Instance.HideBlock(43);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			UtilUIAccountManager.mInstance.ShowDialog(code, null);
		}
		else
		{
			UtilUIAccountManager.mInstance.AccountServerIsFinished();
		}
	}
}
