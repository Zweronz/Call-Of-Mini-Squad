using System.Collections.Generic;
using UnityEngine;

public class UtilUITeamPlayerDetailExDataInfo : MonoBehaviour
{
	public enum PropertyIndex
	{
		E_level = 0,
		E_hp = 1,
		E_def = 2,
		E_proCrit = 3,
		E_critDamagePercent = 4,
		E_proResilience = 5,
		E_proHit = 6,
		E_hpPercent = 7,
		E_proDodge = 8,
		E_proStab = 9,
		E_atkPercent = 10,
		E_atkFrequencyPercent = 11,
		E_reduceDamagePercent = 12,
		E_moveSpeedPercent = 13,
		E_atkRange = 14
	}

	[SerializeField]
	private UIToggle[] m_titlesToggle;

	[SerializeField]
	private GameObject[] m_titlesToggleTips;

	[SerializeField]
	public UIPanel[] m_conentsPanel;

	[SerializeField]
	private GameObject m_levelUpEffectPrefab;

	[SerializeField]
	private GameObject m_breakEffectPrefab;

	[SerializeField]
	private GameObject m_unlockEffectPrefab;

	private UtilUITeamPlayerDetailDataInfo_TitleToggleStateChange_Delegate TitleToggleStateChangeEvent;

	[SerializeField]
	private UILabel m_wsWeaponNameLabel;

	[SerializeField]
	private UITexture m_wsWeaponIconTexture;

	[SerializeField]
	private UILabel m_wsWeaponCombatLabel;

	[SerializeField]
	private GameObject m_wsWeaponStarsGO;

	[SerializeField]
	private UILabel m_wsWeaponLevelLabel;

	[SerializeField]
	private UILabel m_wsWeaponIncrementLevelLabel;

	[SerializeField]
	private UILabel m_wsWeaponDamageLabel;

	[SerializeField]
	private UILabel m_wsWeaponIncrementDamageLabel;

	[SerializeField]
	private UILabel m_wsWeaponAmmoLabel;

	[SerializeField]
	private UILabel m_wsWeaponIncrementAmmoLabel;

	[SerializeField]
	private UILabel m_wsWeaponRPMLabel;

	[SerializeField]
	private UILabel m_wsWeaponIncrementRPMLabel;

	[SerializeField]
	private GameObject m_wsWeaponGoldGO;

	[SerializeField]
	private UILabel m_wsWeaponGoldLabel;

	[SerializeField]
	private GameObject m_wsWeaponItemGO;

	[SerializeField]
	private UILabel m_wsWeaponItemLabel;

	[SerializeField]
	private UIImageButton m_wsWeaponUpgradeBtn;

	[SerializeField]
	private UIImageButton m_wsWeaponBreakBtn;

	[SerializeField]
	private UIImageButton m_wsWeaponBreakFailBtn;

	private UtilUITeamPlayerDetailExDataInfo_WSWeaponUpgradeBtnClicked_Delegate WSWeaponUpgradeBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate WSWeaponBreakBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate WSWeaponBreakFailBtnClickEvent;

	[SerializeField]
	private UILabel m_wsSkillNameLabel;

	[SerializeField]
	private UITexture m_wsSkillIconTexture;

	[SerializeField]
	private GameObject m_wsSkillStarsGO;

	[SerializeField]
	private UILabel m_wsSkillLevelLabel;

	[SerializeField]
	private UILabel m_wsSkillIntroLabel;

	[SerializeField]
	private GameObject m_wsSkillGoldGO;

	[SerializeField]
	private UILabel m_wsSkillGoldLabel;

	[SerializeField]
	private GameObject m_wsSkillItemGO;

	[SerializeField]
	private UILabel m_wsSkillItemLabel;

	[SerializeField]
	private UIImageButton m_wsSkillUpgradeBtn;

	[SerializeField]
	private UIImageButton m_wsSkillBreakBtn;

	[SerializeField]
	private UIImageButton m_wsSkillBreakFailBtn;

	private UtilUITeamPlayerDetailExDataInfo_WSSkillUpgradeBtnClicked_Delegate WSSkillUpgradeBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate WSSkillBreakBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate WSSkillBreakFailBtnClickEvent;

