using UnityEngine;

public class UtilUIDialogPopupBControl : MonoBehaviour
{
	[SerializeField]
	private UIImageButton lBtn;

	[SerializeField]
	private UILabel lBtnLabel;

	[SerializeField]
	private UIImageButton rBtn;

	[SerializeField]
	private UISprite rBtnIcon;

	[SerializeField]
	private UILabel rBtnLabel;

	[SerializeField]
	private UILabel msgLabel;

	[SerializeField]
	private GameObject blockGO;

	[SerializeField]
	private GameObject rewardPartGO;

	[SerializeField]
	private GameObject reward1GO;

	[SerializeField]
	private UISprite reward1Icon;

	[SerializeField]
	private UILabel reward1Label;

	[SerializeField]
	private GameObject reward2GO;

	[SerializeField]
	private UISprite reward2Icon;

	[SerializeField]
	private UILabel reward2Label;

	private UtilUIDialogPopupBControl_LBtnClicked_Delegate _lEvent;

	private UtilUIDialogPopupBControl_RBtnClicked_Delegate _rEvent;

	public void Init(string _str, UIWidget.Pivot _pivot, Defined.COST_TYPE r1CT, int r1Num, Defined.COST_TYPE r2CT, int r2Num, string _leftLabel, string _rightLabel, bool _rightIconVisable, UtilUIDialogPopupBControl_LBtnClicked_Delegate _lDele, UtilUIDialogPopupBControl_RBtnClicked_Delegate _rDele)
	{
		if (_leftLabel != "Cancel")
		{
		}
		_lEvent = _lDele;
		_rEvent = _rDele;
		rBtnLabel.text = _rightLabel;
		msgLabel.text = _str;
		msgLabel.pivot = _pivot;
		if ((bool)rBtnIcon)
		{
			rBtnIcon.gameObject.SetActive(true);
		}
		else
		{
			rBtnIcon.gameObject.SetActive(false);
		}
		reward1GO.SetActive(false);
		reward2GO.SetActive(false);
		if (r1Num > 0)
		{
			reward1Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(r1CT);
			reward1Label.text = string.Empty + r1Num;
			reward1GO.SetActive(true);
		}
		if (r2Num > 0)
		{
			reward2Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(r2CT);
			reward2Label.text = string.Empty + r2Num;
			reward2GO.SetActive(true);
		}
	}

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		blockGO.SetActive(bShow);
	}

	public void HandleLButtonClickedEvent()
	{
		SetVisable(false);
		if (_lEvent != null)
		{
			_lEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleRButtonClickedEvent()
	{
		SetVisable(false);
		if (_rEvent != null)
		{
			_rEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}
