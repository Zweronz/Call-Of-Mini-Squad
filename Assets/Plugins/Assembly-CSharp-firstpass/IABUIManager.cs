using Prime31;
using UnityEngine;

public class IABUIManager : MonoBehaviourGUI
{
	private void OnGUI()
	{
		beginColumn();
		if (GUILayout.Button("Initialize IAB"))
		{
			string publicKey = "your public key from the Android developer portal here";
			GoogleIAB.init(publicKey);
		}
		if (GUILayout.Button("Query Inventory"))
		{
			string[] skus = new string[4] { "com.prime31.testproduct", "android.test.purchased", "com.prime31.managedproduct", "com.prime31.testsubscription" };
			GoogleIAB.queryInventory(skus);
		}
		if (GUILayout.Button("Are subscriptions supported?"))
		{
		}
		if (GUILayout.Button("Purchase Test Product"))
		{
			GoogleIAB.purchaseProduct("android.test.purchased");
		}
		if (GUILayout.Button("Consume Test Purchase"))
		{
			GoogleIAB.consumeProduct("android.test.purchased");
		}
		if (GUILayout.Button("Test Unavailable Item"))
		{
			GoogleIAB.purchaseProduct("android.test.item_unavailable");
		}
		endColumn(true);
		if (GUILayout.Button("Purchase Real Product"))
		{
			GoogleIAB.purchaseProduct("com.prime31.testproduct", "payload that gets stored and returned");
		}
		if (GUILayout.Button("Purchase Real Subscription"))
		{
			GoogleIAB.purchaseProduct("com.prime31.testsubscription", "subscription payload");
		}
		if (GUILayout.Button("Consume Real Purchase"))
		{
			GoogleIAB.consumeProduct("com.prime31.testproduct");
		}
		if (GUILayout.Button("Enable High Details Logs"))
		{
			GoogleIAB.enableLogging(true);
		}
		if (GUILayout.Button("Consume Multiple Purchases"))
		{
			string[] skus2 = new string[2] { "com.prime31.testproduct", "android.test.purchased" };
			GoogleIAB.consumeProducts(skus2);
		}
		endColumn();
	}
}
