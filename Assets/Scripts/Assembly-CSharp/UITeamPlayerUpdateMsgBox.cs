using System;
using UnityEngine;

public class UITeamPlayerUpdateMsgBox : MonoBehaviour
{
	[Serializable]
	public class Info
	{
		public UILabel intro;

		public UITexture sprite;

		public UISprite bg;

		public UILabel own;

		public UIButton useBtn;
	}

	public UITexture modelTexture;

	public UILabel nameLabel;

	public UILabel lvLabel;

	public UILabel expLabel;

	public UISlider lvSlider;

	public Info[] lsExpInfo = new Info[3];

	public void Hide()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 10000f, base.transform.localPosition.z);
	}

	public void Show()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
	}

	public void UpdateAvatarModel(UITexture texture)
	{
		if ((bool)modelTexture)
		{
			modelTexture = texture;
		}
	}

	public void UpdateAvatarName(string name)
	{
		if ((bool)nameLabel)
		{
			nameLabel.text = name;
		}
	}

	public void UpdateAvatarModelUV(Rect uv)
	{
		if ((bool)modelTexture)
		{
			modelTexture.uvRect = uv;
		}
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

	public void UpdateIntro(int index, string strIcon, string bg, string name, string intro)
	{
		if (index < lsExpInfo.Length)
		{
			Material equipTextureMaterial = UIUtil.GetEquipTextureMaterial(strIcon);
			lsExpInfo[index].sprite.mainTexture = equipTextureMaterial.mainTexture;
			lsExpInfo[index].bg.spriteName = bg;
			lsExpInfo[index].intro.text = name + "\n" + intro + "\nYOU OWN:";
		}
	}

	public void UpdateCount(int index, int count)
	{
		string str = count + string.Empty;
		if (count > 0)
		{
			str = UIUtil.GetCombinationString(UIUtil._UIGreenColor, str);
			lsExpInfo[index].useBtn.isEnabled = true;
		}
		else
		{
			str = UIUtil.GetCombinationString(UIUtil._UIRedColor, str);
			lsExpInfo[index].useBtn.isEnabled = false;
		}
		UpdateCount(index, str);
	}

	private void UpdateCount(int index, string count)
	{
		if (index < lsExpInfo.Length)
		{
			lsExpInfo[index].own.text = count;
		}
	}
}