	[SerializeField]
	private GameObject[] m_helmsArrTreeContainerGOS;

	[SerializeField]
	private GameObject[] m_helmsArrTreeLinesLockedStateGO;

	[SerializeField]
	private UILabel m_helmsDetailNameLabel;

	[SerializeField]
	private UITexture m_helmsDetailIconTexture;

	[SerializeField]
	private UILabel m_helmsDetailCombatLabel;

	[SerializeField]
	private List<GameObject> lsHelmsPropertys;

	[SerializeField]
	private GameObject m_helmsDetailLevelStarsGO;

	[SerializeField]
	private UILabel m_helmsDetailIncrementLevelStarsLabel;

	[SerializeField]
	private GameObject m_helmsDetailUpgradePartGO;

	[SerializeField]
	private UILabel m_helmsDetailUpgradePartGoldLabel;

	[SerializeField]
	private UIImageButton m_helmsDetailUpgradePartUpgradeBtn;

	[SerializeField]
	private GameObject m_helmsDetailUnlockPartGO;

	[SerializeField]
	private UILabel m_helmsDetailUnlockPartGoldLabel;

	[SerializeField]
	private UISprite m_helmsDetailUnlockPartItemImg;

	[SerializeField]
	private UILabel m_helmsDetailUnlockPartItemLabel;

	[SerializeField]
	private UIImageButton m_helmsDetailUnlockPartUnlockBtn;

	[SerializeField]
	private GameObject m_helmsDetailEquipPartGO;

	[SerializeField]
	private UIImageButton m_helmsDetailEquipPartEquipBtn;

	private UtilUITeamPlayerDetailDataInfo_HelmsDetailUpgradeBtnClicked_Delegate HelmsDetailUpgradeBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_HelmsDetailUnlockBtnClicked_Delegate HelmsDetailUnlockBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_HelmsDetailEquipBtnClicked_Delegate HelmsDetailEquipBtnClickEvent;

	[SerializeField]
	private GameObject[] m_armorArrTreeContainerGOS;

	[SerializeField]
	private GameObject[] m_armorArrTreeLinesLockedStateGO;

	[SerializeField]
	private UILabel m_armorDetailNameLabel;

	[SerializeField]
	private UITexture m_armorDetailIconTexture;

	[SerializeField]
	private UILabel m_armorDetailCombatLabel;

	[SerializeField]
	private List<GameObject> lsArmorPropertys;

	[SerializeField]
	private GameObject m_armorDetailLevelStarsGO;

	[SerializeField]
	private UILabel m_armorDetailIncrementLevelStarsLabel;

	[SerializeField]
	private GameObject m_armorDetailUpgradePartGO;

	[SerializeField]
	private UILabel m_armorDetailUpgradePartGoldLabel;

	[SerializeField]
	private UIImageButton m_armorDetailUpgradePartUpgradeBtn;

	[SerializeField]
	private GameObject m_armorDetailUnlockPartGO;

	[SerializeField]
	private UILabel m_armorDetailUnlockPartGoldLabel;

	[SerializeField]
	private UISprite m_armorDetailUnlockPartItemImg;

	[SerializeField]
	private UILabel m_armorDetailUnlockPartItemLabel;

	[SerializeField]
	private UIImageButton m_armorDetailUnlockPartUnlockBtn;

	[SerializeField]
	private GameObject m_armorDetailEquipPartGO;

	[SerializeField]
	private UIImageButton m_armorDetailEquipPartEquipBtn;

	private UtilUITeamPlayerDetailDataInfo_ArmorDetailUpgradeBtnClicked_Delegate ArmorDetailUpgradeBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_ArmorDetailUnlockBtnClicked_Delegate ArmorDetailUnlockBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_ArmorDetailEquipBtnClicked_Delegate ArmorDetailEquipBtnClickEvent;

	[SerializeField]
	private GameObject[] m_ornamentsArrTreeContainerGOS;

	[SerializeField]
	private GameObject[] m_ornamentsArrTreeLinesLockedStateGO;

	[SerializeField]
	private UILabel m_ornamentsDetailNameLabel;

	[SerializeField]
	private UITexture m_ornamentsDetailIconTexture;

	[SerializeField]
	private UILabel m_ornamentsDetailCombatLabel;

