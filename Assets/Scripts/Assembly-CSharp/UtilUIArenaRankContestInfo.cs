using UnityEngine;

public class UtilUIArenaRankContestInfo : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UILabel rank;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel city;

	[SerializeField]
	private GameObject[] rewardGOS;

	private UtilUIArenaRankContestInfo_GOCLICK_Delegate _event;

	public void UpdateBackground(bool bBlueType)
	{
		if (bBlueType)
		{
			UpdateBackground("bg_frame065__L10R10T10B10");
		}
		else
		{
			UpdateBackground("bg_frame064__L10R10T10B10");
		}
	}

	private void UpdateBackground(string _str)
	{
		background.spriteName = _str;
	}

	public void UpdateRank(string _str)
	{
		rank.text = _str;
	}

	public void UpdateIcon(string _str)
	{
		icon.spriteName = _str;
	}

	public void UpdateLv(string _str)
	{
		level.text = _str;
	}

	public void UpdateCity(string _str)
	{
		city.text = _str;
	}

	public void UpdateReward(int rHornor, int rCrystal, int rMoney)
	{
		int num = 0;
		if (rHornor > 0 && num < rewardGOS.Length)
		{
			UISprite component = rewardGOS[num].GetComponent<UISprite>();
			UILabel component2 = rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
			component.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
			component2.text = rHornor.ToString("###, ###");
			num++;
		}
		if (rCrystal > 0 && num < rewardGOS.Length)
		{
			UISprite component3 = rewardGOS[num].GetComponent<UISprite>();
			UILabel component4 = rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
			component3.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
			component4.text = rCrystal.ToString("###, ###");
			num++;
		}
		if (rMoney > 0 && num < rewardGOS.Length)
		{
			UISprite component5 = rewardGOS[num].GetComponent<UISprite>();
			UILabel component6 = rewardGOS[num].transform.Find("Label").GetComponent<UILabel>();
			component5.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Money);
			component6.text = rMoney.ToString("###, ###");
			num++;
		}
		if (num == 0)
		{
			rewardGOS[0].SetActive(false);
			rewardGOS[1].SetActive(false);
		}
		else
		{
			rewardGOS[0].SetActive(true);
			rewardGOS[1].SetActive(true);
		}
		switch (num)
		{
		case 1:
			rewardGOS[0].transform.localPosition = new Vector3(-50f, 0f, 0f);
			rewardGOS[1].SetActive(false);
			break;
		case 2:
			rewardGOS[0].transform.localPosition = new Vector3(-160f, 0f, 0f);
			rewardGOS[1].transform.localPosition = new Vector3(30f, 0f, 0f);
			break;
		}
	}

	public void BlindFuntion(UtilUIArenaRankContestInfo_GOCLICK_Delegate _dele)
	{
		_event = _dele;
	}

	public void HandleContestItemClickedEvent()
	{
		if (_event != null)
		{
			_event(base.gameObject);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}
