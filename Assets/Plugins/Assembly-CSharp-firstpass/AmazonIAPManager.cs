using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmazonIAPManager : MonoBehaviour
{
	public static event Action itemDataRequestFailedEvent;

	public static event Action<List<string>, List<AmazonItem>> itemDataRequestFinishedEvent;

	public static event Action<string> purchaseFailedEvent;

	public static event Action<AmazonReceipt> purchaseSuccessfulEvent;

	public static event Action purchaseUpdatesRequestFailedEvent;

	public static event Action<List<string>, List<AmazonReceipt>> purchaseUpdatesRequestSuccessfulEvent;

	public static event Action<bool> onSdkAvailableEvent;

	public static event Action<string> onGetUserIdResponseEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void itemDataRequestFailed(string empty)
	{
		if (AmazonIAPManager.itemDataRequestFailedEvent != null)
		{
			AmazonIAPManager.itemDataRequestFailedEvent();
		}
	}

	public void itemDataRequestFinished(string json)
	{
		if (AmazonIAPManager.itemDataRequestFinishedEvent != null)
		{
			Hashtable hashtable = json.hashtableFromJson();
			ArrayList arrayList = hashtable["unavailableSkus"] as ArrayList;
			ArrayList array = hashtable["availableSkus"] as ArrayList;
			AmazonIAPManager.itemDataRequestFinishedEvent(arrayList.ToList<string>(), AmazonItem.fromArrayList(array));
		}
	}

	public void purchaseFailed(string reason)
	{
		if (AmazonIAPManager.purchaseFailedEvent != null)
		{
			AmazonIAPManager.purchaseFailedEvent(reason);
		}
	}

	public void purchaseSuccessful(string json)
	{
		if (AmazonIAPManager.purchaseSuccessfulEvent != null)
		{
			AmazonIAPManager.purchaseSuccessfulEvent(new AmazonReceipt(json.hashtableFromJson()));
		}
	}

	public void purchaseUpdatesRequestFailed(string empty)
	{
		if (AmazonIAPManager.purchaseUpdatesRequestFailedEvent != null)
		{
			AmazonIAPManager.purchaseUpdatesRequestFailedEvent();
		}
	}

	public void purchaseUpdatesRequestSuccessful(string json)
	{
		if (AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent != null)
		{
			Hashtable hashtable = json.hashtableFromJson();
			ArrayList arrayList = hashtable["revokedSkus"] as ArrayList;
			ArrayList array = hashtable["receipts"] as ArrayList;
			AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent(arrayList.ToList<string>(), AmazonReceipt.fromArrayList(array));
		}
	}

	public void onSdkAvailable(string param)
	{
		if (AmazonIAPManager.onSdkAvailableEvent != null)
		{
			AmazonIAPManager.onSdkAvailableEvent(param == "1");
		}
	}

	public void onGetUserIdResponse(string param)
	{
		if (AmazonIAPManager.onGetUserIdResponseEvent != null)
		{
			AmazonIAPManager.onGetUserIdResponseEvent(param);
		}
	}
}
