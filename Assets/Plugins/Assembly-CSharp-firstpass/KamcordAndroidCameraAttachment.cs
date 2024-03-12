using UnityEngine;

public class KamcordAndroidCameraAttachment : MonoBehaviour
{
	private void Awake()
	{
		KamcordImplementationAndroid.SetRenderCameraEnabled("Pre", false);
		KamcordImplementationAndroid.SetRenderCameraEnabled("Post", false);
	}

	private void OnDestroy()
	{
		KamcordImplementationAndroid.SetRenderCameraEnabled("Pre", true);
		KamcordImplementationAndroid.SetRenderCameraEnabled("Post", true);
	}

	private void OnPreRender()
	{
		Kamcord.BeginDraw();
	}

	private void OnPostRender()
	{
		Kamcord.EndDraw();
	}
}
