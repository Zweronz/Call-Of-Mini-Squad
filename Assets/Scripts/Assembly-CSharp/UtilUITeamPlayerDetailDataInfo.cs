using UnityEngine;

public class UtilUITeamPlayerDetailDataInfo : MonoBehaviour
{
	[SerializeField]
	private UISprite m_iconSprite;

	[SerializeField]
	private UILabel m_nameLabel;

	[SerializeField]
	private UILabel m_healthLabel;

	[SerializeField]
	private UILabel m_damageLabel;

	[SerializeField]
	private UILabel m_defenseLabel;

	[SerializeField]
	private UILabel m_critLabel;

	[SerializeField]
	private UILabel m_speedLabel;

	[SerializeField]
	private UILabel m_hitLabel;

	[SerializeField]
	private UILabel m_dodgeLabel;

	[SerializeField]
	private UILabel m_combatLabel;

	[SerializeField]
	private GameObject m_tipsGO;

	[SerializeField]
	private UISprite m_tipsIconSprite;

	[SerializeField]
	private UILabel m_tipsNameLabel;

	[SerializeField]
	private UILabel m_tipsIntroLabel;

	[SerializeField]
	private UITexture m_tipsSkillIconTexture;

	[SerializeField]
	private UITexture m_tipsWeaponIconTexture;

	[SerializeField]
	private UIImageButton m_pageStateBtn;

	private UtilUITeamPlayerDetailDataInfo_PageStateBtnClick_Delegate PageStateBtnClickEvent;

	public UIImageButton PageStateBtn
	{
		get
		{
			return m_pageStateBtn;
		}
	}

	public void UpdatePageStateBtn(bool bEnable)
	{
		PageStateBtn.isEnabled = bEnable;
		if (bEnable)
		{
			PageStateBtn.gameObject.transform.Find("Disable Click").gameObject.SetActive(false);
		}
		else
		{
			PageStateBtn.gameObject.transform.Find("Disable Click").gameObject.SetActive(true);
		}
	}

	private void HandleEvent_PageStateBtnClicked()
	{
		if (PageStateBtnClickEvent != null)
		{
			PageStateBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_PageStateDisableBtnClicked()
	{
		UIDialogManager.Instance.ShowDriftMsgInfoUI("Hero is locked");
	}

	public void UpdatePlayerIcon(string str)
	{
		m_iconSprite.spriteName = str;
	}

	public void UpdateName(string str)
	{
		m_nameLabel.text = str;
	}

	public void UpdateHealth(string str)
	{
		m_healthLabel.text = str;
	}

	public void UpdateDamage(string str)
	{
		m_damageLabel.text = str;
	}

	public void UpdateArmor(string str)
	{
		m_defenseLabel.text = str;
	}

	public void UpdateCrit(string str)
	{
		m_critLabel.text = str;
	}

	public void UpdateSpeed(string str)
	{
		m_speedLabel.text = str;
	}

	public void UpdateHit(string str)
	{
		m_hitLabel.text = str;
	}

	public void UpdateDodge(string str)
	{
		m_dodgeLabel.text = str;
	}

	public void UpdateCombat(string str)
	{
		m_combatLabel.text = str;
	}

	public void SetTipsVisable(bool bShow)
	{
		m_tipsGO.SetActive(bShow);
	}

	public bool GetTipsVisable()
	{
		return m_tipsGO.activeSelf;
	}

	public void UpdateTipsState(bool upperPart)
	{
		if (upperPart)
		{
			m_tipsGO.transform.localPosition = new Vector3(m_tipsGO.transform.localPosition.x, 107f, m_tipsGO.transform.localPosition.z);
		}
		else
		{
			m_tipsGO.transform.localPosition = new Vector3(m_tipsGO.transform.localPosition.x, -150f, m_tipsGO.transform.localPosition.z);
		}
	}

	public void UpdatTipsInfo(string icon, string name, string intro, Texture weaponTex, Texture skillTex)
	{
		m_tipsIconSprite.spriteName = icon;
		m_tipsNameLabel.text = name;
		m_tipsIntroLabel.text = intro;
		m_tipsSkillIconTexture.mainTexture = skillTex;
		m_tipsWeaponIconTexture.mainTexture = weaponTex;
	}

	public void SetPageStateBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_PageStateBtnClick_Delegate dele)
	{
		PageStateBtnClickEvent = dele;
	}
}
