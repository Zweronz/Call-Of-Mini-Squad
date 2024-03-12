using System;
using UnityEngine;

public class CameraHome : MonoBehaviour
{
	private static CameraHome m_instance;

	private float m_cameraPitch;

	private Vector2 m_cameraSize;

	private Vector3 m_viewCenter;

	private float m_viewRotation;

	private Vector2 m_viewSize;

	public static CameraHome Instance()
	{
		return m_instance;
	}

	public void Awake()
	{
		m_instance = this;
		base.GetComponent<Camera>().orthographic = true;
		base.GetComponent<Camera>().aspect = (float)Screen.width / (float)Screen.height;
		m_cameraPitch = (float)Math.PI / 3f;
		m_cameraSize = new Vector2(16f, 16f * (float)Screen.height / (float)Screen.width);
		m_viewCenter = new Vector3(0f, 0f, 0f);
		m_viewRotation = 0f;
		m_viewSize = new Vector2(m_cameraSize.x, m_cameraSize.y / Mathf.Sin(m_cameraPitch));
		UpdateCamera();
	}

	public void Move1(Vector2 fingerPosition1, Vector3 touchPosition1)
	{
		float num = (fingerPosition1.x / (float)Screen.width - 0.5f) * m_viewSize.x;
		float num2 = (fingerPosition1.y / (float)Screen.height - 0.5f) * m_viewSize.y;
		float num3 = Mathf.Cos(m_viewRotation);
		float num4 = Mathf.Sin(m_viewRotation);
		Vector3 vector = new Vector3(num3, 0f, num4);
		Vector3 vector2 = new Vector3(0f - num4, 0f, num3);
		m_viewCenter = touchPosition1 - (vector * num + vector2 * num2);
		UpdateCamera();
	}

	public void Move2(Vector2 fingerPosition1, Vector3 touchPosition1, Vector2 fingerPosition2, Vector3 touchPosition2)
	{
		float num = (float)Screen.height / (float)Screen.width / Mathf.Sin(m_cameraPitch);
		float num2 = fingerPosition1.x / (float)Screen.width - 0.5f - (fingerPosition2.x / (float)Screen.width - 0.5f);
		float num3 = fingerPosition1.y / (float)Screen.height - 0.5f - (fingerPosition2.y / (float)Screen.height - 0.5f) * num;
		float num4 = touchPosition1.x - touchPosition2.x;
		float num5 = touchPosition1.z - touchPosition2.z;
		float num6 = num2;
		float num7 = 0f - num3;
		float num8 = num4;
		float num9 = num3;
		float num10 = num2;
		float num11 = num5;
		float num12 = num6 * num10 - num9 * num7;
		if (!((double)Mathf.Abs(num6 * num10 - num9 * num7) < 0.0001))
		{
			float num13 = num10 * num8 - num7 * num11;
			float num14 = num6 * num11 - num9 * num8;
			m_viewRotation = Mathf.Atan2(num14, num13);
			float num15 = Mathf.Cos(m_viewRotation);
			float num16 = Mathf.Sin(m_viewRotation);
			Vector3 vector = new Vector3(num15, 0f, num16);
			Vector3 vector2 = new Vector3(0f - num16, 0f, num15);
			float num17 = 0f;
			num17 = ((!(Mathf.Abs(num15) >= Mathf.Abs(num16))) ? (num14 / num12 / num16) : (num13 / num12 / num15));
			m_cameraSize = new Vector2(num17, num17 * (float)Screen.height / (float)Screen.width);
			m_viewSize = new Vector2(num17, num * num17);
			float num18 = (fingerPosition1.x / (float)Screen.width - 0.5f) * m_viewSize.x;
			float num19 = (fingerPosition1.y / (float)Screen.height - 0.5f) * m_viewSize.y;
			m_viewCenter = touchPosition1 - (vector * num18 + vector2 * num19);
			UpdateCamera();
		}
	}

	private void UpdateCamera()
	{
		float y = Mathf.Sin(m_cameraPitch) * 20f;
		float num = Mathf.Cos(m_cameraPitch) * 20f;
		float z = Mathf.Cos(m_viewRotation);
		float num2 = Mathf.Sin(m_viewRotation);
		Vector3 vector = new Vector3(0f - num2, 0f, z);
		base.GetComponent<Camera>().transform.position = m_viewCenter + -vector * num + new Vector3(0f, y, 0f);
		base.GetComponent<Camera>().transform.LookAt(m_viewCenter, vector);
		base.GetComponent<Camera>().orthographicSize = m_cameraSize.y / 2f;
	}
}
