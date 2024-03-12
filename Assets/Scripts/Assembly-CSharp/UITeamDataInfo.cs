using UnityEngine;

public class UITeamDataInfo : MonoBehaviour
{
	public UIImageButton[] equipmentButtons;

	public UILabel lvLabel;

	public UILabel expLabel;

	public UISlider lvSlider;

	public UILabel hpLabel;

	public UILabel atkLabel;

	public UILabel defLabel;

	public UILabel dredLabel;

	public UILabel critLabel;

	public UILabel hitLabel;

	public UILabel resLabel;

	public UIButton lvUpdateButton;

	public UIImageButton weaponButton;

	public UIButton skillButton;

	private void Awake()
	{
	}

	public void UpdateCharacterLVNUMInfo(string str)
	{
		if ((bool)lvLabel)
		{
			lvLabel.text = str;
		}
	}

	private void UpdateCharacterEXP(string str)
	{
		if ((bool)expLabel)
		{
			expLabel.text = str;
		}
	}

	private void UpdateCharacterLVSlider(float percent)
	{
		if ((bool)lvSlider)
		{
			lvSlider.sliderValue = percent;
		}
	}

	public void UpdateCharacterExpInfo(float expOwn, float expTotal)
	{
		if (expTotal == 0f)
		{
			UpdateCharacterEXP(expOwn + "/" + expTotal);
			UpdateCharacterLVSlider(0f);
		}
		else
		{
			float percent = expOwn / expTotal;
			UpdateCharacterEXP(expOwn + "/" + expTotal);
			UpdateCharacterLVSlider(percent);
		}
	}

	public void UpdateCharacterHPNUMInfo(string str)
	{
		if ((bool)hpLabel)
		{
			hpLabel.text = str;
		}
	}

	public void UpdateCharacterATKInfo(string str)
	{
		if ((bool)atkLabel)
		{
			atkLabel.text = str;
		}
	}

	public void UpdateCharacterDEFInfo(string str)
	{
		if ((bool)defLabel)
		{
			defLabel.text = str;
		}
	}

	public void UpdateCharacterDREDInfo(string str)
	{
		if ((bool)dredLabel)
		{
			dredLabel.text = str;
		}
	}

	public void UpdateCharacterCRITInfo(string str)
	{
		if ((bool)critLabel)
		{
			critLabel.text = str;
		}
	}

	public void UpdateCharacterHITInfo(string str)
	{
		if ((bool)hitLabel)
		{
			hitLabel.text = str;
		}
	}

	public void UpdateCharacterRESInfo(string str)
	{
	}

	protected void UpdateBackground(int index, Defined.RANK_TYPE rt)
	{
		if (equipmentButtons[index] != null)
		{
			string bk = string.Empty;
			string bkClick = string.Empty;
			UIUtil.GetBackgroundSpriteNameByRankType(rt, ref bk, ref bkClick);
			UpdateBackgroundImage(index, bk, bkClick);
		}
	}

	protected void UpdateBackgroundImage(int index, string bks, string bkCS)
	{
		equipmentButtons[index].normalSprite = bks;
		equipmentButtons[index].hoverSprite = bkCS;
		equipmentButtons[index].pressedSprite = bkCS;
		equipmentButtons[index].transform.Find("Background Image").GetComponent<UISprite>().spriteName = bks;
		equipmentButtons[index].enabled = false;
		equipmentButtons[index].enabled = true;
	}

	protected void UpdateEquipIconImage(int index, string spriteName)
	{
		if (equipmentButtons[index] != null)
		{
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(spriteName);
			equipmentButtons[index].transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().mainTexture = equipTextureMaterial.mainTexture;
		}
	}

	protected void UpdateEquipLvLabel(int index, int lv)
	{
		if (equipmentButtons[index] != null)
		{
			if (lv == -99)
			{
				equipmentButtons[index].transform.Find("Label").GetComponent<UILabel>().text = string.Empty;
			}
			else
			{
				equipmentButtons[index].transform.Find("Label").GetComponent<UILabel>().text = "Lv " + lv;
			}
		}
	}

