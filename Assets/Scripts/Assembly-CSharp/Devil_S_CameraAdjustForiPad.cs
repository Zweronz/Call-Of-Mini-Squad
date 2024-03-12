using UnityEngine;

public class Devil_S_CameraAdjustForiPad : MonoBehaviour
{
	public bool bFullScreen;

	private void Awake()
	{
		if (!bFullScreen)
		{
			Camera camera = base.GetComponent<Camera>();
			float num = ((!(camera.pixelRect.width > 960f)) ? camera.pixelRect.width : 960f);
			float num2 = ((!(camera.pixelRect.height > 640f)) ? camera.pixelRect.height : 640f);
			if (Screen.width >= 1024 && Screen.height >= 768)
			{
				camera.pixelRect = new Rect(((float)Screen.width - num) * 0.5f, ((float)Screen.height - num2) * 0.5f, num, num2);
			}
		}
	}
}
