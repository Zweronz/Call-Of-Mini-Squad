using UnityEngine;

public class AdMobAndroidEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		AdMobAndroidManager.dismissingScreenEvent += dismissingScreenEvent;
		AdMobAndroidManager.failedToReceiveAdEvent += failedToReceiveAdEvent;
		AdMobAndroidManager.leavingApplicationEvent += leavingApplicationEvent;
		AdMobAndroidManager.presentingScreenEvent += presentingScreenEvent;
		AdMobAndroidManager.receivedAdEvent += receivedAdEvent;
		AdMobAndroidManager.interstitialDismissingScreenEvent += interstitialDismissingScreenEvent;
		AdMobAndroidManager.interstitialFailedToReceiveAdEvent += interstitialFailedToReceiveAdEvent;
		AdMobAndroidManager.interstitialLeavingApplicationEvent += interstitialLeavingApplicationEvent;
		AdMobAndroidManager.interstitialPresentingScreenEvent += interstitialPresentingScreenEvent;
		AdMobAndroidManager.interstitialReceivedAdEvent += interstitialReceivedAdEvent;
	}

	private void OnDisable()
	{
		AdMobAndroidManager.dismissingScreenEvent -= dismissingScreenEvent;
		AdMobAndroidManager.failedToReceiveAdEvent -= failedToReceiveAdEvent;
		AdMobAndroidManager.leavingApplicationEvent -= leavingApplicationEvent;
		AdMobAndroidManager.presentingScreenEvent -= presentingScreenEvent;
		AdMobAndroidManager.receivedAdEvent -= receivedAdEvent;
		AdMobAndroidManager.interstitialDismissingScreenEvent -= interstitialDismissingScreenEvent;
		AdMobAndroidManager.interstitialFailedToReceiveAdEvent -= interstitialFailedToReceiveAdEvent;
		AdMobAndroidManager.interstitialLeavingApplicationEvent -= interstitialLeavingApplicationEvent;
		AdMobAndroidManager.interstitialPresentingScreenEvent -= interstitialPresentingScreenEvent;
		AdMobAndroidManager.interstitialReceivedAdEvent -= interstitialReceivedAdEvent;
	}

	private void dismissingScreenEvent()
	{
	}

	private void failedToReceiveAdEvent(string error)
	{
	}

	private void leavingApplicationEvent()
	{
	}

	private void presentingScreenEvent()
	{
	}

	private void receivedAdEvent()
	{
	}

	private void interstitialDismissingScreenEvent()
	{
	}

	private void interstitialFailedToReceiveAdEvent(string error)
	{
	}

	private void interstitialLeavingApplicationEvent()
	{
	}

	private void interstitialPresentingScreenEvent()
	{
	}

	private void interstitialReceivedAdEvent()
	{
	}
}
