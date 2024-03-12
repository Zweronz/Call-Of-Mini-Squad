using System;
using System.Collections;
using UnityEngine;

namespace ChartboostSDK
{
	public class Chartboost : MonoBehaviour
	{
		private static Chartboost sInstance;

		private static bool isPaused;

		private static float lastTimeScale;

		public static event Func<CBLocation, bool> shouldDisplayInterstitial;

		public static event Action<CBLocation> didDisplayInterstitial;

		public static event Action<CBLocation> didCacheInterstitial;

		public static event Action<CBLocation> didClickInterstitial;

		public static event Action<CBLocation> didCloseInterstitial;

		public static event Action<CBLocation> didDismissInterstitial;

		public static event Action<CBLocation, CBImpressionError> didFailToLoadInterstitial;

		public static event Action<CBLocation, CBClickError> didFailToRecordClick;

		public static event Func<CBLocation, bool> shouldDisplayMoreApps;

		public static event Action<CBLocation> didDisplayMoreApps;

		public static event Action<CBLocation> didCacheMoreApps;

		public static event Action<CBLocation> didClickMoreApps;

		public static event Action<CBLocation> didCloseMoreApps;

		public static event Action<CBLocation> didDismissMoreApps;

		public static event Action<CBLocation, CBImpressionError> didFailToLoadMoreApps;

		public static event Func<CBLocation, bool> shouldDisplayRewardedVideo;

		public static event Action<CBLocation> didDisplayRewardedVideo;

		public static event Action<CBLocation> didCacheRewardedVideo;

		public static event Action<CBLocation> didClickRewardedVideo;

		public static event Action<CBLocation> didCloseRewardedVideo;

		public static event Action<CBLocation> didDismissRewardedVideo;

		public static event Action<CBLocation, int> didCompleteRewardedVideo;

		public static event Action<CBLocation, CBImpressionError> didFailToLoadRewardedVideo;

		public static event Action<CBLocation> didCacheInPlay;

		public static event Action<CBLocation, CBImpressionError> didFailToLoadInPlay;

		public static event Action<CBLocation> willDisplayVideo;

		public static event Action didPauseClickForConfirmation;

		public static Chartboost Instance()
		{
			return sInstance;
		}

		public static bool isAnyViewVisible()
		{
			return CBExternal.isAnyViewVisible();
		}

		public static void cacheInterstitial(CBLocation location)
		{
			CBExternal.cacheInterstitial(location);
		}

		public static bool hasInterstitial(CBLocation location)
		{
			return CBExternal.hasInterstitial(location);
		}

		public static void showInterstitial(CBLocation location)
		{
			CBExternal.showInterstitial(location);
		}

		public static void cacheMoreApps(CBLocation location)
		{
			CBExternal.cacheMoreApps(location);
		}

		public static bool hasMoreApps(CBLocation location)
		{
			return CBExternal.hasMoreApps(location);
		}

		public static void showMoreApps(CBLocation location)
		{
			CBExternal.showMoreApps(location);
		}

		public static void cacheRewardedVideo(CBLocation location)
		{
			CBExternal.cacheRewardedVideo(location);
		}

		public static bool hasRewardedVideo(CBLocation location)
		{
			return CBExternal.hasRewardedVideo(location);
		}

		public static void showRewardedVideo(CBLocation location)
		{
			CBExternal.showRewardedVideo(location);
		}

		public static void cacheInPlay(CBLocation location)
		{
			CBExternal.cacheInPlay(location);
		}

		public static bool hasInPlay(CBLocation location)
		{
			return CBExternal.hasInPlay(location);
		}

		public static CBInPlay getInPlay(CBLocation location)
		{
			return CBExternal.getInPlay(location);
		}

		public static void didPassAgeGate(bool pass)
		{
			CBExternal.didPassAgeGate(pass);
		}

		public static void setShouldPauseClickForConfirmation(bool shouldPause)
		{
			CBExternal.setShouldPauseClickForConfirmation(shouldPause);
		}

		public static string getCustomId()
		{
			return CBExternal.getCustomId();
		}

		public static void setCustomId(string customId)
		{
			CBExternal.setCustomId(customId);
		}

		public static bool getAutoCacheAds()
		{
			return CBExternal.getAutoCacheAds();
		}

		public static void setAutoCacheAds(bool autoCacheAds)
		{
			CBExternal.setAutoCacheAds(autoCacheAds);
		}

		public static void setShouldRequestInterstitialsInFirstSession(bool shouldRequest)
		{
			CBExternal.setShouldRequestInterstitialsInFirstSession(shouldRequest);
		}

		public static void setShouldDisplayLoadingViewForMoreApps(bool shouldDisplay)
		{
			CBExternal.setShouldDisplayLoadingViewForMoreApps(shouldDisplay);
		}

		public static void setShouldPrefetchVideoContent(bool shouldPrefetch)
		{
			CBExternal.setShouldPrefetchVideoContent(shouldPrefetch);
		}