	[SerializeField]
	private List<GameObject> lsOrnamentsPropertys;

	[SerializeField]
	private GameObject m_ornamentsDetailLevelStarsGO;

	[SerializeField]
	private UILabel m_ornamentsDetailIncrementLevelStarsLabel;

	[SerializeField]
	private GameObject m_ornamentsDetailUpgradePartGO;

	[SerializeField]
	private UILabel m_ornamentsDetailUpgradePartGoldLabel;

	[SerializeField]
	private UIImageButton m_ornamentsDetailUpgradePartUpgradeBtn;

	[SerializeField]
	private GameObject m_ornamentsDetailUnlockPartGO;

	[SerializeField]
	private UILabel m_ornamentsDetailUnlockPartGoldLabel;

	[SerializeField]
	private UISprite m_ornamentsDetailUnlockPartItemImg;

	[SerializeField]
	private UILabel m_ornamentsDetailUnlockPartItemLabel;

	[SerializeField]
	private UIImageButton m_ornamentsDetailUnlockPartUnlockBtn;

	[SerializeField]
	private GameObject m_ornamentsDetailEquipPartGO;

	[SerializeField]
	private UIImageButton m_ornamentsDetailEquipPartEquipBtn;

	private UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUpgradeBtnClicked_Delegate OrnamentsDetailUpgradeBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUnlockBtnClicked_Delegate OrnamentsDetailUnlockBtnClickEvent;

	private UtilUITeamPlayerDetailDataInfo_OrnamentsDetailEquipBtnClicked_Delegate OrnamentsDetailEquipBtnClickEvent;

	public void HideAllToggleTips()
	{
		SetToggleTipsVisable(0, false);
		SetToggleTipsVisable(1, false);
		SetToggleTipsVisable(2, false);
		SetToggleTipsVisable(3, false);
	}

	public void SetToggleTipsVisable(int index, bool bShow)
	{
		m_titlesToggleTips[index].SetActive(bShow);
	}

