using UnityEngine;

public class UINewStoreManager : MonoBehaviour
{
	[SerializeField]
	private UtilUIPropertyInfo m_scriptUIPropertyInfo;

	public UtilUIPropertyInfo UIPROPERTYINFO
	{
		get
		{
			return m_scriptUIPropertyInfo;
		}
	}

	private void Start()
	{
		UIDialogManager.Instance.SetPropertyScript(UIPROPERTYINFO);
		BlindFunction();
		UpdatePropertyInfoPart("Map", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
		UIDialogManager.Instance.ShowShopDialogUI(HandleBuyIAPFinishedEvent);
	}

	private void BlindFunction()
	{
		UIPROPERTYINFO.SetBackBtnClickDelegate(HandleBackBtnClickEvent);
	}

	public void UpdatePropertyInfoPart(string name, int rank, int gold, int crystal)
	{
		if (name != "null#")
		{
			UIPROPERTYINFO.UpdateName(name);
		}
		if (rank > 0)
		{
			UIPROPERTYINFO.UpdateRank(rank.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateRank(0 + string.Empty);
		}
		if (gold > 0)
		{
			UIPROPERTYINFO.UpdateGold(gold.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateGold(0 + string.Empty);
		}
		if (crystal > 0)
		{
			UIPROPERTYINFO.UpdateCrystal(crystal.ToString("###, ###"));
		}
		else
		{
			UIPROPERTYINFO.UpdateCrystal(0 + string.Empty);
		}
	}

	public void HandleBuyIAPFinishedEvent(int code)
	{
		UpdatePropertyInfoPart("Store", DataCenter.Save().Honor, DataCenter.Save().Money, DataCenter.Save().Crystal);
	}

	public void HandleBackBtnClickEvent()
	{
		SceneManager.Instance.SwitchScene(string.Empty);
	}
}
