using UnityEngine;

public class UtilUIDialogPopupControl : MonoBehaviour
{
	[SerializeField]
	private UIImageButton btn;

	[SerializeField]
	private UIImageButton closeBtn;

	[SerializeField]
	private UILabel msgLabel;

	[SerializeField]
	private GameObject blockGO;

	[SerializeField]
	private UISprite btnIcon;

	[SerializeField]
	private UILabel btnLabel;

	private UtilUIDialogPopupControl_OKBtnClicked_Delegate _event;

	public void Init(string _str, UIWidget.Pivot _pivot, string btnIcon, string btnLabel, bool bShowCloseBtn, UtilUIDialogPopupControl_OKBtnClicked_Delegate _dele)
	{
		_event = _dele;
		msgLabel.text = _str;
		msgLabel.pivot = _pivot;
		this.btnIcon.gameObject.SetActive(btnIcon != string.Empty);
		this.btnIcon.spriteName = btnIcon;
		this.btnLabel.text = btnLabel;
		closeBtn.gameObject.SetActive(bShowCloseBtn);
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		blockGO.SetActive(bShow);
	}

	public void HandleButtonClickedEvent()
	{
		SetVisable(false);
		if (_event != null)
		{
			_event();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}