	public void HandleEvent_TeamPlayer0ToggleStateChange()
	{
		if (TitleToggleStateChangeEvent != null)
		{
			TitleToggleStateChangeEvent(0, m_titlesToggle[0].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleEvent_TeamPlayer1ToggleStateChange()
	{
		if (TitleToggleStateChangeEvent != null)
		{
			TitleToggleStateChangeEvent(1, m_titlesToggle[1].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleEvent_TeamPlayer2ToggleStateChange()
	{
		if (TitleToggleStateChangeEvent != null)
		{
			TitleToggleStateChangeEvent(2, m_titlesToggle[2].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleEvent_TeamPlayer3ToggleStateChange()
	{
		if (TitleToggleStateChangeEvent != null)
		{
			TitleToggleStateChangeEvent(3, m_titlesToggle[3].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void SetEffectVisable(Transform parentTrans, UIEffectManager.EffectType type)
	{
		UIEffectManager.Instance.ShowEffectParticle(parentTrans, type);
	}

	public void SetTitleChecked(int index)
	{
		m_titlesToggle[index].value = true;
	}

	public void SetTitleToggleStateChangeDelegate(UtilUITeamPlayerDetailDataInfo_TitleToggleStateChange_Delegate dele)
	{
		TitleToggleStateChangeEvent = dele;
	}

	private void HandleEvent_WSWeaponUpgradeBtnClicked()
	{
		if (WSWeaponUpgradeBtnClickEvent != null)
		{
			WSWeaponUpgradeBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_WSWeaponBreakBtnClicked()
	{
		if (WSWeaponBreakBtnClickEvent != null)
		{
			WSWeaponBreakBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_WSWeaponBreakFailedBtnClicked()
	{
		if (WSWeaponBreakFailBtnClickEvent != null)
		{
			WSWeaponBreakFailBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowWeaponEffect(UIEffectManager.EffectType _eT)
	{
		SetEffectVisable(m_wsWeaponIconTexture.transform, _eT);
	}

	public void UpdateWSWeaponName(string str)
	{
		m_wsWeaponNameLabel.text = str;
	}

	public void UpdateWSWeaponIcon(Texture tex)
	{
		m_wsWeaponIconTexture.mainTexture = tex;
	}

	public void UpdateWSWeaponCombat(string str)
	{
		m_wsWeaponCombatLabel.text = str;
	}

	public void UpdateWSWeaponStars(int showCount)
	{
		UIUtil.ShowStars(m_wsWeaponStarsGO.transform, showCount);
	}

	public void UpdateWSWeaponLevel(string str)
	{
		m_wsWeaponLevelLabel.text = str;
	}

	public void UpdateWSWeaponIncrementLevel(string str)
	{
		m_wsWeaponIncrementLevelLabel.text = str;
		m_wsWeaponIncrementLevelLabel.color = UIUtil._UIGreenColor;
	}

	public void UpdateWSWeaponDamage(string str)
	{
		m_wsWeaponDamageLabel.text = str;
	}

	public void UpdateWSWeaponIncrementDamage(string str)
	{
		m_wsWeaponIncrementDamageLabel.text = str;
		m_wsWeaponIncrementDamageLabel.color = UIUtil._UIGreenColor;
	}

	public void UpdateWSWeaponAmmo(string str, int fontSize)
	{
		m_wsWeaponAmmoLabel.text = str;
		m_wsWeaponAmmoLabel.fontSize = fontSize;
	}

	public void UpdateWSWeaponIncrementAmmo(string str)
	{
		m_wsWeaponIncrementAmmoLabel.text = str;
		m_wsWeaponIncrementAmmoLabel.color = UIUtil._UIGreenColor;
	}

	public void UpdateWSWeaponRPM(string str)
	{
		m_wsWeaponRPMLabel.text = str;
	}

	public void UpdateWSWeaponIncrementRPM(string str)
	{
		m_wsWeaponIncrementRPMLabel.text = str;
		m_wsWeaponIncrementRPMLabel.color = UIUtil._UIGreenColor;
	}

	public void SetWSWeaponGoldVisable(bool bShow)
	{
		m_wsWeaponGoldGO.SetActive(bShow);
	}

	public void UpdateWSWeaponGoldPrice(string str)
	{
		m_wsWeaponGoldLabel.text = str;
	}

	public void SetWSWeaponItemVisable(bool bShow, bool bCrystal)
	{
		m_wsWeaponItemGO.SetActive(bShow);
		if (bCrystal)
		{
			m_wsWeaponItemGO.transform.Find("Sprite").GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
		}
		else
		{
			m_wsWeaponItemGO.transform.Find("Sprite").GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
		}
	}

	public void UpdateWSWeaponItemPrice(string str)
	{
		m_wsWeaponItemLabel.text = str;
	}

	public void SetWSWeaponUpgradeBtnVisable(bool bShow)
	{
		m_wsWeaponUpgradeBtn.gameObject.SetActive(bShow);
	}

	public void SetWSWeaponBreakBtnVisable(bool bShow)
	{
		m_wsWeaponBreakBtn.gameObject.SetActive(bShow);
	}

	public void SetWSWeaponBreakFailBtnVisable(bool bShow)
	{
		m_wsWeaponBreakFailBtn.gameObject.SetActive(bShow);
	}

	public void SetWSWeaponUpgradeBtnClickDelegate(UtilUITeamPlayerDetailExDataInfo_WSWeaponUpgradeBtnClicked_Delegate dele)
	{
		WSWeaponUpgradeBtnClickEvent = dele;
	}

	public void SetWSWeaponBreakBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate dele, UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate failDele)
	{
		WSWeaponBreakBtnClickEvent = dele;
		WSWeaponBreakFailBtnClickEvent = failDele;
	}

	private void HandleEvent_WSSkillUpgradeBtnClicked()
	{
		if (WSSkillUpgradeBtnClickEvent != null)
		{
			WSSkillUpgradeBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_WSSkillBreakBtnClicked()
	{
		if (WSSkillBreakBtnClickEvent != null)
		{
			WSSkillBreakBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_WSSkillBreakFailBtnClicked()
	{
		if (WSSkillBreakFailBtnClickEvent != null)
		{
			WSSkillBreakFailBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowSkillEffect(UIEffectManager.EffectType _eT)
	{
		SetEffectVisable(m_wsSkillIconTexture.transform, _eT);
	}

	public void UpdateWSSkillName(string str)
	{
		m_wsSkillNameLabel.text = str;
	}

	public void UpdateWSSkillIcon(Texture tex)
	{
		m_wsSkillIconTexture.mainTexture = tex;
	}

	public void UpdateWSSkillStars(int showCount)
	{
		UIUtil.ShowStars(m_wsSkillStarsGO.transform, showCount);
	}

	public void UpdateWSSkillLevel(string str)
	{
		m_wsSkillLevelLabel.text = str;
	}

	public void UpdateWSSkillIntroduce(string str)
	{
		m_wsSkillIntroLabel.text = str;
	}

	public void SetWSSkillGoldVisable(bool bShow)
	{
		m_wsSkillGoldGO.SetActive(bShow);
	}

	public void UpdateWSSkillGoldPrice(string str)
	{
		m_wsSkillGoldLabel.text = str;
	}

	public void SetWSSkillItemVisable(bool bShow, bool bCrystal)
	{
		m_wsSkillItemGO.SetActive(bShow);
		if (bCrystal)
		{
			m_wsSkillItemGO.transform.Find("Sprite").GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
		}
		else
		{
			m_wsSkillItemGO.transform.Find("Sprite").GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
		}
	}

	public void UpdateWSSkillItemPrice(string str)
	{
		m_wsSkillItemLabel.text = str;
	}

	public void SetWSSkillUpgradeBtnVisable(bool bShow)
	{
		m_wsSkillUpgradeBtn.gameObject.SetActive(bShow);
	}

	public void SetWSSkillBreakBtnVisable(bool bShow)
	{
		m_wsSkillBreakBtn.gameObject.SetActive(bShow);
	}

	public void SetWSSkillBreakFailBtnVisable(bool bShow)
	{
		m_wsSkillBreakFailBtn.gameObject.SetActive(bShow);
	}

	public void SetWSSkillUpgradeBtnClickDelegate(UtilUITeamPlayerDetailExDataInfo_WSSkillUpgradeBtnClicked_Delegate dele)
	{
		WSSkillUpgradeBtnClickEvent = dele;
	}

	public void SetWSSKillBreakBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate dele, UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate failDele)
	{
		WSSkillBreakBtnClickEvent = dele;
		WSSkillBreakFailBtnClickEvent = failDele;
	}

	private void HandleEvent_HelmsUpgradeBtnClicked()
	{
		if (HelmsDetailUpgradeBtnClickEvent != null)
		{
			HelmsDetailUpgradeBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_HelmsUnlockBtnClicked()
	{
		if (HelmsDetailUnlockBtnClickEvent != null)
		{
			HelmsDetailUnlockBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_HelmsEquipBtnClicked()
	{
		if (HelmsDetailEquipBtnClickEvent != null)
		{
			HelmsDetailEquipBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowHelmsEffect(Transform _trans, UIEffectManager.EffectType _eT)
	{
		if (_trans != null)
		{
			SetEffectVisable(_trans, _eT);
		}
		else
		{
			SetEffectVisable(m_helmsDetailIconTexture.transform, _eT);
		}
	}

	public void SetHelmsTreeLockedStateVisable(int index, bool bShow, int unlockLevel)
	{
		m_helmsArrTreeLinesLockedStateGO[index].SetActive(bShow);
		m_helmsArrTreeLinesLockedStateGO[index].transform.Find("Label").GetComponent<UILabel>().text = "UNLOCK AT TEAM LEVEL " + unlockLevel;
	}

	public void UpdateHelmsDetailName(string str)
	{
		m_helmsDetailNameLabel.text = str;
	}

	public void UpdateHelmsDetailIcon(Texture tex)
	{
		m_helmsDetailIconTexture.mainTexture = tex;
	}

	public void UpdateHelmsDetailCombat(string str)
	{
		m_helmsDetailCombatLabel.text = str;
	}

	public void RepositionHelms()
	{
		lsHelmsPropertys[0].transform.parent.GetComponent<UIGrid>().repositionNow = true;
	}

	public void SetAllHelmsPropertyVisable(bool bShow)
	{
		for (int i = 0; i < lsHelmsPropertys.Count; i++)
		{
			SetHelmsPropertyVisable((PropertyIndex)i, bShow);
		}
	}

	public void SetHelmsPropertyVisable(PropertyIndex pi, bool bShow)
	{
		lsHelmsPropertys[(int)pi].SetActive(bShow);
	}

	public void UpdateHelmsDetailProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			UIUtil.ShowStars(m_helmsDetailLevelStarsGO.transform, int.Parse(str));
		}
		else
		{
			lsHelmsPropertys[(int)pi].transform.Find("Value").GetComponent<UILabel>().text = str;
		}
	}

	public void UpdateHelmsDetailIncrementProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			m_helmsDetailIncrementLevelStarsLabel.text = str;
			m_helmsDetailIncrementLevelStarsLabel.color = UIUtil._UIGreenColor;
		}
		else if (str == string.Empty)
		{
			lsHelmsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = string.Empty;
		}
		else
		{
			lsHelmsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = "↑ " + str;
			lsHelmsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().color = UIUtil._UIGreenColor;
		}
	}

	public void SetHelmsDetailUpgradePartVisable(bool bShow)
	{
		m_helmsDetailUpgradePartGO.SetActive(bShow);
	}

	public void UpdateHelmsDetailUpgradePartGoldPrice(string str)
	{
		m_helmsDetailUpgradePartGoldLabel.text = str;
	}

	public void SetHelmsDetailUnlockPartVisable(bool bShow)
	{
		m_helmsDetailUnlockPartGO.SetActive(bShow);
	}

	public void SetHelmsDetailUnlockPartEnable(bool bEnable)
	{
		m_helmsDetailUnlockPartUnlockBtn.isEnabled = bEnable;
	}

	public void UpdateHelmsDetailUnlockPartGoldPrice(string str)
	{
		m_helmsDetailUnlockPartGoldLabel.text = str;
	}

	public void UpdateHelmsDetailUnlockPartItemPrice(string str, bool bCrystal)
	{
		m_helmsDetailUnlockPartItemLabel.text = str;
		if (bCrystal)
		{
			m_helmsDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
		}
		else
		{
			m_helmsDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
		}
	}

	public void SetHelmsDetailEquipPartVisable(bool bShow)
	{
		m_helmsDetailEquipPartGO.SetActive(bShow);
	}

	public void SetHelmsDetailUpgradeBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_HelmsDetailUpgradeBtnClicked_Delegate dele)
	{
		HelmsDetailUpgradeBtnClickEvent = dele;
	}

	public void SetHelmsDetailUnlockBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_HelmsDetailUnlockBtnClicked_Delegate dele)
	{
		HelmsDetailUnlockBtnClickEvent = dele;
	}

	public void SetHelmsDetailEquipBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_HelmsDetailEquipBtnClicked_Delegate dele)
	{
		HelmsDetailEquipBtnClickEvent = dele;
	}

	private void HandleEvent_ArmorUpgradeBtnClicked()
	{
		if (ArmorDetailUpgradeBtnClickEvent != null)
		{
			ArmorDetailUpgradeBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_ArmorUnlockBtnClicked()
	{
		if (ArmorDetailUnlockBtnClickEvent != null)
		{
			ArmorDetailUnlockBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_ArmorEquipBtnClicked()
	{
		if (ArmorDetailEquipBtnClickEvent != null)
		{
			ArmorDetailEquipBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowArmorEffect(Transform _trans, UIEffectManager.EffectType _eT)
	{
		if (_trans != null)
		{
			SetEffectVisable(_trans, _eT);
		}
		else
		{
			SetEffectVisable(m_armorDetailIconTexture.transform, _eT);
		}
	}

	public void SetArmorTreeLockedStateVisable(int index, bool bShow, int unlockLevel)
	{
		m_armorArrTreeLinesLockedStateGO[index].SetActive(bShow);
		m_armorArrTreeLinesLockedStateGO[index].transform.Find("Label").GetComponent<UILabel>().text = "UNLOCK AT TEAM LEVEL " + unlockLevel;
	}

	public void UpdateArmorDetailName(string str)
	{
		m_armorDetailNameLabel.text = str;
	}

	public void UpdateArmorDetailIcon(Texture tex)
	{
		m_armorDetailIconTexture.mainTexture = tex;
	}

	public void UpdateArmorDetailCombat(string str)
	{
		m_armorDetailCombatLabel.text = str;
	}

	public void RepositionArmor()
	{
		lsArmorPropertys[0].transform.parent.GetComponent<UIGrid>().repositionNow = true;
	}

	public void SetAllArmorPropertyVisable(bool bShow)
	{
		for (int i = 0; i < lsArmorPropertys.Count; i++)
		{
			SetArmorPropertyVisable((PropertyIndex)i, bShow);
		}
	}

	public void SetArmorPropertyVisable(PropertyIndex pi, bool bShow)
	{
		lsArmorPropertys[(int)pi].SetActive(bShow);
	}

	public void UpdateArmorDetailProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			UIUtil.ShowStars(m_armorDetailLevelStarsGO.transform, int.Parse(str));
		}
		else
		{
			lsArmorPropertys[(int)pi].transform.Find("Value").GetComponent<UILabel>().text = str;
		}
	}

	public void UpdateArmorDetailIncrementProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			m_armorDetailIncrementLevelStarsLabel.text = str;
			m_armorDetailIncrementLevelStarsLabel.color = UIUtil._UIGreenColor;
		}
		else if (str == string.Empty)
		{
			lsArmorPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = string.Empty;
		}
		else
		{
			lsArmorPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = "↑ " + str;
			lsArmorPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().color = UIUtil._UIGreenColor;
		}
	}

	public void SetArmorDetailUpgradePartVisable(bool bShow)
	{
		m_armorDetailUpgradePartGO.SetActive(bShow);
	}

	public void UpdateArmorDetailUpgradePartGoldPrice(string str)
	{
		m_armorDetailUpgradePartGoldLabel.text = str;
	}

	public void SetArmorDetailUnlockPartVisable(bool bShow)
	{
		m_armorDetailUnlockPartGO.SetActive(bShow);
	}

	public void UpdateArmorDetailUnlockPartGoldPrice(string str)
	{
		m_armorDetailUnlockPartGoldLabel.text = str;
	}

	public void UpdateArmorDetailUnlockPartItemPrice(string str, bool bCrystal)
	{
		m_armorDetailUnlockPartItemLabel.text = str;
		if (bCrystal)
		{
			m_armorDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
		}
		else
		{
			m_armorDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
		}
	}

	public void SetArmorDetailEquipPartVisable(bool bShow)
	{
		m_armorDetailEquipPartGO.SetActive(bShow);
	}

	public void SetArmorDetailEquipPartEnabel(bool bEnable)
	{
		m_armorDetailUnlockPartUnlockBtn.isEnabled = bEnable;
	}

	public void SetArmorDetailUpgradeBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_ArmorDetailUpgradeBtnClicked_Delegate dele)
	{
		ArmorDetailUpgradeBtnClickEvent = dele;
	}

	public void SetArmorDetailUnlockBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_ArmorDetailUnlockBtnClicked_Delegate dele)
	{
		ArmorDetailUnlockBtnClickEvent = dele;
	}

	public void SetArmorDetailEquipBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_ArmorDetailEquipBtnClicked_Delegate dele)
	{
		ArmorDetailEquipBtnClickEvent = dele;
	}

	private void HandleEvent_OrnamentsUpgradeBtnClicked()
	{
		if (OrnamentsDetailUpgradeBtnClickEvent != null)
		{
			OrnamentsDetailUpgradeBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_OrnamentsUnlockBtnClicked()
	{
		if (OrnamentsDetailUnlockBtnClickEvent != null)
		{
			OrnamentsDetailUnlockBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void HandleEvent_OrnamentsEquipBtnClicked()
	{
		if (OrnamentsDetailEquipBtnClickEvent != null)
		{
			OrnamentsDetailEquipBtnClickEvent();
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ShowOrnamentsEffect(Transform _trans, UIEffectManager.EffectType _eT)
	{
		if (_trans != null)
		{
			SetEffectVisable(_trans, _eT);
		}
		else
		{
			SetEffectVisable(m_ornamentsDetailIconTexture.transform, _eT);
		}
	}

	public void SetOrnamentsTreeLockedStateVisable(int index, bool bShow, int unlockLevel)
	{
		m_ornamentsArrTreeLinesLockedStateGO[index].SetActive(bShow);
		m_ornamentsArrTreeLinesLockedStateGO[index].transform.Find("Label").GetComponent<UILabel>().text = "UNLOCK AT TEAM LEVEL " + unlockLevel;
	}

	public void UpdateOrnamentsDetailName(string str)
	{
		m_ornamentsDetailNameLabel.text = str;
	}

	public void UpdateOrnamentsDetailIcon(Texture tex)
	{
		m_ornamentsDetailIconTexture.mainTexture = tex;
	}

	public void UpdateOrnamentsDetailCombat(string str)
	{
		m_ornamentsDetailCombatLabel.text = str;
	}

	public void RepositionOrnaments()
	{
		lsOrnamentsPropertys[0].transform.parent.GetComponent<UIGrid>().repositionNow = true;
	}

	public void SetAllOrnamentsPropertyVisable(bool bShow)
	{
		for (int i = 0; i < lsOrnamentsPropertys.Count; i++)
		{
			SetOrnamentsPropertyVisable((PropertyIndex)i, bShow);
		}
	}

	public void SetOrnamentsPropertyVisable(PropertyIndex pi, bool bShow)
	{
		lsOrnamentsPropertys[(int)pi].SetActive(bShow);
	}

	public void UpdateOrnamentsDetailProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			UIUtil.ShowStars(m_ornamentsDetailLevelStarsGO.transform, int.Parse(str));
		}
		else
		{
			lsOrnamentsPropertys[(int)pi].transform.Find("Value").GetComponent<UILabel>().text = str;
		}
	}

	public void UpdateOrnamentsDetailIncrementProperty(PropertyIndex pi, string str)
	{
		if (pi == PropertyIndex.E_level)
		{
			m_ornamentsDetailIncrementLevelStarsLabel.text = str;
			m_ornamentsDetailIncrementLevelStarsLabel.color = UIUtil._UIGreenColor;
		}
		else if (str == string.Empty)
		{
			lsOrnamentsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = string.Empty;
		}
		else
		{
			lsOrnamentsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().text = "↑ " + str;
			lsOrnamentsPropertys[(int)pi].transform.Find("Increment").GetComponent<UILabel>().color = UIUtil._UIGreenColor;
		}
	}

	public void SetOrnamentsDetailUpgradePartVisable(bool bShow)
	{
		m_ornamentsDetailUpgradePartGO.SetActive(bShow);
	}

	public void UpdateOrnamentsDetailUpgradePartGoldPrice(string str)
	{
		m_ornamentsDetailUpgradePartGoldLabel.text = str;
	}

	public void SetOrnamentsDetailUnlockPartVisable(bool bShow)
	{
		m_ornamentsDetailUnlockPartGO.SetActive(bShow);
	}

	public void UpdateOrnamentsDetailUnlockPartGoldPrice(string str)
	{
		m_ornamentsDetailUnlockPartGoldLabel.text = str;
	}

	public void UpdateOrnamentsDetailUnlockPartItemPrice(string str, bool bCrystal)
	{
		m_ornamentsDetailUnlockPartItemLabel.text = str;
		if (bCrystal)
		{
			m_ornamentsDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
		}
		else
		{
			m_ornamentsDetailUnlockPartItemImg.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Honor);
		}
	}

	public void SetOrnamentsDetailEquipPartVisable(bool bShow)
	{
		m_ornamentsDetailEquipPartGO.SetActive(bShow);
	}

	public void SetOrnamentsDetailEquipPartEnable(bool bEnable)
	{
		m_ornamentsDetailUnlockPartUnlockBtn.isEnabled = bEnable;
	}

	public void SetOrnamentsDetailUpgradeBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUpgradeBtnClicked_Delegate dele)
	{
		OrnamentsDetailUpgradeBtnClickEvent = dele;
	}

	public void SetOrnamentsDetailUnlockBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUnlockBtnClicked_Delegate dele)
	{
		OrnamentsDetailUnlockBtnClickEvent = dele;
	}

	public void SetOrnamentsDetailEquipBtnClickDelegate(UtilUITeamPlayerDetailDataInfo_OrnamentsDetailEquipBtnClicked_Delegate dele)
	{
		OrnamentsDetailEquipBtnClickEvent = dele;
	}
}
