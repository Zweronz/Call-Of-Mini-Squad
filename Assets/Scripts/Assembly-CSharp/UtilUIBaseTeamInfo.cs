using UnityEngine;

public class UtilUIBaseTeamInfo : MonoBehaviour
{
	[SerializeField]
	private new UILabel name;

	[SerializeField]
	private UILabel level;

	[SerializeField]
	private UILabel combat;

	[SerializeField]
	private UISprite expSlider;

	[SerializeField]
	private UISprite detailImageBG;

	[SerializeField]
	private UILabel detailLabel;

	[SerializeField]
	private GameObject detailGO;

	public void UpdateName(string str)
	{
		name.text = str;
	}

	public void UpdateLevel(string str)
	{
		level.text = str;
	}

	public void UpdateCombat(string str)
	{
		combat.text = str;
	}

	public void UpdateExpPercent(float own, float total)
	{
		float fillAmount = own / total;
		expSlider.fillAmount = fillAmount;
	}

	public void UpdateDetailInfo(int level, float own, float total)
	{
		string text = "Team Level:" + level + "\n";
		string text2 = "Exp required to reach the next level:\n" + own + " / " + total;
		detailLabel.text = text + text2;
	}

	public void UpdateDetailInfo(string detail)
	{
		detailLabel.text = detail;
	}

	public void HandleTeamBtnClickEvent()
	{
		SetDetailVisable(!detailGO.activeSelf);
	}

	public void SetDetailVisable(bool bShow)
	{
		detailGO.SetActive(bShow);
	}
}
