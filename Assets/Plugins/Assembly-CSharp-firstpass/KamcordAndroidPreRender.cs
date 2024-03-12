using UnityEngine;

public class KamcordAndroidPreRender : MonoBehaviour
{
	private void OnPreRender()
	{
		Kamcord.BeginDraw();
	}
}
