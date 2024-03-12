using UnityEngine;

public class UITeamSkillUpdateMsgBox : MonoBehaviour
{
	public UIButton updateButton;

	public UIButton breakButton;

	public UITexture skillIconImage;

	public UILabel skillNameLabel;

	public UILabel introLabel;

	public UILabel baseSkillLV;

	public UILabel priceLabel;

	public UISprite[] iconMaterialBGImage;

	public UITexture[] iconMaterialImage;

	public UILabel[] iconMaterialCountLabel;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void BreakBtnVisable(bool bShow)
	{
		breakButton.gameObject.SetActive(bShow);
	}

	private void UpdateBtnVisable(bool bShow)
	{
		updateButton.gameObject.SetActive(bShow);
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 10000f, base.transform.localPosition.z);
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
	}

	public void ShowUpdateBtn(bool bShow)
	{
		if (bShow)
		{
			BreakBtnVisable(false);
			UpdateBtnVisable(true);
		}
		else
		{
			BreakBtnVisable(true);
			UpdateBtnVisable(false);
		}
	}

	public void UpdateIconImage(string spriteName)
	{
		if ((bool)skillIconImage)
		{
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(spriteName);
			skillIconImage.mainTexture = equipTextureMaterial.mainTexture;
			skillIconImage.MakePixelPerfect();
		}
	}

	public void UpdateSkillName(string str)
	{
		if ((bool)skillNameLabel)
		{
			skillNameLabel.color = Color.white;
			skillNameLabel.text = str;
		}
	}

	public void UpdateIntro(string str)
	{
		if ((bool)introLabel)
		{
			introLabel.text = str;
		}
	}

	public void UpdateLV(int nowLV, int maxLV)
	{
		if ((bool)baseSkillLV)
		{
			baseSkillLV.text = "LV" + nowLV + "/[00ff00]" + maxLV + "[-]";
		}
	}

	public void UpdatePrice(string str)
	{
		if ((bool)priceLabel)
		{
			priceLabel.text = str;
		}
	}

	public void SetMatericalPartAllHidden()
	{
		for (int i = 0; i < iconMaterialBGImage.Length; i++)
		{
			iconMaterialBGImage[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < iconMaterialImage.Length; j++)
		{
			iconMaterialImage[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < iconMaterialCountLabel.Length; k++)
		{
			iconMaterialCountLabel[k].gameObject.SetActive(false);
		}
	}

	public void UpdateMaterialsInfo(int index, string strbgIcon, string strIcon, int ownCount, int needCount)
	{
		if (iconMaterialBGImage[index] != null && iconMaterialImage[index] != null && iconMaterialCountLabel[index] != null)
		{
			iconMaterialBGImage[index].gameObject.SetActive(true);
			iconMaterialImage[index].gameObject.SetActive(true);
			iconMaterialCountLabel[index].gameObject.SetActive(true);
			iconMaterialBGImage[index].spriteName = strbgIcon;
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(strIcon);
			iconMaterialImage[index].mainTexture = equipTextureMaterial.mainTexture;
			string empty = string.Empty;
			empty = ((ownCount >= needCount) ? UIUtil.GetCombinationString(UIUtil._UIGreenColor, ownCount + string.Empty) : UIUtil.GetCombinationString(UIUtil._UIRedColor, ownCount + string.Empty));
			iconMaterialCountLabel[index].text = empty + "/" + needCount;
		}
	}
}
