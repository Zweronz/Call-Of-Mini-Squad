using System.Collections.Generic;
using UnityEngine;

public class AmazonIAPEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		AmazonIAPManager.itemDataRequestFailedEvent += itemDataRequestFailedEvent;
		AmazonIAPManager.itemDataRequestFinishedEvent += itemDataRequestFinishedEvent;
		AmazonIAPManager.purchaseFailedEvent += purchaseFailedEvent;
		AmazonIAPManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		AmazonIAPManager.purchaseUpdatesRequestFailedEvent += purchaseUpdatesRequestFailedEvent;
		AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent += purchaseUpdatesRequestSuccessfulEvent;
		AmazonIAPManager.onSdkAvailableEvent += onSdkAvailableEvent;
		AmazonIAPManager.onGetUserIdResponseEvent += onGetUserIdResponseEvent;
	}

	private void OnDisable()
	{
		AmazonIAPManager.itemDataRequestFailedEvent -= itemDataRequestFailedEvent;
		AmazonIAPManager.itemDataRequestFinishedEvent -= itemDataRequestFinishedEvent;
		AmazonIAPManager.purchaseFailedEvent -= purchaseFailedEvent;
		AmazonIAPManager.purchaseSuccessfulEvent -= purchaseSuccessfulEvent;
		AmazonIAPManager.purchaseUpdatesRequestFailedEvent -= purchaseUpdatesRequestFailedEvent;
		AmazonIAPManager.purchaseUpdatesRequestSuccessfulEvent -= purchaseUpdatesRequestSuccessfulEvent;
		AmazonIAPManager.onSdkAvailableEvent -= onSdkAvailableEvent;
		AmazonIAPManager.onGetUserIdResponseEvent -= onGetUserIdResponseEvent;
	}

	private void itemDataRequestFailedEvent()
	{
	}

	private void itemDataRequestFinishedEvent(List<string> unavailableSkus, List<AmazonItem> availableItems)
	{
	}

	private void purchaseFailedEvent(string reason)
	{
	}

	private void purchaseSuccessfulEvent(AmazonReceipt receipt)
	{
	}

	private void purchaseUpdatesRequestFailedEvent()
	{
	}

	private void purchaseUpdatesRequestSuccessfulEvent(List<string> revokedSkus, List<AmazonReceipt> receipts)
	{
		foreach (AmazonReceipt receipt in receipts)
		{
		}
	}

	private void onSdkAvailableEvent(bool isTestMode)
	{
	}

	private void onGetUserIdResponseEvent(string userId)
	{
	}
}
