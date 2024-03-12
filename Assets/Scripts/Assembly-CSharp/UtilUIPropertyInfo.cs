using UnityEngine;

public class UtilUIPropertyInfo : MonoBehaviour
{
	[SerializeField]
	private UIImageButton m_backBtn;

	[SerializeField]
	private UILabel m_nameLabel;

	[SerializeField]
	private UILabel m_rankLabel;

	[SerializeField]
	private UILabel m_goldLabel;

	[SerializeField]
	private UILabel m_crystalLabel;

	[SerializeField]
	private UIImageButton m_iapBtn;

	private UtilUIPropertyInfo_BackBtnClick_Delegate BackBtnClickEvent;

	private UtilUIPropertyInfo_IAPBtnClick_Delegate IAPBtnClickEvent;

	private void HandleEvent_BackBtnClicked()
	{
		if (BackBtnClickEvent != null)
		{
			BackBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_IAPBtnClicked()
	{
		if (IAPBtnClickEvent != null)
		{
			IAPBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void SetBackBtnVisable(bool bShow)
	{
		m_backBtn.gameObject.SetActive(bShow);
	}

	public void UpdateName(string str)
	{
		m_nameLabel.text = str;
	}

	public void UpdateRank(string str)
	{
		m_rankLabel.text = str;
	}

	public void UpdateGold(string str)
	{
		m_goldLabel.text = str;
	}

	public void UpdateCrystal(string str)
	{
		m_crystalLabel.text = str;
	}

	public void SetBackBtnClickDelegate(UtilUIPropertyInfo_BackBtnClick_Delegate dele)
	{
		BackBtnClickEvent = dele;
	}

	public void SetIAPBtnClickDelegate(UtilUIPropertyInfo_IAPBtnClick_Delegate dele)
	{
		IAPBtnClickEvent = dele;
	}
}
