using UnityEngine;

public class UtilUIReviewDialog : MonoBehaviour
{
	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void HandleButtonClickedEvent()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.trinitigame.android.callofminidoubleshot2");
		UIConstant.bNeedLoseConnect = false;
		Hide();
	}

	public void HandleCloseButtonClickedEvent()
	{
		UIConstant.bNeedLoseConnect = true;
		Hide();
	}
}