	public void UpdateEquipIcon(int index, string spriteName, Defined.RANK_TYPE colorType, int lv)
	{
		UpdateEquipLvLabel(index, lv);
		UpdateEquipIconImage(index, spriteName);
		UpdateBackground(index, colorType);
	}

	public void SetLevelupBtnVisable(bool bShow)
	{
		if (lvUpdateButton != null)
		{
			lvUpdateButton.gameObject.SetActive(bShow);
		}
	}

	public void SetSkillBtnEnable(bool bEnable)
	{
		if (skillButton != null)
		{
			skillButton.isEnabled = bEnable;
		}
	}

	public void SetWeaponBtnEnable(bool bEnable)
	{
		if (weaponButton != null)
		{
			weaponButton.isEnabled = bEnable;
		}
	}

	public void UpdateSkillImage(string spriteName)
	{
		if (skillButton != null)
		{
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(spriteName);
			skillButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().mainTexture = equipTextureMaterial.mainTexture;
		}
	}

	public void UpdateSkillLVLabel(int lv)
	{
		if (skillButton != null)
		{
			if (lv == -99)
			{
				skillButton.transform.Find("Label").GetComponent<UILabel>().text = string.Empty;
			}
			else
			{
				skillButton.transform.Find("Label").GetComponent<UILabel>().text = "Lv " + lv;
			}
		}
	}

	protected void UpdateWeaponBackgroundColor(Defined.RANK_TYPE colorType)
	{
		if (weaponButton != null)
		{
			string bk = string.Empty;
			string bkClick = string.Empty;
			UIUtil.GetBackgroundSpriteNameByRankType(colorType, ref bk, ref bkClick);
			UpdateWeaponBackgroundColor(bk, bkClick);
		}
	}

	protected void UpdateWeaponBackgroundColor(string bks, string bkCS)
	{
		weaponButton.transform.Find("Background Image").GetComponent<UISprite>().spriteName = bks;
		weaponButton.normalSprite = bks;
		weaponButton.hoverSprite = bkCS;
		weaponButton.pressedSprite = bkCS;
	}

	protected void UpdateWeaponIconImage(string spriteName)
	{
		if (weaponButton != null)
		{
			if (spriteName == string.Empty)
			{
				spriteName = "4x4tou";
			}
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(spriteName);
			weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().mainTexture = equipTextureMaterial.mainTexture;
			weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().MakePixelPerfect();
			weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().width = (int)((float)weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().width * 0.8f);
			weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().height = (int)((float)weaponButton.transform.Find("Sprite (guanbi_guanbi)").GetComponent<UITexture>().height * 0.8f);
		}
	}

	protected void UpdateWeaponLVLabel(int lv)
	{
		if (weaponButton != null)
		{
			if (lv == -99)
			{
				weaponButton.transform.Find("Label").GetComponent<UILabel>().text = string.Empty;
			}
			else
			{
				weaponButton.transform.Find("Label").GetComponent<UILabel>().text = "Lv " + lv;
			}
		}
	}

	public void UpdateWeaponInfo(string spriteName, Defined.RANK_TYPE colorType, int weaponLV)
	{
		UpdateWeaponLVLabel(weaponLV);
		UpdateWeaponIconImage(spriteName);
		UpdateWeaponBackgroundColor(colorType);
	}

	public void SetIsPVPUI(bool bIsPVP)
	{
		if (bIsPVP)
		{
			SetLevelupBtnVisable(false);
			SetWeaponBtnEnable(false);
			SetSkillBtnEnable(false);
		}
		else
		{
			SetLevelupBtnVisable(true);
			SetWeaponBtnEnable(true);
			SetSkillBtnEnable(true);
		}
	}
}
