using System;
using System.Collections.Generic;
using UnityEngine;

public class TapjoyPlugin : MonoBehaviour
{
	public const string MacAddressOptionOn = "0";

	public const string MacAddressOptionOffWithVersionCheck = "1";

	public const string MacAddressOptionOff = "2";

	private static Dictionary<string, TapjoyEvent> eventDictionary = new Dictionary<string, TapjoyEvent>();

	public static event Action connectCallSucceeded;

	public static event Action connectCallFailed;

	public static event Action<int> getTapPointsSucceeded;

	public static event Action getTapPointsFailed;

	public static event Action<int> spendTapPointsSucceeded;

	public static event Action spendTapPointsFailed;

	public static event Action awardTapPointsSucceeded;

	public static event Action awardTapPointsFailed;

	public static event Action<int> tapPointsEarned;

	public static event Action getFullScreenAdSucceeded;

	public static event Action getFullScreenAdFailed;

	public static event Action getDisplayAdSucceeded;

	public static event Action getDisplayAdFailed;

	public static event Action videoAdStarted;

	public static event Action videoAdFailed;

	public static event Action videoAdCompleted;

	public static event Action showOffersFailed;

	public static event Action<TapjoyViewType> viewOpened;

	public static event Action<TapjoyViewType> viewClosed;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		SetCallbackHandler(base.gameObject.name);
		Debug.Log("C#: UnitySendMessage directs to " + base.gameObject.name);
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void TapjoyConnectSuccess(string message)
	{
		if (TapjoyPlugin.connectCallSucceeded != null)
		{
			TapjoyPlugin.connectCallSucceeded();
		}
	}

	public void TapjoyConnectFail(string message)
	{
		if (TapjoyPlugin.connectCallFailed != null)
		{
			TapjoyPlugin.connectCallFailed();
		}
	}

	public void TapPointsLoaded(string message)
	{
		if (TapjoyPlugin.getTapPointsSucceeded != null)
		{
			TapjoyPlugin.getTapPointsSucceeded(int.Parse(message));
		}
	}

	public void TapPointsLoadedError(string message)
	{
		if (TapjoyPlugin.getTapPointsFailed != null)
		{
			TapjoyPlugin.getTapPointsFailed();
		}
	}

	public void TapPointsSpent(string message)
	{
		if (TapjoyPlugin.spendTapPointsSucceeded != null)
		{
			TapjoyPlugin.spendTapPointsSucceeded(int.Parse(message));
		}
	}

	public void TapPointsSpendError(string message)
	{
		if (TapjoyPlugin.spendTapPointsFailed != null)
		{
			TapjoyPlugin.spendTapPointsFailed();
		}
	}

	public void TapPointsAwarded(string message)
	{
		if (TapjoyPlugin.awardTapPointsSucceeded != null)
		{
			TapjoyPlugin.awardTapPointsSucceeded();
		}
	}

	public void TapPointsAwardError(string message)
	{
		if (TapjoyPlugin.awardTapPointsFailed != null)
		{
			TapjoyPlugin.awardTapPointsFailed();
		}
	}

	public void CurrencyEarned(string message)
	{
		if (TapjoyPlugin.tapPointsEarned != null)
		{
			TapjoyPlugin.tapPointsEarned(int.Parse(message));
		}
	}

	public void FullScreenAdLoaded(string message)
	{
		if (TapjoyPlugin.getFullScreenAdSucceeded != null)
		{
			TapjoyPlugin.getFullScreenAdSucceeded();
		}
	}

	public void FullScreenAdError(string message)
	{
		if (TapjoyPlugin.getFullScreenAdFailed != null)
		{
			TapjoyPlugin.getFullScreenAdFailed();
		}
	}

	public void DisplayAdLoaded(string message)
	{
		if (TapjoyPlugin.getDisplayAdSucceeded != null)
		{
			TapjoyPlugin.getDisplayAdSucceeded();
		}
	}

	public void DisplayAdError(string message)
	{
		if (TapjoyPlugin.getDisplayAdFailed != null)
		{
			TapjoyPlugin.getDisplayAdFailed();
		}
	}

	public void VideoAdStart(string message)
	{
		if (TapjoyPlugin.videoAdStarted != null)
		{
			TapjoyPlugin.videoAdStarted();
		}
	}

	public void VideoAdError(string message)
	{
		if (TapjoyPlugin.videoAdFailed != null)
		{
			TapjoyPlugin.videoAdFailed();
		}
	}

