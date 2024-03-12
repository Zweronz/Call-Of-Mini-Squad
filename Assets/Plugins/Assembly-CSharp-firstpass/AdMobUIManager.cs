using Prime31;
using UnityEngine;

public class AdMobUIManager : MonoBehaviourGUI
{
	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Init"))
		{
			AdMobAndroid.init("a152ce99288f0ac");
		}
		if (GUILayout.Button("Set Test Devices"))
		{
			AdMobAndroid.setTestDevices(new string[1] { SystemInfo.deviceUniqueIdentifier });
		}
		if (GUILayout.Button("Create Smart Banner"))
		{
			AdMobAndroid.createBanner(AdMobAndroidAd.smartBanner, AdMobAdPlacement.BottomCenter);
		}
		if (GUILayout.Button("Create 320x50 banner"))
		{
			AdMobAndroid.createBanner("ca-app-pub-8386987260001674/8398905145", AdMobAndroidAd.phone320x50, AdMobAdPlacement.TopCenter);
		}
		if (GUILayout.Button("Create 300x250 banner"))
		{
			AdMobAndroid.createBanner(AdMobAndroidAd.tablet300x250, AdMobAdPlacement.BottomCenter);
		}
		if (GUILayout.Button("Destroy Banner"))
		{
			AdMobAndroid.destroyBanner();
		}
		endColumn(true);
		if (GUILayout.Button("Refresh Ad"))
		{
			AdMobAndroid.refreshAd();
		}
		if (GUILayout.Button("Request Interstitial"))
		{
			AdMobAndroid.requestInterstital("ca-app-pub-8386987260001674/9875638345");
		}
		if (GUILayout.Button("Is Interstitial Ready?"))
		{
			bool flag = AdMobAndroid.isInterstitalReady();
		}
		if (GUILayout.Button("Display Interstitial"))
		{
			AdMobAndroid.displayInterstital();
		}
		if (GUILayout.Button("Hide Banner"))
		{
			AdMobAndroid.hideBanner(true);
		}
		if (GUILayout.Button("Show Banner"))
		{
			AdMobAndroid.hideBanner(false);
		}
		endColumn();
	}
}
