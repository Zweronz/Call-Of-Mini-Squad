using UnityEngine;

public class UtilUIShopStoreItem : MonoBehaviour
{
	[SerializeField]
	private UIToggle m_toggle;

	[SerializeField]
	private UITexture m_icon;

	[SerializeField]
	private UILabel m_ownLabel;

	[SerializeField]
	private UISprite m_priceIcon;

	[SerializeField]
	private UILabel m_priceLabel;

	private UtilUIShopStoreItem_ItemToggleStateChange_Delegate ItemToggleStateChangeEvent;

	public void HandleEvent_ItemToggleStateChange()
	{
		if (ItemToggleStateChangeEvent != null)
		{
			ItemToggleStateChangeEvent(base.gameObject, m_toggle.value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void SetBChecked(bool bCheck)
	{
		m_toggle.value = bCheck;
	}

	public void UpdateOwnCount(string str)
	{
		m_ownLabel.text = str;
	}

	public void UpdateUI(Texture icon, string own, string priceIcon, string price)
	{
		m_icon.mainTexture = icon;
		m_priceIcon.spriteName = priceIcon;
		m_priceLabel.text = price;
		UpdateOwnCount(own);
	}

	public void SetItemToggleStateChangeDelegate(UtilUIShopStoreItem_ItemToggleStateChange_Delegate dele)
	{
		ItemToggleStateChangeEvent = dele;
	}
}
