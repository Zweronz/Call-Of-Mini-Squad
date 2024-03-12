using System;
using CoMDS2;
using UnityEngine;

[AddComponentMenu("NGUI/Extend/Joystick Button")]
public class TUIButtonJoystick : TUIControl
{
	public const int CommandDown = 1;

	public const int CommandMove = 2;

	public const int CommandUp = 3;

	public GameObject m_JoyStickButtonUnpress;

	public GameObject m_JoyStickButtonPress;

	public float m_MinDistance;

	public float m_MaxDistance;

	private float m_Direction;

	private float m_Distance;

	private Vector2 m_beganPos;

	private bool m_bPressed;

	private int m_iFingerId;

	private Vector2 m_touchPos;

	public void Start()
	{
		m_beganPos = Vector2.zero;
		m_bPressed = false;
		m_iFingerId = -1;
		TUIHandleManager component = base.transform.root.gameObject.GetComponent<TUIHandleManager>();
		if (component != null)
		{
			component.AddHandle(this);
		}
		if (m_JoyStickButtonPress != null)
		{
			m_JoyStickButtonPress.SetActive(false);
		}
	}

	public override bool HandleInput(TUIInput input)
	{
		if (Tutorial.Instance.TutorialInProgress && Tutorial.Instance.TutorialPahseFire != Tutorial.TutorialPhaseState.InProgress)
		{
			return false;
		}
		Vector2 vector = new Vector2(input.position.x - (float)Screen.width, input.position.y);
		if ((float)Screen.height > 768f)
		{
			vector.x *= 768f / (float)Screen.height;
			vector.y *= 768f / (float)Screen.height;
		}
		Vector3 position = new Vector3(input.position.x, input.position.y, UICamera.mainCamera.nearClipPlane);
		Vector3 vector2 = UICamera.mainCamera.ScreenToWorldPoint(position);
		input.position.x = vector2.x;
		input.position.y = vector2.y;
		if (input.inputType == TUIInputType.Began)
		{
			m_beganPos = input.position;
			if (PtInControl(input.position))
			{
				if (Tutorial.Instance.TutorialPahseFire == Tutorial.TutorialPhaseState.InProgress)
				{
					Tutorial.Instance.TutorialPahseFire = Tutorial.TutorialPhaseState.Done;
				}
				m_JoyStickButtonUnpress.SetActive(false);
				m_JoyStickButtonPress.SetActive(true);
				m_bPressed = true;
				m_iFingerId = input.fingerId;
				float num = vector.x - base.transform.localPosition.x;
				float num2 = vector.y - base.transform.localPosition.y;
				m_Direction = ((!(num2 >= 0f)) ? (Mathf.Atan2(num2, num) + (float)Math.PI * 2f) : Mathf.Atan2(num2, num));
				m_Distance = Mathf.Sqrt(num * num + num2 * num2);
				if (m_Distance > m_MaxDistance)
				{
					m_Distance = m_MaxDistance;
				}
				float wparam = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
				Show();
				PostEvent(this, 1, wparam, m_Direction, null);
				return true;
			}
		}
		else
		{
			if (m_beganPos.x > 0f && PtInControl(input.position) && !m_bPressed)
			{
				m_JoyStickButtonUnpress.SetActive(false);
				m_JoyStickButtonPress.SetActive(true);
				m_bPressed = true;
				m_iFingerId = input.fingerId;
			}
			if (input.fingerId == m_iFingerId)
			{
				if (input.inputType == TUIInputType.Moved)
				{
					float num3 = vector.x - base.transform.localPosition.x;
					float num4 = vector.y - base.transform.localPosition.y;
					m_Direction = ((!(num4 >= 0f)) ? (Mathf.Atan2(num4, num3) + (float)Math.PI * 2f) : Mathf.Atan2(num4, num3));
					m_Distance = Mathf.Sqrt(num3 * num3 + num4 * num4);
					if (m_Distance > m_MaxDistance)
					{
						m_Distance = m_MaxDistance;
					}
					float wparam2 = (m_Distance - m_MinDistance) / (m_MaxDistance - m_MinDistance);
					Show();
					PostEvent(this, 2, wparam2, m_Direction, null);
					return true;
				}
				if (input.inputType == TUIInputType.Ended)
				{
					m_JoyStickButtonUnpress.SetActive(true);
					m_JoyStickButtonPress.SetActive(false);
					m_bPressed = false;
					m_beganPos = Vector2.zero;
					m_iFingerId = -1;
					m_Direction = 0f;
					m_Distance = 0f;
					Show();
					PostEvent(this, 3, 0f, 0f, null);
					return true;
				}
			}
		}
		return false;
	}

	public void Show()
	{
		if (null != m_JoyStickButtonPress)
		{
			Vector3 localPosition = m_JoyStickButtonPress.transform.localPosition;
			localPosition = new Vector3(m_Distance * Mathf.Cos(m_Direction), m_Distance * Mathf.Sin(m_Direction), localPosition.z);
			m_JoyStickButtonPress.transform.localPosition = localPosition;
		}
	}
}
