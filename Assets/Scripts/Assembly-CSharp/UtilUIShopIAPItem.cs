using UnityEngine;

public class UtilUIShopIAPItem : MonoBehaviour
{
	[SerializeField]
	private UIToggle m_toggle;

	[SerializeField]
	private UISprite m_toggleBackground;

	[SerializeField]
	private UISprite m_toggleCheckmark;

	[SerializeField]
	private UISprite m_icon;

	[SerializeField]
	private UILabel m_nameLabel;

	[SerializeField]
	private UILabel m_limitLabel;

	[SerializeField]
	private UISprite m_priceIcon;

	[SerializeField]
	private UILabel m_priceLabel;

	[SerializeField]
	private GameObject rewardPartCurrencyGO1;

	[SerializeField]
	private UISprite rewardPartCurrencyGO1Icon;

	[SerializeField]
	private UILabel rewardPartCurrencyGO1Label;

	[SerializeField]
	private GameObject rewardPartCurrencyGO2;

	[SerializeField]
	private UISprite rewardPartCurrencyGO2Icon;

	[SerializeField]
	private UILabel rewardPartCurrencyGO2Label;

	[SerializeField]
	private GameObject rewardPartCurrencyGO3;

	[SerializeField]
	private UISprite rewardPartCurrencyGO3Icon;

	private UtilUIShopIAPItem_ItemToggleStateChange_Delegate ItemToggleStateChangeEvent;

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

	public void UpdateLimit(string str)
	{
		m_limitLabel.text = str;
	}

	public void UpdateUI(bool bIAP, string icon, string name, int limit, string price, int crystal, int money, int hero, int backgroundState)
	{
		if (backgroundState == 1)
		{
			m_toggleBackground.spriteName = "bg_frame054_L0R0T0B0";
			m_toggleCheckmark.spriteName = "bg_frame053_L0R0T0B0";
			m_nameLabel.color = new Color(0.20784314f, 0.45490196f, 73f / 85f);
		}
		else
		{
			m_toggleBackground.spriteName = "bg_frame055_L0R0T0B0";
			m_toggleCheckmark.spriteName = "bg_frame062_L0R0T0B0";
			m_nameLabel.color = new Color(14f / 15f, 0.6392157f, 0.11764706f);
		}
		m_icon.spriteName = icon;
		m_nameLabel.text = name;
		if (limit > 0)
		{
			UpdateLimit(UIUtil.GetCombinationString(UIUtil._UIGreenColor, "Limit: " + limit));
		}
		else if (limit == 0)
		{
			UpdateLimit(UIUtil.GetCombinationString(UIUtil._UIRedColor, "Limit: " + limit));
		}
		else
		{
			UpdateLimit(string.Empty);
		}
		if (bIAP)
		{
			m_priceIcon.gameObject.SetActive(false);
			m_priceLabel.text = price;
			UpdateRewardPart(crystal, money, hero);
		}
		else
		{
			m_priceIcon.gameObject.SetActive(true);
			m_priceLabel.text = crystal + string.Empty;
			UpdateRewardPart(0, money, hero);
		}
	}

	public void UpdateRewardPart(int crystal, int money, int hero)
	{
		rewardPartCurrencyGO1.SetActive(false);
		rewardPartCurrencyGO2.SetActive(false);
		rewardPartCurrencyGO3.SetActive(false);
		if (hero >= 0)
		{
			rewardPartCurrencyGO3.SetActive(true);
			DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(hero);
			rewardPartCurrencyGO3.SetActive(true);
			rewardPartCurrencyGO3Icon.spriteName = heroDataByIndex.iconFileName;
		}
		if (crystal > 0)
		{
			rewardPartCurrencyGO1.SetActive(true);
			rewardPartCurrencyGO1Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
			rewardPartCurrencyGO1Label.text = string.Empty + crystal;
		}
		if (money > 0)
		{
			rewardPartCurrencyGO2.SetActive(true);
			rewardPartCurrencyGO2Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Money);
			rewardPartCurrencyGO2Label.text = string.Empty + money;
		}
		if (crystal > 0 && money > 0)
		{
			rewardPartCurrencyGO1.transform.localPosition = new Vector3(-32f, 0f, 0f);
			rewardPartCurrencyGO2.transform.localPosition = new Vector3(-32f, -32f, 0f);
		}
		else if (crystal > 0)
		{
			rewardPartCurrencyGO1.transform.localPosition = new Vector3(0f, -22f, 0f);
		}
		else if (money > 0)
		{
			rewardPartCurrencyGO2.transform.localPosition = new Vector3(0f, -22f, 0f);
		}
	}

	public void SetItemToggleStateChangeDelegate(UtilUIShopIAPItem_ItemToggleStateChange_Delegate dele)
	{
		ItemToggleStateChangeEvent = dele;
	}

	public void Selected()
	{
		m_toggle.value = true;
	}
}
