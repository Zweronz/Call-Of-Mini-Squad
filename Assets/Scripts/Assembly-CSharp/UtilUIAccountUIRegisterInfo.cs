using UnityEngine;

public class UtilUIAccountUIRegisterInfo : MonoBehaviour
{
	public UIInput mEmailInput;

	public UIInput mPasswordInput;

	public UIInput mPasswordConfirmInput;

	public TweenPosition tweenPos;

	public UIWidget uiWidget;

	private bool gBackBtnJustHide;

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
		mEmailInput.label.maxLineCount = 1;
		mPasswordInput.label.maxLineCount = 1;
		mPasswordConfirmInput.label.maxLineCount = 1;
		mEmailInput.keyboardType = UIInput.KeyboardType.EmailAddress;
		mPasswordInput.keyboardType = UIInput.KeyboardType.ASCIICapable;
		mPasswordConfirmInput.keyboardType = UIInput.KeyboardType.ASCIICapable;
	}

	public void Init(bool backBtnJustHide)
	{
		gBackBtnJustHide = backBtnJustHide;
		mEmailInput.value = string.Empty;
		mPasswordInput.value = string.Empty;
		mPasswordConfirmInput.value = string.Empty;
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		base.transform.localPosition = tweenPos.from;
	}

	public void OnSubmitEmail()
	{
		string value = NGUIText.StripSymbols(mEmailInput.value);
		if (!string.IsNullOrEmpty(value))
		{
			mEmailInput.isSelected = false;
		}
	}

	public void OnSubmitPassword()
	{
		string value = NGUIText.StripSymbols(mPasswordInput.value);
		if (!string.IsNullOrEmpty(value))
		{
			mPasswordInput.isSelected = false;
		}
	}

	public void OnSubmitPasswordConfirm()
	{
		string value = NGUIText.StripSymbols(mPasswordInput.value);
		if (!string.IsNullOrEmpty(value))
		{
			mPasswordConfirmInput.isSelected = false;
		}
	}

	public void OnRegisterBtnClickedEvent()
	{
		if (!UtilUIAccountManager.IsEmail(mEmailInput.value))
		{
			UtilUIAccountManager.mInstance.ShowDialog("Invalid email address. Please enter again.", null);
			return;
		}
		if (mPasswordInput.value != mPasswordConfirmInput.value)
		{
			UtilUIAccountManager.mInstance.ShowDialog("Passwords entered are different. Please enter again.", null);
			return;
		}
		if (mPasswordInput.value.Length < 4 || mPasswordInput.value.Length > 10)
		{
			UtilUIAccountManager.mInstance.ShowDialog("Invalid password. Please enter again.", null);
			return;
		}
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 44);
		UIDialogManager.Instance.ShowBlock(44);
		string empty = string.Empty;
		empty = ((!(UtilUIAccountManager.mInstance.accountData.deviceAccountInfo.email == string.Empty)) ? string.Empty : UtilUIAccountManager.mInstance.accountData.deviceid);
		UtilUIAccountManager.mInstance.RequestRegister(empty, mEmailInput.value, mPasswordConfirmInput.value, OnRegisterFinished);
	}

	public void OnBackBtnClickedEvent()
	{
		UtilUIAccountManager.mInstance.HideAll();
		if (!gBackBtnJustHide)
		{
			UtilUIAccountManager.mInstance.ShowScene(UtilUIAccountManager.UIScene.E_LOGIN);
		}
	}

	public void OnRegisterFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 44);
		UIDialogManager.Instance.HideBlock(44);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			UtilUIAccountManager.mInstance.ShowDialog(code, null);
		}
		else if (gBackBtnJustHide)
		{
			UtilUIAccountManager.mInstance.HideScene(UtilUIAccountManager.UIScene.E_REGISTER);
			UtilUIAccountManager.mInstance.ShowDialog("Congratulation! Triniti Account has been registered!", null);
		}
		else
		{
			UtilUIAccountManager.mInstance.ShowDialog("Congratulation! Triniti Account has been registered!", UtilUIAccountManager.mInstance.AccountServerIsFinished);
		}
	}
}
