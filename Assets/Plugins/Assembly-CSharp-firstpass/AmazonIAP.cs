using System;
using UnityEngine;

public class AmazonIAP
{
	private static AndroidJavaObject _plugin;

	static AmazonIAP()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.amazon.AmazonIAPPlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void initiateItemDataRequest(string[] items)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			IntPtr methodID = AndroidJNI.GetMethodID(_plugin.GetRawClass(), "initiateItemDataRequest", "([Ljava/lang/String;)V");
			AndroidJNI.CallVoidMethod(_plugin.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(new object[1] { items }));
		}
	}

	public static void initiatePurchaseRequest(string sku)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("initiatePurchaseRequest", sku);
		}
	}

	public static void initiateGetUserIdRequest()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("initiateGetUserIdRequest");
		}
	}
}
