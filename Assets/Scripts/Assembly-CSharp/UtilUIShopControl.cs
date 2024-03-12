using System.Collections.Generic;
using UnityEngine;

public class UtilUIShopControl : MonoBehaviour
{
	protected class STOREITEM
	{
		public UtilUIShopStoreItem ui;

		public UtilUIShopStoreItemData data;

		public STOREITEM(GameObject go)
		{
			ui = go.GetComponent<UtilUIShopStoreItem>();
			data = go.GetComponent<UtilUIShopStoreItemData>();
		}
	}

	protected class IAPITEM
	{
		public UtilUIShopIAPItem ui;

		public UtilUIShopIAPItemData data;

		public IAPITEM(GameObject go, UtilUIShopIAPItemData _d)
		{
			ui = go.GetComponent<UtilUIShopIAPItem>();
			data = _d;
		}
	}

	[SerializeField]
	private UIToggle[] titleToggles;

	[SerializeField]
	private UILabel storeRPageName;

	[SerializeField]
	private UITexture storeRPageIcon;

	[SerializeField]
	private UILabel storeRPageIntro;

	[SerializeField]
	private UIImageButton storeRPageBuyBtn;

	[SerializeField]
	private UIGrid StoreGrid;

	[SerializeField]
	private AutoCreatByPrefab storeAutoCreat;

	[SerializeField]
	private GameObject dialogGO;

	[SerializeField]
	private UILabel dialogTitle;

	[SerializeField]
	private GameObject rewardPartCurrencyGO1;

	[SerializeField]
	private UISprite rewardPartCurrencyGO1Icon;

	[SerializeField]
	private UILabel rewardPartCurrencyGO1Label;

	[SerializeField]
	private GameObject rewardPartCurrencyGO2;

	[SerializeField]
	private UISprite rewardPartCurrencyGO2Icon;

	[SerializeField]
	private UILabel rewardPartCurrencyGO2Label;

	[SerializeField]
	private GameObject rewardPartCurrencyGO3;

	[SerializeField]
	private UISprite rewardPartCurrencyGO3Icon;

	[SerializeField]
	private UILabel rewardPartCurrencyGO3Label;

	protected Dictionary<int, STOREITEM> dictStoreItems = new Dictionary<int, STOREITEM>();

	protected Dictionary<GameObject, int> dictMapStoreItems = new Dictionary<GameObject, int>();

	protected int nowSelectStoreItemIndex = -1;

	[SerializeField]
	private UIGrid iapGrid;

	[SerializeField]
	private AutoCreatByPrefab iapAutoCreat;

	[SerializeField]
	private UIImageButton buyBtn;

	private UtilUIShopControl_BuyIAPFinished_Delegate buyIAPFinishedEvent;

	protected Dictionary<string, IAPITEM> dictIAPItems = new Dictionary<string, IAPITEM>();

	protected Dictionary<GameObject, string> dictMapIAPItems = new Dictionary<GameObject, string>();

	protected string nowSelectIAPItemIndex = string.Empty;

	public void SetVisable(bool bShow)
	{
		base.gameObject.SetActive(bShow);
		if (!bShow)
		{
			UtilIAPVerify.Instance().SetPurchaseIAP_SelfFinishedSuccessedEventDelegate(null);
		}
	}

