using System;
using CoMDS2;
using UnityEngine;

[AddComponentMenu("NGUI/Extend/Joystick Button Ex")]
public class TUIButtonJoystickEx : TUIControl
{
	public const int CommandDown = 1;

	public const int CommandMove = 2;

	public const int CommandUp = 3;

	public GameObject m_JoyStickButtonUnpress;

	public GameObject m_JoyStickButtonPress;

	public GameObject m_JoyStickBk;

	public float m_MinDistance;

	public float m_MaxDistance;

	private float m_Direction;

	private float m_Distance;

	private Vector3 m_originalPos = Vector3.zero;

	private Vector2 m_beganPos;

	private bool m_bPressed;

	private int m_iFingerId;

	public void Start()
	{
		m_beganPos = Vector2.zero;
		m_bPressed = false;
		m_iFingerId = -1;
		m_originalPos = base.transform.localPosition;
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
		if (Tutorial.Instance.TutorialInProgress && Tutorial.Instance.TutorialPahseMove != Tutorial.TutorialPhaseState.InProgress)
		{
			return false;
		}
		Vector2 position = input.position;
		Vector2 position2 = input.position;
		Vector3 position3 = new Vector3(input.position.x, input.position.y, UICamera.mainCamera.nearClipPlane);
		Vector3 vector = UICamera.mainCamera.ScreenToWorldPoint(position3);
		input.position.x = vector.x;
		input.position.y = vector.y;
		if ((float)Screen.height > 768f)
		{
			position.x *= 768f / (float)Screen.height;
			position.y *= 768f / (float)Screen.height;
		}
		if (input.inputType == TUIInputType.Began)
		{
			m_beganPos = input.position;
			if (PtInControl(input.position) || position2.x < (float)(Screen.width >> 1))
			{
				if (!PtInControl(input.position))
				{
					if (Tutorial.Instance.TutorialPahseMove == Tutorial.TutorialPhaseState.InProgress)
					{
						return false;
					}
					if (m_JoyStickBk != null)
					{
						m_JoyStickBk.SetActive(false);
					}
					m_bPressed = true;
					base.transform.localPosition = position;
				}
				else if (Tutorial.Instance.TutorialPahseMove == Tutorial.TutorialPhaseState.InProgress)
				{
					Tutorial.Instance.TutorialPahseMove = Tutorial.TutorialPhaseState.Done;
				}
				if (m_JoyStickButtonPress != null)
				{
					m_JoyStickButtonUnpress.SetActive(false);
					m_JoyStickButtonPress.SetActive(true);
				}
				m_iFingerId = input.fingerId;
				float num = position.x - base.transform.localPosition.x;
				float num2 = position.y - base.transform.localPosition.y;
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
			if (m_beganPos.x > 0f && PtInControl(input.position))
			{
				if (!m_bPressed)
				{
					m_bPressed = true;
					m_iFingerId = input.fingerId;
				}
			}
			else if (Tutorial.Instance.TutorialPahseMove == Tutorial.TutorialPhaseState.InProgress)
			{
				return false;
			}
			if (input.fingerId == m_iFingerId)
			{
				if (input.inputType == TUIInputType.Moved)
				{
					float num3 = position.x - base.transform.localPosition.x;
					float num4 = position.y - base.transform.localPosition.y;
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
					if (m_bPressed)
					{
						base.transform.localPosition = m_originalPos;
						m_bPressed = false;
					}
					m_JoyStickButtonUnpress.SetActive(true);
					m_JoyStickButtonPress.SetActive(false);
					if (m_JoyStickBk != null)
					{
						m_JoyStickBk.SetActive(true);
					}
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
