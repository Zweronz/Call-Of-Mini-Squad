using UnityEngine;

public class CameraViewPortSetByTexture : MonoBehaviour
{
	public Camera gCamera;

	public UITexture gTex;

	private void Awake()
	{
		if (null == gCamera)
		{
			gCamera = base.gameObject.GetComponent<Camera>();
		}
		Transform transform = NGUITools.GetRoot(gTex.gameObject).transform;
		Vector3 vector = transform.InverseTransformPoint(gTex.transform.position);
		float left = ((float)Screen.width / 2f - Mathf.Abs(vector.x)) / (float)Screen.width;
		float top = ((float)Screen.height / 2f - Mathf.Abs(vector.y)) / (float)Screen.height;
		float width = (float)gTex.width / (float)Screen.width;
		float height = (float)gTex.height / (float)Screen.height;
		gCamera.rect = new Rect(left, top, width, height);
	}
}
