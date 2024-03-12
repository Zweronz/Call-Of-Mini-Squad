using UnityEngine;

public class UtilUIAccountUIRetriveInfo : MonoBehaviour
{
	public UIInput mEmailInput;

	public TweenPosition tweenPos;

	public UIWidget uiWidget;

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
		mEmailInput.keyboardType = UIInput.KeyboardType.EmailAddress;
	}

	public void Init()
	{
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		base.transform.localPosition = tweenPos.from;
	}

	public void OnRetriveBtnClickedEvent()
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 45);
		UIDialogManager.Instance.ShowBlock(45);
		string email = NGUIText.StripSymbols(mEmailInput.value);
		UtilUIAccountManager.mInstance.RequestRemindEmailPassword(email, OnRegisterFinished);
	}

	public void OnBackBtnClickedEvent()
	{
		UtilUIAccountManager.mInstance.HideAll();
		UtilUIAccountManager.mInstance.ShowScene(UtilUIAccountManager.UIScene.E_LOGIN);
	}

	public void OnRegisterFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 45);
		UIDialogManager.Instance.HideBlock(45);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			UtilUIAccountManager.mInstance.ShowDialog(code, null);
		}
		else
		{
			UtilUIAccountManager.mInstance.ShowDialog("Information has been sent to the email. Please have a check.", OnBackBtnClickedEvent);
		}
	}
}