	public void VideoAdComplete(string message)
	{
		if (TapjoyPlugin.videoAdCompleted != null)
		{
			TapjoyPlugin.videoAdCompleted();
		}
	}

	public void ShowOffersError(string message)
	{
		if (TapjoyPlugin.showOffersFailed != null)
		{
			TapjoyPlugin.showOffersFailed();
		}
	}

	public void ViewOpened(string message)
	{
		if (TapjoyPlugin.viewOpened != null)
		{
			int obj = int.Parse(message);
			TapjoyPlugin.viewOpened((TapjoyViewType)obj);
		}
	}

	public void ViewClosed(string message)
	{
		if (TapjoyPlugin.viewClosed != null)
		{
			int obj = int.Parse(message);
			TapjoyPlugin.viewClosed((TapjoyViewType)obj);
		}
	}

	public static void SetCallbackHandler(string handlerName)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SetCallbackHandler(handlerName);
		}
	}

	public static void RequestTapjoyConnect(string appID, string secretKey)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.RequestTapjoyConnect(appID, secretKey);
		}
	}

	public static void RequestTapjoyConnect(string appID, string secretKey, Dictionary<string, string> flags)
	{
		if (Application.isEditor)
		{
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, string> flag in flags)
		{
			dictionary.Add(flag.Key, flag.Value);
		}
		TapjoyPluginAndroid.RequestTapjoyConnect(appID, secretKey, dictionary);
	}

	public static void RequestTapjoyConnect(string appID, string secretKey, Dictionary<string, object> flags)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.RequestTapjoyConnect(appID, secretKey, flags);
		}
	}

	public static void EnableLogging(bool enable)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EnableLogging(enable);
		}
	}

	public static void SendSegmentationParams(Dictionary<string, object> segmentationParams)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SendSegmentationParams(segmentationParams);
		}
	}

	public static void AppPause()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.AppPause();
		}
	}

	public static void AppResume()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.AppResume();
		}
	}

	public static void ActionComplete(string actionID)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ActionComplete(actionID);
		}
	}

	public static void SetUserID(string userID)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SetUserID(userID);
		}
	}

	public static void ShowOffers()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowOffers();
		}
	}

	public static void GetTapPoints()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.GetTapPoints();
		}
	}

	public static void SpendTapPoints(int points)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SpendTapPoints(points);
		}
	}

	public static void AwardTapPoints(int points)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.AwardTapPoints(points);
		}
	}

	public static int QueryTapPoints()
	{
		if (Application.isEditor)
		{
			return 0;
		}
		return TapjoyPluginAndroid.QueryTapPoints();
	}

	public static void ShowDefaultEarnedCurrencyAlert()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowDefaultEarnedCurrencyAlert();
		}
	}

	[Obsolete("GetDisplayAd is deprecated since 10.2.0")]
	public static void GetDisplayAd()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.GetDisplayAd();
		}
	}

	[Obsolete("ShowDisplayAd is deprecated since 10.2.0")]
	public static void ShowDisplayAd()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowDisplayAd();
		}
	}

	[Obsolete("HideDisplayAd is deprecated since 10.2.0")]
	public static void HideDisplayAd()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.HideDisplayAd();
		}
	}

	[Obsolete("SetDisplayAdContentSize is deprecated.")]
	public static void SetDisplayAdContentSize(int size)
	{
		if (!Application.isEditor)
		{
			SetDisplayAdSize((TapjoyDisplayAdSize)size);
		}
	}

	[Obsolete("SetDisplayAdSize is deprecated since 10.2.0")]
	public static void SetDisplayAdSize(TapjoyDisplayAdSize size)
	{
		if (!Application.isEditor)
		{
			string displayAdSize = "320x50";
			if (size == TapjoyDisplayAdSize.SIZE_640X100)
			{
				displayAdSize = "640x100";
			}
			if (size == TapjoyDisplayAdSize.SIZE_768X90)
			{
				displayAdSize = "768x90";
			}
			TapjoyPluginAndroid.SetDisplayAdSize(displayAdSize);
		}
	}

	[Obsolete("EnableDisplayAdAutoRefresh is deprecated since 10.2.0")]
	public static void EnableDisplayAdAutoRefresh(bool enable)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EnableDisplayAdAutoRefresh(enable);
		}
	}

	[Obsolete("MoveDisplayAd is deprecated since 10.2.0")]
	public static void MoveDisplayAd(int x, int y)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.MoveDisplayAd(x, y);
		}
	}

	[Obsolete("SetTransitionEffect is deprecated since 10.0.0")]
	public static void SetTransitionEffect(int transition)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SetTransitionEffect(transition);
		}
	}

	[Obsolete("GetFullScreenAd is deprecated since 10.0.0")]
	public static void GetFullScreenAd()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.GetFullScreenAd();
		}
	}

	[Obsolete("ShowFullScreenAd is deprecated since 10.0.0. Tapjoy ad units now use TJEvent")]
	public static void ShowFullScreenAd()
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowFullScreenAd();
		}
	}

	[Obsolete("SetVideoCacheCount is deprecated, video is now controlled via your Tapjoy Dashboard.")]
	public static void SetVideoCacheCount(int cacheCount)
	{
	}

	public static void SendShutDownEvent()
	{
		TapjoyPluginAndroid.SendShutDownEvent();
	}

	public static void SendIAPEvent(string name, float price, int quantity, string currencyCode)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SendIAPEvent(name, price, quantity, currencyCode);
		}
	}

	public static void ShowOffersWithCurrencyID(string currencyID, bool selector)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowOffersWithCurrencyID(currencyID, selector);
		}
	}

	public static void GetDisplayAdWithCurrencyID(string currencyID)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.GetDisplayAdWithCurrencyID(currencyID);
		}
	}

	public static void GetFullScreenAdWithCurrencyID(string currencyID)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.GetFullScreenAdWithCurrencyID(currencyID);
		}
	}

	public static void SetCurrencyMultiplier(float multiplier)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.SetCurrencyMultiplier(multiplier);
		}
	}

	public void SendEventCompleteWithContent(string guid)
	{
		if (eventDictionary != null && eventDictionary.ContainsKey(guid))
		{
			eventDictionary[guid].TriggerSendEventSucceeded(true);
		}
	}

	public void SendEventComplete(string guid)
	{
		if (eventDictionary != null && eventDictionary.ContainsKey(guid))
		{
			eventDictionary[guid].TriggerSendEventSucceeded(false);
		}
	}

	public void ContentIsReady(string parametersAsString)
	{
		string[] array = parametersAsString.Split(',');
		if (array.Length == 2)
		{
			string key = array[0];
			int status = Convert.ToInt32(array[1]);
			if (eventDictionary != null && eventDictionary.ContainsKey(key))
			{
				eventDictionary[key].TriggerContentIsReady(status);
			}
		}
	}

	public void SendEventFail(string guid)
	{
		if (eventDictionary != null && eventDictionary.ContainsKey(guid))
		{
			eventDictionary[guid].TriggerSendEventFailed(null);
		}
	}

	public void ContentDidAppear(string guid)
	{
		if (eventDictionary != null && eventDictionary.ContainsKey(guid))
		{
			eventDictionary[guid].TriggerContentDidAppear();
		}
	}

	public void ContentDidDisappear(string guid)
	{
		if (eventDictionary != null && eventDictionary.ContainsKey(guid))
		{
			eventDictionary[guid].TriggerContentDidDisappear();
		}
	}

	public void DidRequestAction(string message)
	{
		string[] array = message.Split(',');
		string key = array[0];
		int result;
		int.TryParse(array[1], out result);
		string identifier = array[2];
		int result2;
		int.TryParse(array[3], out result2);
		if (eventDictionary != null && eventDictionary.ContainsKey(key))
		{
			eventDictionary[key].TriggerDidRequestAction(result, identifier, result2);
		}
	}

	public static string CreateEvent(TapjoyEvent eventRef, string eventName, string eventParameter)
	{
		if (Application.isEditor)
		{
			return null;
		}
		string text = Guid.NewGuid().ToString();
		while (eventDictionary.ContainsKey(text))
		{
			text = Guid.NewGuid().ToString();
		}
		eventDictionary.Add(text, eventRef);
		TapjoyPluginAndroid.CreateEvent(text, eventName, eventParameter);
		return text;
	}

	public static void SendEvent(string guid)
	{
		TapjoyPluginAndroid.SendEvent(guid);
	}

	public static void ShowEvent(string guid)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.ShowEvent(guid);
		}
	}

	public static void EnableEventAutoPresent(string guid, bool autoPresent)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EnableEventAutoPresent(guid, autoPresent);
		}
	}

	public static void EnableEventPreload(string guid, bool preload)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EnableEventPreload(guid, preload);
		}
	}

	public static void EventRequestCompleted(string guid)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EventRequestCompleted(guid);
		}
	}

	public static void EventRequestCancelled(string guid)
	{
		if (!Application.isEditor)
		{
			TapjoyPluginAndroid.EventRequestCancelled(guid);
		}
	}
}
