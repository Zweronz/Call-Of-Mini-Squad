using UnityEngine;

public class CurrencyMoneyInfo : MonoBehaviour
{
	public Defined.COST_TYPE m_enCostType = Defined.COST_TYPE.Crystal;

	public UILabel label;

	public UISprite icon;

	public bool resetIconPixelPerfect;

	private void Start()
	{
		icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(m_enCostType);
		if (resetIconPixelPerfect)
		{
			icon.MakePixelPerfect();
		}
	}

	private void Update()
	{
		UpdateLabel(GetCurrencyMoneyCount(m_enCostType));
	}

	private void UpdateLabel(int num)
	{
		if (label.text != num.ToString())
		{
			label.text = string.Empty + num;
		}
	}

	private int GetCurrencyMoneyCount(Defined.COST_TYPE ct)
	{
		int result = 0;
		switch (ct)
		{
		case Defined.COST_TYPE.Crystal:
			result = DataCenter.Save().Crystal;
			break;
		case Defined.COST_TYPE.Element:
			result = DataCenter.Save().Element;
			break;
		case Defined.COST_TYPE.Money:
			result = DataCenter.Save().Money;
			break;
		default:
			result = -9999;
			break;
		case Defined.COST_TYPE.Honor:
		case Defined.COST_TYPE.Radar:
			break;
		}
		return result;
	}
}