	public void ShowRewardDialog(string title, int crystal, int money, int hero)
	{
		dialogGO.SetActive(true);
		rewardPartCurrencyGO1.SetActive(false);
		rewardPartCurrencyGO2.SetActive(false);
		rewardPartCurrencyGO3.SetActive(false);
		dialogTitle.text = title;
		if (crystal > 0)
		{
			rewardPartCurrencyGO1.SetActive(true);
			rewardPartCurrencyGO1Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal);
			rewardPartCurrencyGO1Label.text = string.Empty + crystal;
		}
		if (money > 0)
		{
			rewardPartCurrencyGO2.SetActive(true);
			rewardPartCurrencyGO2Icon.spriteName = UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Money);
			rewardPartCurrencyGO2Label.text = string.Empty + money;
			if (crystal > 0)
			{
				rewardPartCurrencyGO2.transform.localPosition = new Vector3(0f, 15f, 0f);
			}
			else
			{
				rewardPartCurrencyGO2.transform.localPosition = new Vector3(0f, 55f, 0f);
			}
		}
		if (hero < 0)
		{
			return;
		}
		rewardPartCurrencyGO3.SetActive(true);
		DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(hero);
		rewardPartCurrencyGO3.SetActive(true);
		rewardPartCurrencyGO3Icon.spriteName = heroDataByIndex.iconFileName;
		rewardPartCurrencyGO3Label.text = heroDataByIndex.name;
		if (crystal > 0)
		{
			if (money > 0)
			{
				rewardPartCurrencyGO3.transform.localPosition = new Vector3(0f, -30f, 0f);
			}
			else
			{
				rewardPartCurrencyGO3.transform.localPosition = new Vector3(0f, 10f, 0f);
			}
		}
		else if (money > 0)
		{
			rewardPartCurrencyGO3.transform.localPosition = new Vector3(0f, 10f, 0f);
		}
		else
		{
			rewardPartCurrencyGO3.transform.localPosition = new Vector3(0f, 50f, 0f);
		}
	}

	public void HideRewardDialog()
	{
		dialogGO.SetActive(false);
	}

	public void InitIAPShop(UtilUIShopIAPItemData[] lsItems, UtilUIShopControl_BuyIAPFinished_Delegate finishEvent)
	{
		ClearIAPItems();
		for (int i = 0; i < lsItems.Length; i++)
		{
			CreateIAPItem(lsItems[i]);
			SerializeIAPItem(lsItems[i].ID);
			UpdateIAPItemUI(lsItems[i].ID);
			if (i == 0)
			{
				IAPITEM iAPITEM = dictIAPItems[lsItems[i].ID];
				iAPITEM.ui.Selected();
			}
		}
		iapGrid.repositionNow = true;
		buyIAPFinishedEvent = finishEvent;
		UtilIAPVerify.Instance().SetPurchaseIAP_SelfFinishedSuccessedEventDelegate(OnStorePurchaseIAPFinished);
	}

	public void InitStoreShop(DataConf.StoreItemData[] lsItems)
	{
		ClearStoreItems();
		for (int i = 0; i < lsItems.Length; i++)
		{
			CreateStoreItem(lsItems[i].index);
			SerializeStoreItem(lsItems[i].index, lsItems[i]);
			UpdateStoreItemUI(lsItems[i].index);
		}
		StoreGrid.repositionNow = true;
	}

	public void HandleCloseBtn()
	{
		SetVisable(false);
		UIConstant.bNeedLoseConnect = true;
	}

	private void ClearIAPItems()
	{
		foreach (KeyValuePair<GameObject, string> dictMapIAPItem in dictMapIAPItems)
		{
			Object.Destroy(dictMapIAPItem.Key);
		}
		dictIAPItems.Clear();
		dictMapIAPItems.Clear();
	}

	protected void CreateIAPItem(UtilUIShopIAPItemData data)
	{
		GameObject gameObject = iapAutoCreat.CreatePefab(data.INDEX);
		IAPITEM value = new IAPITEM(gameObject, data);
		dictIAPItems.Add(data.ID, value);
		dictMapIAPItems.Add(gameObject, data.ID);
	}

	protected void SerializeIAPItem(string id)
	{
		IAPITEM iAPITEM = dictIAPItems[id];
		iAPITEM.ui.SetItemToggleStateChangeDelegate(HandleIAPItemSelectEvent);
	}

	protected void UpdateIAPItemUI(string id)
	{
		IAPITEM iAPITEM = dictIAPItems[id];
		iAPITEM.ui.UpdateUI(iAPITEM.data.COMMODITYISIAP, iAPITEM.data.ICONNAME, iAPITEM.data.NAME, iAPITEM.data.LIMITCOUNT, "$" + iAPITEM.data.PRICE, iAPITEM.data.CRYSTAL, iAPITEM.data.MONEY, iAPITEM.data.HERO, iAPITEM.data.BACKGROUNDSTATE);
	}

	public void RequsetCrystalExchangMoneyBySelectItem()
	{
		RequsetCrystalExchangMoney(nowSelectIAPItemIndex);
	}

	public void RequsetCrystalExchangMoney(string _iapID)
	{
		UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 37);
		UIDialogManager.Instance.ShowBlock(37);
		DataCenter.State().selectIAPID = _iapID;
		HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Store_CrystalExchangMoney, OnStoreCrystalExchangMoneyFinished);
	}

	protected void HandleIAPItemSelectEvent(GameObject go, bool bCheck)
	{
		if (bCheck)
		{
			nowSelectIAPItemIndex = dictMapIAPItems[go];
			IAPITEM iAPITEM = dictIAPItems[nowSelectIAPItemIndex];
			if (iAPITEM.data.LIMITCOUNT != 0)
			{
				buyBtn.normalSprite = "btn_classic01on_192x43_b__L15R15T0B0";
				buyBtn.hoverSprite = buyBtn.normalSprite;
				buyBtn.pressedSprite = "btn_classic01_on_192x43_s__L15R15T0B0";
				buyBtn.isEnabled = false;
				buyBtn.isEnabled = true;
			}
			else
			{
				buyBtn.normalSprite = "btn_classic03on_192x43_b__L15R15T0B0";
				buyBtn.hoverSprite = buyBtn.normalSprite;
				buyBtn.pressedSprite = "btn_classic03_on_192x43_s__L15R15T0B0";
				buyBtn.isEnabled = false;
				buyBtn.isEnabled = true;
			}
		}
	}

	protected void HandleBuyIAPItemBtnClickEvent()
	{
		UIConstant.bNeedLoseConnect = false;
		IAPITEM iAPITEM = dictIAPItems[nowSelectIAPItemIndex];
		if (iAPITEM.data.COMMODITYISIAP)
		{
			if (iAPITEM.data.LIMITCOUNT != 0)
			{
				if (!UtilIAPVerify.Instance()._CanIAPCertificate)
				{
					UIEffectManager.Instance.ShowEffect(UIEffectManager.EffectType.E_Loading, 8);
					UIDialogManager.Instance.ShowBlock(8);
					UtilIAPVerify.Instance().RequesetPurchaseIAP(nowSelectIAPItemIndex);
				}
				else
				{
					string str = "You must wait until your last purchase is completed.";
					UIDialogManager.Instance.ShowPopupA(str, UIWidget.Pivot.Center, true);
				}
			}
			else
			{
				string str2 = "You have already been purchased " + iAPITEM.data.NAME + ".";
				UIDialogManager.Instance.ShowDriftMsgInfoUI(str2);
			}
		}
		else
		{
			string dESCRIBE = iAPITEM.data.DESCRIBE;
			UIDialogManager.Instance.ShowPopupA(dESCRIBE, UIWidget.Pivot.Center, UIUtil.GetCurrencyIconSpriteNameByCurrencyType(Defined.COST_TYPE.Crystal), iAPITEM.data.CRYSTAL + string.Empty, true, RequsetCrystalExchangMoneyBySelectItem);
		}
	}

	public void OnStorePurchaseIAPFinished(int code, string IAPId)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 8);
		UIDialogManager.Instance.HideBlock(8);
		UIConstant.bNeedLoseConnect = true;
		string str = string.Empty;
		if (code != 0)
		{
			if (code < 0)
			{
				str = "System Error. Code: " + code;
				UIDialogManager.Instance.ShowPopupA(str, UIWidget.Pivot.Center, true);
				return;
			}
			switch (code)
			{
			case 1045:
				str = "Purchase failed.";
				break;
			case 1046:
				str = "Verification timed out! The request will continue in the background.";
				break;
			case 1047:
				str = "You have purchased.";
				break;
			case 1048:
				str = "You have purchased.";
				break;
			case 1049:
				str = "Purchase failed.";
				break;
			}
			UIDialogManager.Instance.ShowPopupA(str, UIWidget.Pivot.Center, true);
		}
		else
		{
			IAPITEM iAPITEM = dictIAPItems[IAPId];
			str = "You got " + UIUtil.GetCombinationString(UIUtil._UIGreenColor, iAPITEM.data.NAME) + ".";
			ShowRewardDialog(str, iAPITEM.data.CRYSTAL, iAPITEM.data.MONEY, iAPITEM.data.HERO);
			UpdateIAPItemUI(IAPId);
			if (buyIAPFinishedEvent != null)
			{
				buyIAPFinishedEvent(code);
			}
			else
			{
				UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
			}
		}
	}

	public void OnStoreCrystalExchangMoneyFinished(int code)
	{
		UIEffectManager.Instance.HideEffect(UIEffectManager.EffectType.E_Loading, 37);
		UIDialogManager.Instance.HideBlock(37);
		if (code != 0)
		{
			UIDialogManager.Instance.ShowHttpFeedBackMsg(code);
		}
		else if (buyIAPFinishedEvent != null)
		{
			buyIAPFinishedEvent(code);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	private void ClearStoreItems()
	{
		foreach (KeyValuePair<GameObject, int> dictMapStoreItem in dictMapStoreItems)
		{
			Object.Destroy(dictMapStoreItem.Key);
		}
		dictStoreItems.Clear();
		dictMapStoreItems.Clear();
	}

	protected void CreateStoreItem(int index)
	{
		GameObject gameObject = storeAutoCreat.CreatePefab(index);
		STOREITEM value = new STOREITEM(gameObject);
		dictStoreItems.Add(index, value);
		dictMapStoreItems.Add(gameObject, index);
	}

	protected void SerializeStoreItem(int index, DataConf.StoreItemData data)
	{
		STOREITEM sTOREITEM = dictStoreItems[index];
		List<KeyValuePair<Defined.COST_TYPE, int>> list = new List<KeyValuePair<Defined.COST_TYPE, int>>();
		if (Random.Range(0, 2) == 0)
		{
			KeyValuePair<Defined.COST_TYPE, int> item = new KeyValuePair<Defined.COST_TYPE, int>(Defined.COST_TYPE.Money, Random.Range(0, 999));
			list.Add(item);
		}
		else
		{
			KeyValuePair<Defined.COST_TYPE, int> item2 = new KeyValuePair<Defined.COST_TYPE, int>(Defined.COST_TYPE.Crystal, Random.Range(0, 999));
			list.Add(item2);
		}
		int ownCount = 0;
		if (DataCenter.Save().GetBagData().otherItems.ContainsKey(index))
		{
			ownCount = DataCenter.Save().GetBagData().otherItems[index].count;
		}
		DataConf.StuffData stuffDataByIndex = DataCenter.Conf().GetStuffDataByIndex(index);
		sTOREITEM.data.Init(index, "Store ID Test", "Name Test " + index, stuffDataByIndex.fileName, list, ownCount, string.Empty, (Random.Range(0, 3) != 0) ? true : false);
		sTOREITEM.ui.SetItemToggleStateChangeDelegate(HandleStoreItemSelectEvent);
	}

	protected void UpdateStoreItemUI(int index)
	{
		STOREITEM sTOREITEM = dictStoreItems[index];
		sTOREITEM.ui.UpdateUI(UIUtil.GetEquipTextureMaterial(sTOREITEM.data.ICONNAME).mainTexture, "Own: " + sTOREITEM.data.OWNCOUNT, UIUtil.GetCurrencyIconSpriteNameByCurrencyType(sTOREITEM.data.LSPRICES[0].Key), sTOREITEM.data.LSPRICES[0].Value + string.Empty);
	}

	protected void HandleBuyStoreItemBtnClickEvent()
	{
	}

	protected void HandleStoreItemSelectEvent(GameObject go, bool bCheck)
	{
		if (bCheck)
		{
			nowSelectStoreItemIndex = dictMapStoreItems[go];
			STOREITEM sTOREITEM = dictStoreItems[nowSelectStoreItemIndex];
			storeRPageName.text = sTOREITEM.data.name;
			storeRPageIcon.mainTexture = UIUtil.GetEquipTextureMaterial(sTOREITEM.data.ICONNAME).mainTexture;
			storeRPageIntro.text = sTOREITEM.data.INTRODUCE;
			if (sTOREITEM.data.CANBUY)
			{
				storeRPageBuyBtn.isEnabled = true;
			}
			else
			{
				storeRPageBuyBtn.isEnabled = false;
			}
		}
	}
}
