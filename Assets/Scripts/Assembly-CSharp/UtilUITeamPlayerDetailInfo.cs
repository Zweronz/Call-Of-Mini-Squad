using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilUITeamPlayerDetailInfo : MonoBehaviour
{
	public enum EquipmentChangedType
	{
		E_WeaponUpgrade = 0,
		E_WeaponBreak = 1,
		E_SkillUpgrade = 2,
		E_SkillBreak = 3,
		E_EquipHeadUpgrade = 4,
		E_EquipBodyUpgrade = 5,
		E_EquipAccUpgrade = 6,
		E_EquipHeadUsed = 7,
		E_EquipBodyUsed = 8,
		E_EquipAccUsed = 9,
		E_EquipHeadUnlock = 10,
		E_EquipBodyUnlock = 11,
		E_EquipAccUnlock = 12
	}

	public class ITEMINFO
	{
		public UtilUITeamPlayerDetailItem ui;

		public EquipUpgradeData data;

		public GameObject go;

		public Defined.EQUIP_TYPE equipType;

		public ITEMINFO(GameObject go, EquipUpgradeData eud, Defined.EQUIP_TYPE et)
		{
			this.go = go;
			data = eud;
			equipType = et;
			ui = go.GetComponent<UtilUITeamPlayerDetailItem>();
		}
	}

	[SerializeField]
	private UtilUITeamPlayerDetailDataInfo m_scriptTPDetailDataInfo;

	[SerializeField]
	private UtilUITeamPlayerDetailExDataInfo m_scriptTPDetailExDataInfo;

	[SerializeField]
	private UtilUITeamExhibitionInfo m_scriptUIExhibition;

	public PlayerData playdata;

	private DataConf.HeroData heroConf;

	private DataConf.WeaponData weaponConf;

	private DataConf.HeroSkillInfo skillConf;

	private UtilUITeamPlayerDetailInfo_OnEquipmentChanged_Delegate _equipmentChangedEvent;

	private UtilUITeamPlayerDetailInfo_HandleExTitleStateChanged_Delegate _exTitleStateChangedEvent;

	public List<GameObject> lsHelmsGOS;

	protected int lastEqiupedHelmsItemIndex = -1;

	protected int nowSelectHelmsItemIndex;

	public List<GameObject> lsArmorsGOS;

	protected int lastEquipedArmorsItemIndex = -1;

	protected int nowSelectArmorsItemIndex;

	public List<GameObject> lsOmamentsGOS;

	protected int lastEquipedOmamentsItemIndex = -1;

	protected int nowSelectOmamentsItemIndex;

	public GameObject stateChangeGO;

	private Dictionary<int, ITEMINFO> dictEquipItemsInfo = new Dictionary<int, ITEMINFO>();

	private Dictionary<GameObject, int> dictMapEquipItemsInfo = new Dictionary<GameObject, int>();

	public UtilUITeamPlayerDetailDataInfo TPDETAILDATAINFOSCRIPT
	{
		get
		{
			return m_scriptTPDetailDataInfo;
		}
	}

	public UtilUITeamPlayerDetailExDataInfo TPDETAILEXDATAINFOSCRIPT
	{
		get
		{
			return m_scriptTPDetailExDataInfo;
		}
	}

	public UtilUITeamExhibitionInfo UITEAMEXHIBITIONSCRIPT
	{
		get
		{
			return m_scriptUIExhibition;
		}
	}

	public void Init(UtilUITeamPlayerDetailInfo_OnEquipmentChanged_Delegate equipmentUsedEvent, UtilUITeamPlayerDetailDataInfo_PageStateBtnClick_Delegate detailPageStateBtnClick, UtilUITeamPlayerDetailInfo_HandleExTitleStateChanged_Delegate exTitleChangedStateEvent)
	{
		BindingFunction(equipmentUsedEvent, detailPageStateBtnClick, HandleExTitleStateChangeEVent, HandleWSWeaponUpgradeBtnClickEvent, HandleWSWeaponBreakBtnClickEvent, HandleWSWeaponBreakFailBtnClickEvent, HandleWSSkillUpgradeBtnClickEvent, HandleWSSkillBreakBtnClickEvent, HandleWSSkillBreakFailBtnClickEvent, HandleEquipItemDetailUpgradeBtnClickEvent, HandleEquipItemDetailUnlockBtnClickEvent, HandleEquipItemDetailEquipBtnClickEvent, HandleEquipItemStateChangedEvent, HandleEquipItemDetailUpgradeBtnClickEvent, HandleEquipItemDetailUnlockBtnClickEvent, HandleEquipItemDetailEquipBtnClickEvent, HandleEquipItemStateChangedEvent, HandleEquipItemDetailUpgradeBtnClickEvent, HandleEquipItemDetailUnlockBtnClickEvent, HandleEquipItemDetailEquipBtnClickEvent, HandleEquipItemStateChangedEvent);
		_exTitleStateChangedEvent = exTitleChangedStateEvent;
	}

	public void SetData(int id)
	{
		playdata = DataCenter.Save().GetPlayerData(id);
		heroConf = DataCenter.Conf().GetHeroDataByIndex(playdata.heroIndex);
		weaponConf = DataCenter.Conf().GetWeaponDataByType(heroConf.weaponType);
		skillConf = DataCenter.Conf().GetHeroSkillInfo(heroConf.characterType, playdata.skillLevel, playdata.skillStar);
	}

	public void UpdatExDetailUIInfo()
	{
		try
		{
			TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[0].gameObject.SetActive(true);
			TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[1].gameObject.SetActive(true);
			TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[2].gameObject.SetActive(true);
			TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[3].gameObject.SetActive(true);
			UpdateWSWeaponUIInfo();
			UpdateWSWeaponCombat();
			UpdateWSWeaponStars();
			UpdateWSWeaponLevel();
			UpdateWSWeaponDamage();
			UpdateWSWeaponAmmo();
			UpdateWSWeaponRPM();
			UpdateWPricePart();
			UpdateWSSkillUIInfo();
			UpdateWSSkillStars();
			UpdateWSSkillLevel();
			UpdateWSSkillIntro();
			UpdateSPricePart();
			dictEquipItemsInfo.Clear();
			dictMapEquipItemsInfo.Clear();
			ITEMINFO ii = null;
			for (int i = 0; i < playdata.upgradeData.helmsUpgrade.Length; i++)
			{
				EquipUpgradeData equipUpgradeData = playdata.upgradeData.helmsUpgrade[i];
				ITEMINFO iTEMINFO = new ITEMINFO(lsHelmsGOS[equipUpgradeData.index - 1], equipUpgradeData, Defined.EQUIP_TYPE.Head);
				UpdateEquipItemUI(iTEMINFO);
				UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, iTEMINFO);
				dictEquipItemsInfo.Add(equipUpgradeData.equipIndex, iTEMINFO);
				dictMapEquipItemsInfo.Add(iTEMINFO.go, equipUpgradeData.equipIndex);
				if (iTEMINFO.data.equipIndex == playdata.equips[Defined.EQUIP_TYPE.Head].currEquipIndex)
				{
					nowSelectHelmsItemIndex = equipUpgradeData.equipIndex;
					iTEMINFO.ui.Selected(true);
					ii = iTEMINFO;
				}
			}
			UpdateExDetailUI(ii);
			for (int j = 0; j < playdata.upgradeData.ArmorsUpgrade.Length; j++)
			{
				EquipUpgradeData equipUpgradeData2 = playdata.upgradeData.ArmorsUpgrade[j];
				ITEMINFO iTEMINFO2 = new ITEMINFO(lsArmorsGOS[equipUpgradeData2.index - 1], equipUpgradeData2, Defined.EQUIP_TYPE.Body);
				UpdateEquipItemUI(iTEMINFO2);
				UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, iTEMINFO2);
				dictEquipItemsInfo.Add(equipUpgradeData2.equipIndex, iTEMINFO2);
				dictMapEquipItemsInfo.Add(iTEMINFO2.go, equipUpgradeData2.equipIndex);
				if (iTEMINFO2.data.equipIndex == playdata.equips[Defined.EQUIP_TYPE.Body].currEquipIndex)
				{
					nowSelectArmorsItemIndex = equipUpgradeData2.equipIndex;
					iTEMINFO2.ui.Selected(true);
					ii = iTEMINFO2;
				}
			}
			UpdateExDetailUI(ii);
			for (int k = 0; k < playdata.upgradeData.ornamentsUpgrade.Length; k++)
			{
				EquipUpgradeData equipUpgradeData3 = playdata.upgradeData.ornamentsUpgrade[k];
				ITEMINFO iTEMINFO3 = new ITEMINFO(lsOmamentsGOS[equipUpgradeData3.index - 1], equipUpgradeData3, Defined.EQUIP_TYPE.Acc);
				UpdateEquipItemUI(iTEMINFO3);
				UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, iTEMINFO3);
				dictEquipItemsInfo.Add(equipUpgradeData3.equipIndex, iTEMINFO3);
				dictMapEquipItemsInfo.Add(iTEMINFO3.go, equipUpgradeData3.equipIndex);
				if (iTEMINFO3.data.equipIndex == playdata.equips[Defined.EQUIP_TYPE.Acc].currEquipIndex)
				{
					nowSelectOmamentsItemIndex = equipUpgradeData3.equipIndex;
					iTEMINFO3.ui.Selected(true);
					ii = iTEMINFO3;
				}
			}
			UpdateExDetailUI(ii);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void UpdateDetailUIInfo(string name, string iconName, string health, string damage, string defense, string crit, string speed, string hit, string dodge, string combat, string intro, string heroType, Texture wTex, Texture sTex)
	{
		TPDETAILDATAINFOSCRIPT.UpdateName(name);
		TPDETAILDATAINFOSCRIPT.UpdatePlayerIcon(iconName);
		TPDETAILDATAINFOSCRIPT.UpdateHealth(health);
		TPDETAILDATAINFOSCRIPT.UpdateDamage(damage);
		TPDETAILDATAINFOSCRIPT.UpdateArmor(defense);
		TPDETAILDATAINFOSCRIPT.UpdateCrit(crit);
		TPDETAILDATAINFOSCRIPT.UpdateSpeed(speed);
		TPDETAILDATAINFOSCRIPT.UpdateHit(hit);
		TPDETAILDATAINFOSCRIPT.UpdateDodge(dodge);
		TPDETAILDATAINFOSCRIPT.UpdateCombat(combat);
		TPDETAILDATAINFOSCRIPT.UpdatTipsInfo(iconName, name, intro, wTex, sTex);
		UITEAMEXHIBITIONSCRIPT.UpdateHeroInfo(iconName, name, intro, heroType, wTex, sTex);
	}

	public void SetExSelectTitle(int index)
	{
		for (int i = 0; i < TPDETAILEXDATAINFOSCRIPT.m_conentsPanel.Length; i++)
		{
			TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[i].gameObject.SetActive(false);
		}
		TPDETAILEXDATAINFOSCRIPT.SetTitleChecked(index);
		TPDETAILEXDATAINFOSCRIPT.m_conentsPanel[index].gameObject.SetActive(true);
	}

	protected void BindingFunction(UtilUITeamPlayerDetailInfo_OnEquipmentChanged_Delegate equipmentUsedEvent, UtilUITeamPlayerDetailDataInfo_PageStateBtnClick_Delegate detailPageStateBtnClick, UtilUITeamPlayerDetailDataInfo_TitleToggleStateChange_Delegate exTitleStateChange, UtilUITeamPlayerDetailExDataInfo_WSWeaponUpgradeBtnClicked_Delegate exWSWeaponUpgradeBtnClick, UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate exWSWeaponBreakBtnClick, UtilUITeamPlayerDetailDataInfo_WSWeaponBreakBtnClicked_Delegate exWSWeaponBreakFailBtnClick, UtilUITeamPlayerDetailExDataInfo_WSSkillUpgradeBtnClicked_Delegate exWSSkillUpgradeBtnClick, UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate exWSSkillBreakBtnClick, UtilUITeamPlayerDetailDataInfo_WSSkillBreakBtnClicked_Delegate exWSSkillBreakFailBtnClick, UtilUITeamPlayerDetailDataInfo_HelmsDetailUpgradeBtnClicked_Delegate exHelmsUpgradeBtnClick, UtilUITeamPlayerDetailDataInfo_HelmsDetailUnlockBtnClicked_Delegate exHelmsUnlockBtnClick, UtilUITeamPlayerDetailDataInfo_HelmsDetailEquipBtnClicked_Delegate exHelmsEquipBtnClick, UtilUITeamPlayerDetailItem_ItemToggleStateChange_Delegate exHelmsItemStateChangedEvent, UtilUITeamPlayerDetailDataInfo_ArmorDetailUpgradeBtnClicked_Delegate exArmorUpgradeBtnClick, UtilUITeamPlayerDetailDataInfo_ArmorDetailUnlockBtnClicked_Delegate exArmorUnlockBtnClick, UtilUITeamPlayerDetailDataInfo_ArmorDetailEquipBtnClicked_Delegate exArmorEquipBtnClick, UtilUITeamPlayerDetailItem_ItemToggleStateChange_Delegate exArmorItemStateChangedEvent, UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUpgradeBtnClicked_Delegate exOrnamentsUpgradeBtnClick, UtilUITeamPlayerDetailDataInfo_OrnamentsDetailUnlockBtnClicked_Delegate exOrnamentsUnlockBtnClick, UtilUITeamPlayerDetailDataInfo_OrnamentsDetailEquipBtnClicked_Delegate exOrnamentsEquipBtnClick, UtilUITeamPlayerDetailItem_ItemToggleStateChange_Delegate exOrnamentsItemStateChangedEvent)
	{
		_equipmentChangedEvent = equipmentUsedEvent;
		TPDETAILDATAINFOSCRIPT.SetPageStateBtnClickDelegate(detailPageStateBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetTitleToggleStateChangeDelegate(exTitleStateChange);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponUpgradeBtnClickDelegate(exWSWeaponUpgradeBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnClickDelegate(exWSWeaponBreakBtnClick, exWSWeaponBreakFailBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillUpgradeBtnClickDelegate(exWSSkillUpgradeBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetWSSKillBreakBtnClickDelegate(exWSSkillBreakBtnClick, exWSSkillBreakFailBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUpgradeBtnClickDelegate(exHelmsUpgradeBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUnlockBtnClickDelegate(exHelmsUnlockBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailEquipBtnClickDelegate(exHelmsEquipBtnClick);
		for (int i = 0; i < lsHelmsGOS.Count; i++)
		{
			lsHelmsGOS[i].GetComponent<UtilUITeamPlayerDetailItem>().SetItemToggleStateChangeDelegate(exHelmsItemStateChangedEvent);
		}
		TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUpgradeBtnClickDelegate(exArmorUpgradeBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUnlockBtnClickDelegate(exArmorUnlockBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetArmorDetailEquipBtnClickDelegate(exArmorEquipBtnClick);
		for (int j = 0; j < lsArmorsGOS.Count; j++)
		{
			lsArmorsGOS[j].GetComponent<UtilUITeamPlayerDetailItem>().SetItemToggleStateChangeDelegate(exArmorItemStateChangedEvent);
		}
		TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUpgradeBtnClickDelegate(exOrnamentsUpgradeBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUnlockBtnClickDelegate(exOrnamentsUnlockBtnClick);
		TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailEquipBtnClickDelegate(exOrnamentsEquipBtnClick);
		for (int k = 0; k < lsOmamentsGOS.Count; k++)
		{
			lsOmamentsGOS[k].GetComponent<UtilUITeamPlayerDetailItem>().SetItemToggleStateChangeDelegate(exOrnamentsItemStateChangedEvent);
		}
	}

	public void OnLevelupWeaponFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 17);
		UIDialogManager.Instance.HideBlock(17);
		Debug.LogWarning("######### " + code);
		if (code != 0)
		{
			string strInput = "update " + weaponConf.name + "?";
			int num = Mathf.CeilToInt((float)(UIConstant.TradeMoneyNotEnough + UIConstant.TradeHornorNotEnough) / (float)UIConstant.MoneyExchangRate);
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code, strInput, RequestLevelupWeaponUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			return;
		}
		UpdateWSWeaponUIInfo();
		UpdateWSWeaponCombat();
		UpdateWSWeaponStars();
		UpdateWSWeaponLevel();
		UpdateWSWeaponDamage();
		UpdateWSWeaponAmmo();
		UpdateWSWeaponRPM();
		UpdateWPricePart();
		UISoundManager.Instance.PlayLevelUpSound();
		TPDETAILEXDATAINFOSCRIPT.ShowWeaponEffect(UIEffectManager.EffectType.E_Team_LevelUp);
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(EquipmentChangedType.E_WeaponUpgrade);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnLevelupSkillFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 18);
		UIDialogManager.Instance.HideBlock(18);
		if (code != 0)
		{
			string strInput = "update " + skillConf.name + "?";
			int num = Mathf.CeilToInt((float)(UIConstant.TradeMoneyNotEnough + UIConstant.TradeHornorNotEnough) / (float)UIConstant.MoneyExchangRate);
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code, strInput, RequestLevelupSkillUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			return;
		}
		UpdateWSSkillUIInfo();
		UpdateWSSkillStars();
		UpdateWSSkillLevel();
		UpdateWSSkillIntro();
		UpdateSPricePart();
		UISoundManager.Instance.PlayLevelUpSound();
		TPDETAILEXDATAINFOSCRIPT.ShowSkillEffect(UIEffectManager.EffectType.E_Team_LevelUp);
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(EquipmentChangedType.E_SkillUpgrade);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnEquipmentLevelupFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 20);
		UIDialogManager.Instance.HideBlock(20);
		int key = 0;
		if (code != 0)
		{
			if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
			{
				key = nowSelectHelmsItemIndex;
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
			{
				key = nowSelectArmorsItemIndex;
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
			{
				key = nowSelectOmamentsItemIndex;
			}
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(dictEquipItemsInfo[key].data.equipIndex);
			string strInput = "update " + equipDataByIndex.name + "?";
			int num = Mathf.CeilToInt((float)(UIConstant.TradeMoneyNotEnough + UIConstant.TradeHornorNotEnough) / (float)UIConstant.MoneyExchangRate);
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code, strInput, RequestLevelupEquipmentUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			return;
		}
		EquipmentChangedType key2 = EquipmentChangedType.E_EquipHeadUpgrade;
		if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
		{
			key = nowSelectHelmsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowHelmsEffect(null, UIEffectManager.EffectType.E_Team_LevelUp);
			key2 = EquipmentChangedType.E_EquipHeadUpgrade;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
		{
			key = nowSelectArmorsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowArmorEffect(null, UIEffectManager.EffectType.E_Team_LevelUp);
			key2 = EquipmentChangedType.E_EquipBodyUpgrade;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
		{
			key = nowSelectOmamentsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowOrnamentsEffect(null, UIEffectManager.EffectType.E_Team_LevelUp);
			key2 = EquipmentChangedType.E_EquipAccUpgrade;
		}
		UpdateEquipItemUI(dictEquipItemsInfo[key]);
		UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, dictEquipItemsInfo[key]);
		UpdateExDetailUI(dictEquipItemsInfo[key]);
		for (int i = 0; i < DataCenter.State().lsUpdatedEquipmentEquipIndexs.Count; i++)
		{
			int key3 = DataCenter.State().lsUpdatedEquipmentEquipIndexs[i];
			UpdateEquipItemUI(dictEquipItemsInfo[key3]);
		}
		UISoundManager.Instance.PlayLevelUpSound();
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(key2);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnEquipmentUnlockedFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 21);
		UIDialogManager.Instance.HideBlock(21);
		int key = 0;
		if (code != 0)
		{
			if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
			{
				key = nowSelectHelmsItemIndex;
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
			{
				key = nowSelectArmorsItemIndex;
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
			{
				key = nowSelectOmamentsItemIndex;
			}
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(dictEquipItemsInfo[key].data.equipIndex);
			string strInput = "unlock " + equipDataByIndex.name + "?";
			int num = Mathf.CeilToInt((float)(UIConstant.TradeMoneyNotEnough + UIConstant.TradeHornorNotEnough) / (float)UIConstant.MoneyExchangRate);
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code, strInput, RequesetUnlockEquipmentUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			return;
		}
		EquipmentChangedType key2 = EquipmentChangedType.E_EquipHeadUnlock;
		if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
		{
			key = nowSelectHelmsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowHelmsEffect(dictEquipItemsInfo[key].ui.GetIcon().transform, UIEffectManager.EffectType.E_Team_Unlock);
			key2 = EquipmentChangedType.E_EquipHeadUnlock;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
		{
			key = nowSelectArmorsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowArmorEffect(dictEquipItemsInfo[key].ui.GetIcon().transform, UIEffectManager.EffectType.E_Team_Unlock);
			key2 = EquipmentChangedType.E_EquipBodyUnlock;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
		{
			key = nowSelectOmamentsItemIndex;
			TPDETAILEXDATAINFOSCRIPT.ShowOrnamentsEffect(dictEquipItemsInfo[key].ui.GetIcon().transform, UIEffectManager.EffectType.E_Team_Unlock);
			key2 = EquipmentChangedType.E_EquipAccUnlock;
		}
		UpdateEquipItemUI(dictEquipItemsInfo[key]);
		UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, dictEquipItemsInfo[key]);
		UpdateExDetailUI(dictEquipItemsInfo[key]);
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(key2);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnEquipmentBreakFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 19);
		UIDialogManager.Instance.HideBlock(19);
		if (code != 0)
		{
			string empty = string.Empty;
			int num = Mathf.CeilToInt((float)(UIConstant.TradeMoneyNotEnough + UIConstant.TradeHornorNotEnough) / (float)UIConstant.MoneyExchangRate);
			if (DataCenter.State().selectEquipBreakType == Defined.TeamEquipmentBreakType.Weapon)
			{
				empty = "break " + weaponConf.name + "?";
				UIDialogManager.Instance.ShowHttpFeedBackMsg(code, empty, RequestBreakEquipmentWeaponUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			}
			else if (DataCenter.State().selectEquipBreakType == Defined.TeamEquipmentBreakType.Skill)
			{
				empty = "break " + skillConf.name + "?";
				UIDialogManager.Instance.ShowHttpFeedBackMsg(code, empty, RequestBreakEquipmentSkillUseCrystal, Defined.COST_TYPE.Money, UIConstant.TradeMoneyNotEnough, Defined.COST_TYPE.Honor, UIConstant.TradeHornorNotEnough, " " + num);
			}
			return;
		}
		EquipmentChangedType key = EquipmentChangedType.E_WeaponBreak;
		if (DataCenter.State().selectEquipBreakType == Defined.TeamEquipmentBreakType.Weapon)
		{
			UpdateWSWeaponUIInfo();
			UpdateWSWeaponCombat();
			UpdateWSWeaponStars();
			UpdateWSWeaponLevel();
			UpdateWSWeaponDamage();
			UpdateWSWeaponAmmo();
			UpdateWSWeaponRPM();
			UpdateWPricePart();
			TPDETAILEXDATAINFOSCRIPT.ShowWeaponEffect(UIEffectManager.EffectType.E_Team_Break);
			key = EquipmentChangedType.E_WeaponBreak;
		}
		else if (DataCenter.State().selectEquipBreakType == Defined.TeamEquipmentBreakType.Skill)
		{
			UpdateWSSkillUIInfo();
			UpdateWSSkillStars();
			UpdateWSSkillLevel();
			UpdateWSSkillIntro();
			UpdateSPricePart();
			TPDETAILEXDATAINFOSCRIPT.ShowSkillEffect(UIEffectManager.EffectType.E_Team_Break);
			key = EquipmentChangedType.E_SkillBreak;
		}
		UISoundManager.Instance.PlayBreakSound();
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(key);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void OnEquipmentUsedFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 22);
		UIDialogManager.Instance.HideBlock(22);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
			return;
		}
		int key = 0;
		int key2 = 0;
		EquipmentChangedType key3 = EquipmentChangedType.E_EquipHeadUsed;
		if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
		{
			key = nowSelectHelmsItemIndex;
			key2 = lastEqiupedHelmsItemIndex;
			key3 = EquipmentChangedType.E_EquipHeadUsed;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
		{
			key = nowSelectArmorsItemIndex;
			key2 = lastEquipedArmorsItemIndex;
			key3 = EquipmentChangedType.E_EquipBodyUsed;
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
		{
			key = nowSelectOmamentsItemIndex;
			key2 = lastEquipedOmamentsItemIndex;
			key3 = EquipmentChangedType.E_EquipAccUsed;
		}
		UpdateEquipItemUI(dictEquipItemsInfo[key2]);
		UpdateEquipItemUI(dictEquipItemsInfo[key]);
		UpdateTreeItemLockedStateTagVisable(DataCenter.Save().GetTeamData().teamLevel, dictEquipItemsInfo[key]);
		UpdateExDetailUI(dictEquipItemsInfo[key]);
		if (_equipmentChangedEvent != null)
		{
			_equipmentChangedEvent(key3);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	protected void HandleExTitleStateChangeEVent(int index, bool bCheck)
	{
		if (bCheck)
		{
			switch (index)
			{
			case 1:
				DataCenter.State().selectEquipType = Defined.EQUIP_TYPE.Head;
				break;
			case 2:
				DataCenter.State().selectEquipType = Defined.EQUIP_TYPE.Body;
				break;
			case 3:
				DataCenter.State().selectEquipType = Defined.EQUIP_TYPE.Acc;
				break;
			}
			if (_exTitleStateChangedEvent != null)
			{
				_exTitleStateChangedEvent(DataCenter.State().selectEquipType);
			}
			else
			{
				UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
			}
		}
	}

	protected void RequestLevelupWeaponUseCrystal()
	{
		RequestLevelupWeapon(1);
	}

	protected void RequestLevelupWeapon(int useCystal)
	{
		DataCenter.State().protocoUseCrystal = useCystal;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 17);
		UIDialogManager.Instance.ShowBlock(17);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_LevelUpWeapon, OnLevelupWeaponFinished);
	}

	protected void RequestLevelupSkillUseCrystal()
	{
		RequestLevelupSkill(1);
	}

	protected void RequestLevelupSkill(int useCystal)
	{
		DataCenter.State().protocoUseCrystal = useCystal;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 18);
		UIDialogManager.Instance.ShowBlock(18);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_LevelUpSkill, OnLevelupSkillFinished);
	}

	protected void RequestLevelupEquipmentUseCrystal()
	{
		RequestLevelupEquipment(1);
	}

	protected void RequestLevelupEquipment(int useCystal)
	{
		ITEMINFO iTEMINFO = null;
		if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectHelmsItemIndex];
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectArmorsItemIndex];
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectOmamentsItemIndex];
		}
		if (iTEMINFO != null)
		{
			DataCenter.State().selectEquipIndex = iTEMINFO.data.index;
		}
		DataCenter.State().protocoUseCrystal = useCystal;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 20);
		UIDialogManager.Instance.ShowBlock(20);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_LevelUpEquipment, OnEquipmentLevelupFinished);
	}

	protected void RequestBreakEquipmentWeaponUseCrystal()
	{
		RequestBreakEquipment(1, Defined.TeamEquipmentBreakType.Weapon);
	}

	protected void RequestBreakEquipmentSkillUseCrystal()
	{
		RequestBreakEquipment(1, Defined.TeamEquipmentBreakType.Skill);
	}

	protected void RequestBreakEquipment(int useCystal, Defined.TeamEquipmentBreakType bt)
	{
		DataCenter.State().protocoUseCrystal = useCystal;
		DataCenter.State().selectEquipBreakType = bt;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 19);
		UIDialogManager.Instance.ShowBlock(19);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_BreakEquipment, OnEquipmentBreakFinished);
	}

	protected void RequesetUnlockEquipmentUseCrystal()
	{
		RequesetUnlockEquipment(1);
	}

	protected void RequesetUnlockEquipment(int useCystal)
	{
		ITEMINFO iTEMINFO = null;
		if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectHelmsItemIndex];
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectArmorsItemIndex];
		}
		else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
		{
			iTEMINFO = dictEquipItemsInfo[nowSelectOmamentsItemIndex];
		}
		if (iTEMINFO != null)
		{
			DataCenter.State().selectEquipIndex = iTEMINFO.data.index;
		}
		DataCenter.State().protocoUseCrystal = useCystal;
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 21);
		UIDialogManager.Instance.ShowBlock(21);
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_UnlockEquipment, OnEquipmentUnlockedFinished);
	}

	protected void HandleWSWeaponUpgradeBtnClickEvent()
	{
		if (true)
		{
			RequestLevelupWeapon(0);
		}
	}

	protected void HandleWSWeaponBreakBtnClickEvent()
	{
		if (true)
		{
			RequestBreakEquipment(0, Defined.TeamEquipmentBreakType.Weapon);
		}
	}

	protected void HandleWSWeaponBreakFailBtnClickEvent()
	{
		UIDialogManager.Instance.ShowDriftMsgInfoUI("Break Weapon at Team Level " + playdata.upgradeData.weaponBkTeamLevel);
	}

	protected void HandleWSSkillUpgradeBtnClickEvent()
	{
		if (true)
		{
			RequestLevelupSkill(0);
		}
	}

	protected void HandleWSSkillBreakBtnClickEvent()
	{
		if (true)
		{
			RequestBreakEquipment(0, Defined.TeamEquipmentBreakType.Skill);
		}
	}

	protected void HandleWSSkillBreakFailBtnClickEvent()
	{
		UIDialogManager.Instance.ShowDriftMsgInfoUI("Break Skill at Team Level " + playdata.upgradeData.skillBkTeamLevel);
	}

	protected void HandleEquipItemDetailUpgradeBtnClickEvent()
	{
		if (true)
		{
			RequestLevelupEquipment(0);
		}
	}

	protected void HandleEquipItemDetailUnlockBtnClickEvent()
	{
		if (true)
		{
			RequesetUnlockEquipment(0);
		}
	}

	protected void HandleEquipItemDetailEquipBtnClickEvent()
	{
		if (true)
		{
			ITEMINFO iTEMINFO = null;
			if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Head)
			{
				iTEMINFO = dictEquipItemsInfo[nowSelectHelmsItemIndex];
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Body)
			{
				iTEMINFO = dictEquipItemsInfo[nowSelectArmorsItemIndex];
			}
			else if (DataCenter.State().selectEquipType == Defined.EQUIP_TYPE.Acc)
			{
				iTEMINFO = dictEquipItemsInfo[nowSelectOmamentsItemIndex];
			}
			if (iTEMINFO != null)
			{
				DataCenter.State().selectEquipIndex = iTEMINFO.data.index;
			}
			UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 22);
			UIDialogManager.Instance.ShowBlock(22);
			HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Team_UseEquipment, OnEquipmentUsedFinished);
		}
	}

	protected void HandleEquipItemStateChangedEvent(GameObject go, bool bChecked)
	{
		if (bChecked)
		{
			ITEMINFO iTEMINFO = dictEquipItemsInfo[dictMapEquipItemsInfo[go]];
			if (iTEMINFO.equipType == Defined.EQUIP_TYPE.Head)
			{
				nowSelectHelmsItemIndex = dictMapEquipItemsInfo[go];
			}
			else if (iTEMINFO.equipType == Defined.EQUIP_TYPE.Body)
			{
				nowSelectArmorsItemIndex = dictMapEquipItemsInfo[go];
			}
			else if (iTEMINFO.equipType == Defined.EQUIP_TYPE.Acc)
			{
				nowSelectOmamentsItemIndex = dictMapEquipItemsInfo[go];
			}
			UpdateExDetailUI(iTEMINFO);
		}
	}

	protected void UpdateWSWeaponUIInfo()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponName(weaponConf.name);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIcon(UIUtil.GetEquipTextureMaterial(weaponConf.iconFileName).mainTexture);
	}

	protected void UpdateWSWeaponCombat()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponCombat(playdata.upgradeData.weaponCombat + string.Empty);
	}

	protected void UpdateWSWeaponStars()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponStars(playdata.weaponStar);
	}

	protected void UpdateWSWeaponLevel()
	{
		int num = playdata.weaponLevel - 1;
		int num2 = playdata.weaponMaxLevel - 1;
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponLevel(num + "/" + num2);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIncrementLevel(string.Empty);
	}

	protected void UpdateWSWeaponDamage()
	{
		float damage = weaponConf.GetDamage(playdata.weaponLevel, playdata.weaponStar, playdata.weaponMaxLevel);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponDamage(damage + string.Empty);
		if (playdata.weaponLevel < playdata.weaponMaxLevel)
		{
			float damage2 = weaponConf.GetDamage(playdata.weaponLevel, playdata.weaponStar, playdata.weaponMaxLevel, true);
			float num = damage2 - damage;
			TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIncrementDamage("↑" + num + string.Empty);
		}
		else
		{
			TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIncrementDamage(string.Empty);
		}
	}

	protected void UpdateWSWeaponAmmo()
	{
		int num = weaponConf.Ammo(playdata.weaponLevel);
		if (num > 0)
		{
			TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponAmmo(num + string.Empty, 16);
		}
		else
		{
			TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponAmmo("∞", 32);
		}
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIncrementAmmo(string.Empty);
	}

	protected void UpdateWSWeaponRPM()
	{
		float num = weaponConf.FireFrequence(playdata.weaponLevel);
		float f = 60f / num;
		int num2 = Mathf.RoundToInt(f);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponRPM(num2 + string.Empty);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponIncrementRPM(string.Empty);
	}

	protected void UpdateWPricePart()
	{
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponGoldVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponItemVisable(false, true);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponUpgradeBtnVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakFailBtnVisable(false);
		if (playdata.upgradeData.weaponUpgradeCost > 0)
		{
			TPDETAILEXDATAINFOSCRIPT.SetWSWeaponGoldVisable(true);
			TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponGoldPrice(string.Empty + playdata.upgradeData.weaponUpgradeCost);
			TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnVisable(false);
			if (playdata.weaponLevel < playdata.weaponMaxLevel)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponUpgradeBtnVisable(true);
			}
		}
		if (DataCenter.Save().GetTeamData().teamLevel >= playdata.upgradeData.weaponBkTeamLevel)
		{
			if (playdata.upgradeData.weaponUBreakCostHonor > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponItemVisable(true, false);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponItemPrice(playdata.upgradeData.weaponUBreakCostHonor + string.Empty);
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnVisable(true);
			}
			if (playdata.upgradeData.weaponBreakCostMoney > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponGoldVisable(true);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponGoldPrice(string.Empty + playdata.upgradeData.weaponBreakCostMoney);
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnVisable(true);
			}
			if (playdata.upgradeData.weaponUBreakCostCrystal > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponItemVisable(true, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSWeaponItemPrice(playdata.upgradeData.weaponUBreakCostCrystal + string.Empty);
				TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakBtnVisable(true);
			}
		}
		else
		{
			TPDETAILEXDATAINFOSCRIPT.SetWSWeaponBreakFailBtnVisable(true);
		}
	}

	protected void UpdateWSSkillUIInfo()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillName(skillConf.name);
		TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillIcon(UIUtil.GetEquipTextureMaterial(skillConf.fileName).mainTexture);
	}

	protected void UpdateWSSkillStars()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillStars(playdata.skillStar);
	}

	protected void UpdateWSSkillLevel()
	{
		TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillLevel(playdata.skillLevel - 1 + "/" + (playdata.skillMaxLevel - 1));
	}

	protected void UpdateWSSkillIntro()
	{
		string heroSkillInfo = UIUtil.GetHeroSkillInfo(heroConf.characterType, playdata.skillLevel, playdata.skillStar, skillConf.description);
		heroSkillInfo = heroSkillInfo.Replace("\\n", "\n");
		TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillIntroduce(heroSkillInfo);
	}

	protected void UpdateSPricePart()
	{
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillGoldVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillItemVisable(false, true);
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillUpgradeBtnVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakBtnVisable(false);
		TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakFailBtnVisable(false);
		if (playdata.upgradeData.skillUpgradeCost > 0)
		{
			TPDETAILEXDATAINFOSCRIPT.SetWSSkillGoldVisable(true);
			TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillGoldPrice(string.Empty + playdata.upgradeData.skillUpgradeCost);
			if (playdata.skillLevel < playdata.skillMaxLevel)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillUpgradeBtnVisable(true);
			}
		}
		if (DataCenter.Save().GetTeamData().teamLevel >= playdata.upgradeData.skillBkTeamLevel)
		{
			if (playdata.upgradeData.skillBreakCostHonor > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillItemVisable(true, false);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillItemPrice(playdata.upgradeData.skillBreakCostHonor + string.Empty);
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakBtnVisable(true);
			}
			if (playdata.upgradeData.skillBreakCostMoney > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillGoldVisable(true);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillGoldPrice(string.Empty + playdata.upgradeData.skillBreakCostMoney);
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakBtnVisable(true);
			}
			if (playdata.upgradeData.skillBreakCostCrystal > 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillItemVisable(true, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateWSSkillItemPrice(string.Empty + playdata.upgradeData.skillBreakCostCrystal);
				TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakBtnVisable(true);
			}
		}
		else
		{
			TPDETAILEXDATAINFOSCRIPT.SetWSSkillBreakFailBtnVisable(true);
		}
	}

	protected void UpdateEquipItemUI(ITEMINFO ii)
	{
		DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(ii.data.equipIndex);
		ii.ui.UpdateTreeItemState(ii.data.state);
		ii.ui.SetEquipTagVisable(playdata.equips[ii.equipType].currEquipIndex == ii.data.equipIndex);
		if (playdata.equips[ii.equipType].currEquipIndex == ii.data.equipIndex)
		{
			if (ii.equipType == Defined.EQUIP_TYPE.Head)
			{
				lastEqiupedHelmsItemIndex = ii.data.equipIndex;
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Body)
			{
				lastEquipedArmorsItemIndex = ii.data.equipIndex;
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Acc)
			{
				lastEquipedOmamentsItemIndex = ii.data.equipIndex;
			}
		}
		ii.ui.UpdateIcon(UIUtil.GetEquipTextureMaterial(equipDataByIndex.fileName).mainTexture);
		ii.ui.UpdateStars(ii.data.level, ii.data.maxLevel);
	}

	protected void UpdateTreeItemLockedStateTagVisable(int teamLevel, ITEMINFO ii)
	{
		if (ii.data.index == 2)
		{
			if (ii.equipType == Defined.EQUIP_TYPE.Head)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsTreeLockedStateVisable(0, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Body)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorTreeLockedStateVisable(0, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Acc)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsTreeLockedStateVisable(0, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
		}
		else if (ii.data.index == 4)
		{
			if (ii.equipType == Defined.EQUIP_TYPE.Head)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsTreeLockedStateVisable(1, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Body)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorTreeLockedStateVisable(1, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Acc)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsTreeLockedStateVisable(1, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
		}
		else if (ii.data.index == 8)
		{
			if (ii.equipType == Defined.EQUIP_TYPE.Head)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsTreeLockedStateVisable(2, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Body)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorTreeLockedStateVisable(2, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
			else if (ii.equipType == Defined.EQUIP_TYPE.Acc)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsTreeLockedStateVisable(2, teamLevel < ii.data.unlockNeedTeamLevel, ii.data.unlockNeedTeamLevel);
			}
		}
	}

	protected void UpdateExDetailUI(ITEMINFO ii)
	{
		if (ii.equipType == Defined.EQUIP_TYPE.Head)
		{
			DataConf.EquipData equipDataByIndex = DataCenter.Conf().GetEquipDataByIndex(ii.data.equipIndex);
			TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailName(equipDataByIndex.name);
			TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIcon(UIUtil.GetEquipTextureMaterial(equipDataByIndex.fileName).mainTexture);
			TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailCombat(string.Empty + ii.data.combat);
			TPDETAILEXDATAINFOSCRIPT.SetAllHelmsPropertyVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, true);
			TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, ii.data.level + string.Empty);
			TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, string.Empty);
			if (equipDataByIndex.hp[0] != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				if (ii.data.level + 1 < equipDataByIndex.hp.Length)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex.hp[ii.data.level + 1] * heroConf.equipAdditionPercent) - Mathf.CeilToInt((float)equipDataByIndex.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, string.Empty);
				}
			}
			if (equipDataByIndex.def != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, equipDataByIndex.def * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, string.Empty);
			}
			if (equipDataByIndex.proCrit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, equipDataByIndex.proCrit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, string.Empty);
			}
			if (equipDataByIndex.critDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, equipDataByIndex.critDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, string.Empty);
			}
			if (equipDataByIndex.proResilience != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, equipDataByIndex.proResilience + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, string.Empty);
			}
			if (equipDataByIndex.proHit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, equipDataByIndex.proHit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, string.Empty);
			}
			if (equipDataByIndex.hpPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, equipDataByIndex.hpPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, string.Empty);
			}
			if (equipDataByIndex.proDodge != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, equipDataByIndex.proDodge + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, string.Empty);
			}
			if (equipDataByIndex.atkPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, equipDataByIndex.atkPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, string.Empty);
			}
			if (equipDataByIndex.atkFrequencyPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, equipDataByIndex.atkFrequencyPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, string.Empty);
			}
			if (equipDataByIndex.reduceDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, equipDataByIndex.reduceDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, string.Empty);
			}
			if (equipDataByIndex.moveSpeedPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, equipDataByIndex.moveSpeedPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, string.Empty);
			}
			if (equipDataByIndex.atkRange != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, equipDataByIndex.atkRange * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, string.Empty);
			}
			TPDETAILEXDATAINFOSCRIPT.RepositionHelms();
			TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUpgradePartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUnlockPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailEquipPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUnlockPartEnable(true);
			if (ii.data.state == Defined.ItemState.Available)
			{
				if (playdata.equips[ii.equipType].currEquipIndex != ii.data.equipIndex)
				{
					TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailEquipPartVisable(true);
				}
				else if (ii.data.level < ii.data.maxLevel)
				{
					TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUpgradePartVisable(true);
					TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailUpgradePartGoldPrice(ii.data.cost + string.Empty);
				}
			}
			else if (ii.data.state == Defined.ItemState.Purchase)
			{
				TPDETAILEXDATAINFOSCRIPT.SetHelmsDetailUnlockPartVisable(true);
				TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailUnlockPartGoldPrice(ii.data.unlockMoney + string.Empty);
				if (ii.data.unlockCrystal >= 0)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailUnlockPartItemPrice(ii.data.unlockCrystal + string.Empty, true);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateHelmsDetailUnlockPartItemPrice(ii.data.unlockHonor + string.Empty, false);
				}
			}
			else if (ii.data.state != Defined.ItemState.FailByReasonOne)
			{
			}
		}
		else if (ii.equipType == Defined.EQUIP_TYPE.Body)
		{
			DataConf.EquipData equipDataByIndex2 = DataCenter.Conf().GetEquipDataByIndex(ii.data.equipIndex);
			TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailName(equipDataByIndex2.name);
			TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIcon(UIUtil.GetEquipTextureMaterial(equipDataByIndex2.fileName).mainTexture);
			TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailCombat(string.Empty + ii.data.combat);
			TPDETAILEXDATAINFOSCRIPT.SetAllArmorPropertyVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, true);
			TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, ii.data.level + string.Empty);
			TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, string.Empty);
			if (equipDataByIndex2.hp[0] != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex2.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				if (ii.data.level + 1 < equipDataByIndex2.hp.Length)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex2.hp[ii.data.level + 1] * heroConf.equipAdditionPercent) - Mathf.CeilToInt((float)equipDataByIndex2.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, string.Empty);
				}
			}
			if (equipDataByIndex2.def != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, equipDataByIndex2.def * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, string.Empty);
			}
			if (equipDataByIndex2.proCrit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, equipDataByIndex2.proCrit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, string.Empty);
			}
			if (equipDataByIndex2.critDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, equipDataByIndex2.critDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, string.Empty);
			}
			if (equipDataByIndex2.proResilience != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, equipDataByIndex2.proResilience + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, string.Empty);
			}
			if (equipDataByIndex2.proHit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, equipDataByIndex2.proHit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, string.Empty);
			}
			if (equipDataByIndex2.hpPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, equipDataByIndex2.hpPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, string.Empty);
			}
			if (equipDataByIndex2.proDodge != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, equipDataByIndex2.proDodge + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, string.Empty);
			}
			if (equipDataByIndex2.atkPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, equipDataByIndex2.atkPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, string.Empty);
			}
			if (equipDataByIndex2.atkFrequencyPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, equipDataByIndex2.atkFrequencyPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, string.Empty);
			}
			if (equipDataByIndex2.reduceDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, equipDataByIndex2.reduceDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, string.Empty);
			}
			if (equipDataByIndex2.moveSpeedPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, equipDataByIndex2.moveSpeedPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, string.Empty);
			}
			if (equipDataByIndex2.atkRange != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, equipDataByIndex2.atkRange * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, string.Empty);
			}
			TPDETAILEXDATAINFOSCRIPT.RepositionArmor();
			TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUpgradePartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUnlockPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetArmorDetailEquipPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetArmorDetailEquipPartEnabel(true);
			if (ii.data.state == Defined.ItemState.Available)
			{
				if (playdata.equips[ii.equipType].currEquipIndex != ii.data.equipIndex)
				{
					TPDETAILEXDATAINFOSCRIPT.SetArmorDetailEquipPartVisable(true);
				}
				else if (ii.data.level < ii.data.maxLevel)
				{
					TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUpgradePartVisable(true);
					TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailUpgradePartGoldPrice(ii.data.cost + string.Empty);
				}
			}
			else if (ii.data.state == Defined.ItemState.Purchase)
			{
				TPDETAILEXDATAINFOSCRIPT.SetArmorDetailUnlockPartVisable(true);
				TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailUnlockPartGoldPrice(ii.data.unlockMoney + string.Empty);
				if (ii.data.unlockCrystal >= 0)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailUnlockPartItemPrice(ii.data.unlockCrystal + string.Empty, true);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateArmorDetailUnlockPartItemPrice(ii.data.unlockHonor + string.Empty, false);
				}
			}
			else if (ii.data.state != Defined.ItemState.FailByReasonOne)
			{
			}
		}
		else
		{
			if (ii.equipType != Defined.EQUIP_TYPE.Acc)
			{
				return;
			}
			DataConf.EquipData equipDataByIndex3 = DataCenter.Conf().GetEquipDataByIndex(ii.data.equipIndex);
			TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailName(equipDataByIndex3.name);
			TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIcon(UIUtil.GetEquipTextureMaterial(equipDataByIndex3.fileName).mainTexture);
			TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailCombat(string.Empty + ii.data.combat);
			TPDETAILEXDATAINFOSCRIPT.SetAllOrnamentsPropertyVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, true);
			TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, ii.data.level + string.Empty);
			TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_level, string.Empty);
			if (equipDataByIndex3.hp[0] != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex3.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				if (ii.data.level + 1 < equipDataByIndex3.hp.Length)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, Mathf.CeilToInt((float)equipDataByIndex3.hp[ii.data.level + 1] * heroConf.equipAdditionPercent) - Mathf.CeilToInt((float)equipDataByIndex3.hp[ii.data.level] * heroConf.equipAdditionPercent) + string.Empty);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hp, string.Empty);
				}
			}
			if (equipDataByIndex3.def != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, equipDataByIndex3.def * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_def, string.Empty);
			}
			if (equipDataByIndex3.proCrit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, equipDataByIndex3.proCrit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proCrit, string.Empty);
			}
			if (equipDataByIndex3.critDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, equipDataByIndex3.critDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_critDamagePercent, string.Empty);
			}
			if (equipDataByIndex3.proResilience != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, equipDataByIndex3.proResilience + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proResilience, string.Empty);
			}
			if (equipDataByIndex3.proHit != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, equipDataByIndex3.proHit + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proHit, string.Empty);
			}
			if (equipDataByIndex3.hpPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, equipDataByIndex3.hpPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_hpPercent, string.Empty);
			}
			if (equipDataByIndex3.proDodge != 0)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, equipDataByIndex3.proDodge + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_proDodge, string.Empty);
			}
			if (equipDataByIndex3.atkPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, equipDataByIndex3.atkPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkPercent, string.Empty);
			}
			if (equipDataByIndex3.atkFrequencyPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, equipDataByIndex3.atkFrequencyPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkFrequencyPercent, string.Empty);
			}
			if (equipDataByIndex3.reduceDamagePercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, equipDataByIndex3.reduceDamagePercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_reduceDamagePercent, string.Empty);
			}
			if (equipDataByIndex3.moveSpeedPercent != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, equipDataByIndex3.moveSpeedPercent * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_moveSpeedPercent, string.Empty);
			}
			if (equipDataByIndex3.atkRange != 0f)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsPropertyVisable(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, equipDataByIndex3.atkRange * 100f + "%");
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailIncrementProperty(UtilUITeamPlayerDetailExDataInfo.PropertyIndex.E_atkRange, string.Empty);
			}
			TPDETAILEXDATAINFOSCRIPT.RepositionOrnaments();
			TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUpgradePartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUnlockPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailEquipPartVisable(false);
			TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailEquipPartEnable(true);
			if (ii.data.state == Defined.ItemState.Available)
			{
				if (playdata.equips[ii.equipType].currEquipIndex != ii.data.equipIndex)
				{
					TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailEquipPartVisable(true);
				}
				else if (ii.data.level < ii.data.maxLevel)
				{
					TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUpgradePartVisable(true);
					TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailUpgradePartGoldPrice(ii.data.cost + string.Empty);
				}
			}
			else if (ii.data.state == Defined.ItemState.Purchase)
			{
				TPDETAILEXDATAINFOSCRIPT.SetOrnamentsDetailUnlockPartVisable(true);
				TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailUnlockPartGoldPrice(ii.data.unlockMoney + string.Empty);
				if (ii.data.unlockCrystal >= 0)
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailUnlockPartItemPrice(ii.data.unlockCrystal + string.Empty, true);
				}
				else
				{
					TPDETAILEXDATAINFOSCRIPT.UpdateOrnamentsDetailUnlockPartItemPrice(ii.data.unlockHonor + string.Empty, false);
				}
			}
			else if (ii.data.state != Defined.ItemState.FailByReasonOne)
			{
			}
		}
	}
}
