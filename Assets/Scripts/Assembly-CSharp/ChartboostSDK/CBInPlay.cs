using System;
using UnityEngine;

namespace ChartboostSDK
{
	public class CBInPlay
	{
		public Texture2D appIcon;

		public string appName;

		private IntPtr inPlayUniqueId;

		private AndroidJavaObject androidInPlayAd;

		public CBInPlay(AndroidJavaObject inPlayAd, AndroidJavaObject plugin)
		{
			androidInPlayAd = inPlayAd;
			appName = androidInPlayAd.Call<string>("getAppName", new object[0]);
			string s = plugin.Call<string>("getBitmapAsString", new object[1] { androidInPlayAd.Call<AndroidJavaObject>("getAppIcon", new object[0]) });
			appIcon = new Texture2D(4, 4);
			appIcon.LoadImage(Convert.FromBase64String(s));
		}

		public void show()
		{
			androidInPlayAd.Call("show");
		}

		public void click()
		{
			androidInPlayAd.Call("click");
		}

		~CBInPlay()
		{
		}
	}
}
