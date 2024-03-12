using UnityEngine;

public class AmazonIAPUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float width = ((Screen.width < 800 && Screen.height < 800) ? 160 : 320);
		float num2 = ((Screen.width < 800 && Screen.height < 800) ? 40 : 80);
		float num3 = num2 + 10f;
		if (GUI.Button(new Rect(left, num, width, num2), "Initiate Item Data Request"))
		{
			AmazonIAP.initiateItemDataRequest(new string[3] { "coinpack.tier.2", "coinpack.tier.5", "coinpack.tier.10" });
		}
		if (GUI.Button(new Rect(left, num += num3, width, num2), "Initiate Purchase Request"))
		{
			AmazonIAP.initiatePurchaseRequest("coinpack.tier.2");
		}
		if (GUI.Button(new Rect(left, num += num3, width, num2), "Initiate Get User ID Request"))
		{
			AmazonIAP.initiateGetUserIdRequest();
		}
	}
}