		public static void trackLevelInfo(string eventLabel, CBLevelType type, int mainLevel, int subLevel, string description)
		{
			CBExternal.trackLevelInfo(eventLabel, type, mainLevel, subLevel, description);
		}

		public static void trackLevelInfo(string eventLabel, CBLevelType type, int mainLevel, string description)
		{
			CBExternal.trackLevelInfo(eventLabel, type, mainLevel, description);
		}

		public static void trackInAppGooglePlayPurchaseEvent(string title, string description, string price, string currency, string productID, string purchaseData, string purchaseSignature)
		{
			CBExternal.trackInAppGooglePlayPurchaseEvent(title, description, price, currency, productID, purchaseData, purchaseSignature);
		}

		public static void trackInAppAmazonStorePurchaseEvent(string title, string description, string price, string currency, string productID, string userID, string purchaseToken)
		{
			CBExternal.trackInAppAmazonStorePurchaseEvent(title, description, price, currency, productID, userID, purchaseToken);
		}

		private void Awake()
		{
			sInstance = this;
		}

		public void OnInit(string appID, string appSecret)
		{
			CBExternal.init(appID, appSecret);
			CBExternal.setGameObjectName(base.gameObject.name);
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape) && !CBExternal.onBackPressed())
			{
			}
		}

		private void OnGUI()
		{
			if (isAnyViewVisible())
			{
				Rect clientRect = new Rect(0f, 0f, Screen.width, Screen.height);
				clientRect = GUI.ModalWindow(0, clientRect, BlockerWindow, string.Empty);
			}
		}

		private void BlockerWindow(int windowID)
		{
		}

		private void OnApplicationPause(bool paused)
		{
			CBExternal.pause(paused);
		}

		private void OnDisable()
		{
			CBExternal.destroy();
		}

		private static CBImpressionError impressionErrorFromInt(object errorObj)
		{
			bool flag = Application.platform == RuntimePlatform.IPhonePlayer;
			int num;
			try
			{
				num = Convert.ToInt32(errorObj);
			}
			catch
			{
				num = -1;
			}
			int num2 = 10;
			if (!flag)
			{
				num2 = 18;
			}
			if (num < 0 || num > num2)
			{
				return CBImpressionError.Internal;
			}
			if (flag && num == 8)
			{
				return CBImpressionError.UserCancellation;
			}
			if (flag && num == 9)
			{
				return CBImpressionError.InvalidLocation;
			}
			if (flag && num == 10)
			{
				return CBImpressionError.PrefetchingIncomplete;
			}
			return (CBImpressionError)num;
		}

		private static CBClickError clickErrorFromInt(object errorObj)
		{
			int num;
			try
			{
				num = Convert.ToInt32(errorObj);
			}
			catch
			{
				num = -1;
			}
			int num2 = 3;
			if (num < 0 || num > num2)
			{
				return CBClickError.Internal;
			}
			return (CBClickError)num;
		}

		private void didFailToLoadInterstitialEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			CBImpressionError arg = impressionErrorFromInt(hashtable["errorCode"]);
			if (Chartboost.didFailToLoadInterstitial != null)
			{
				Chartboost.didFailToLoadInterstitial(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didDismissInterstitialEvent(string location)
		{
			doUnityPause(false);
			if (Chartboost.didDismissInterstitial != null)
			{
				Chartboost.didDismissInterstitial(CBLocation.locationFromName(location));
			}
		}

		private void didClickInterstitialEvent(string location)
		{
			if (Chartboost.didClickInterstitial != null)
			{
				Chartboost.didClickInterstitial(CBLocation.locationFromName(location));
			}
		}

		private void didCloseInterstitialEvent(string location)
		{
			if (Chartboost.didCloseInterstitial != null)
			{
				Chartboost.didCloseInterstitial(CBLocation.locationFromName(location));
			}
		}

		private void didCacheInterstitialEvent(string location)
		{
			if (Chartboost.didCacheInterstitial != null)
			{
				Chartboost.didCacheInterstitial(CBLocation.locationFromName(location));
			}
		}

		private void shouldDisplayInterstitialEvent(string location)
		{
			bool flag = true;
			if (Chartboost.shouldDisplayInterstitial != null)
			{
				flag = Chartboost.shouldDisplayInterstitial(CBLocation.locationFromName(location));
			}
			CBExternal.chartBoostShouldDisplayInterstitialCallbackResult(flag);
			if (flag)
			{
				showInterstitial(CBLocation.locationFromName(location));
			}
		}

		public void didDisplayInterstitialEvent(string location)
		{
			doUnityPause(true);
			if (Chartboost.didDisplayInterstitial != null)
			{
				Chartboost.didDisplayInterstitial(CBLocation.locationFromName(location));
			}
		}

		private void didFailToLoadMoreAppsEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			CBImpressionError arg = impressionErrorFromInt(hashtable["errorCode"]);
			if (Chartboost.didFailToLoadMoreApps != null)
			{
				Chartboost.didFailToLoadMoreApps(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didDismissMoreAppsEvent(string location)
		{
			doUnityPause(false);
			if (Chartboost.didDismissMoreApps != null)
			{
				Chartboost.didDismissMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void didClickMoreAppsEvent(string location)
		{
			if (Chartboost.didClickMoreApps != null)
			{
				Chartboost.didClickMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void didCloseMoreAppsEvent(string location)
		{
			if (Chartboost.didCloseMoreApps != null)
			{
				Chartboost.didCloseMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void didCacheMoreAppsEvent(string location)
		{
			if (Chartboost.didCacheMoreApps != null)
			{
				Chartboost.didCacheMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void shouldDisplayMoreAppsEvent(string location)
		{
			bool flag = true;
			if (Chartboost.shouldDisplayMoreApps != null)
			{
				flag = Chartboost.shouldDisplayMoreApps(CBLocation.locationFromName(location));
			}
			CBExternal.chartBoostShouldDisplayMoreAppsCallbackResult(flag);
			if (flag)
			{
				showMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void didDisplayMoreAppsEvent(string location)
		{
			doUnityPause(true);
			if (Chartboost.didDisplayMoreApps != null)
			{
				Chartboost.didDisplayMoreApps(CBLocation.locationFromName(location));
			}
		}

		private void didFailToRecordClickEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			CBClickError arg = clickErrorFromInt(hashtable["errorCode"]);
			if (Chartboost.didFailToRecordClick != null)
			{
				Chartboost.didFailToRecordClick(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didFailToLoadRewardedVideoEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			CBImpressionError arg = impressionErrorFromInt(hashtable["errorCode"]);
			if (Chartboost.didFailToLoadRewardedVideo != null)
			{
				Chartboost.didFailToLoadRewardedVideo(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didDismissRewardedVideoEvent(string location)
		{
			doUnityPause(false);
			if (Chartboost.didDismissRewardedVideo != null)
			{
				Chartboost.didDismissRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void didClickRewardedVideoEvent(string location)
		{
			if (Chartboost.didClickRewardedVideo != null)
			{
				Chartboost.didClickRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void didCloseRewardedVideoEvent(string location)
		{
			if (Chartboost.didCloseRewardedVideo != null)
			{
				Chartboost.didCloseRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void didCacheRewardedVideoEvent(string location)
		{
			if (Chartboost.didCacheRewardedVideo != null)
			{
				Chartboost.didCacheRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void shouldDisplayRewardedVideoEvent(string location)
		{
			bool flag = true;
			if (Chartboost.shouldDisplayRewardedVideo != null)
			{
				flag = Chartboost.shouldDisplayRewardedVideo(CBLocation.locationFromName(location));
			}
			CBExternal.chartBoostShouldDisplayRewardedVideoCallbackResult(flag);
			if (flag)
			{
				showRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void didCompleteRewardedVideoEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			int arg;
			try
			{
				arg = Convert.ToInt32(hashtable["reward"]);
			}
			catch
			{
				arg = 0;
			}
			if (Chartboost.didCompleteRewardedVideo != null)
			{
				Chartboost.didCompleteRewardedVideo(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didDisplayRewardedVideoEvent(string location)
		{
			doUnityPause(true);
			if (Chartboost.didDisplayRewardedVideo != null)
			{
				Chartboost.didDisplayRewardedVideo(CBLocation.locationFromName(location));
			}
		}

		private void didCacheInPlayEvent(string location)
		{
			if (Chartboost.didCacheInPlay != null)
			{
				Chartboost.didCacheInPlay(CBLocation.locationFromName(location));
			}
		}

		private void didFailToLoadInPlayEvent(string dataString)
		{
			Hashtable hashtable = (Hashtable)CBJSON.Deserialize(dataString);
			CBImpressionError arg = impressionErrorFromInt(hashtable["errorCode"]);
			if (Chartboost.didFailToLoadInPlay != null)
			{
				Chartboost.didFailToLoadInPlay(CBLocation.locationFromName(hashtable["location"] as string), arg);
			}
		}

		private void didPauseClickForConfirmationEvent()
		{
			if (Chartboost.didPauseClickForConfirmation != null)
			{
				Chartboost.didPauseClickForConfirmation();
			}
		}

		private void willDisplayVideoEvent(string location)
		{
			if (Chartboost.willDisplayVideo != null)
			{
				Chartboost.willDisplayVideo(CBLocation.locationFromName(location));
			}
		}

		private static void doUnityPause(bool pause)
		{
			if (pause && !isPaused)
			{
				lastTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				isPaused = true;
				disableUI(true);
			}
			else if (!pause && isPaused)
			{
				Time.timeScale = lastTimeScale;
				isPaused = false;
				disableUI(false);
			}
		}

		private static void disableUI(bool pause)
		{
		}

		public static bool isImpressionVisible()
		{
			return isPaused;
		}
	}
}
