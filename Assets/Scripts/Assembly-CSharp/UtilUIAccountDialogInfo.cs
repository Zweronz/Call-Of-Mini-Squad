using UnityEngine;

public class UtilUIAccountDialogInfo : MonoBehaviour
{
	public UILabel label;

	private UtilUIAccountDialogInfo_OnEvent OnEvent;

	public void Hide()
	{
		base.gameObject.SetActive(false);
		OnEvent = null;
	}

	public void Show(string str, UtilUIAccountDialogInfo_OnEvent _eve)
	{
		label.text = str;
		OnEvent = _eve;
		base.gameObject.SetActive(true);
	}

	public void HandleBackBtnClick()
	{
		if (OnEvent != null)
		{
			OnEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		Hide();
	}
}
