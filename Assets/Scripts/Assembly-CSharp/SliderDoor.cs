using UnityEngine;

internal class SliderDoor : MonoBehaviour
{
	private enum Phase
	{
		Closed = 0,
		Opening = 1,
		Opened = 2,
		Closing = 3,
		ReadyClosing = 4
	}

	public enum Forward
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	public Forward m_forward;

	public GameObject closedObj;

	private bool m_enable;

	private Phase m_phase;

	private Vector3 m_direction = Vector3.right;

	private Vector3 m_originalPosition;

	private float m_openWidth;

	private float m_openWidthMax = 3f;

	private float m_openedTimer;

	private float m_openedTimeLimit = 3f;

	private bool m_bAutoClose;

	private void Awake()
	{
		switch (m_forward)
		{
		case Forward.Left:
			m_direction = -Vector3.right;
			break;
		case Forward.Right:
			m_direction = Vector3.right;
			break;
		case Forward.Up:
			m_direction = Vector3.forward;
			break;
		case Forward.Down:
			m_direction = -Vector3.forward;
			break;
		}
		m_originalPosition = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
	}

	public void ExecuteEvent(string eventName)
	{
		switch (eventName)
		{
		case "Enter":
			m_enable = true;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
				if (closedObj != null)
				{
					closedObj.SetActive(false);
				}
			}
			if (m_phase == Phase.ReadyClosing)
			{
				m_phase = Phase.Opened;
			}
			else if (m_phase != Phase.Opened)
			{
				m_phase = Phase.Opening;
			}
			m_bAutoClose = false;
			break;
		case "Exit":
			if (m_phase == Phase.Opening)
			{
				m_bAutoClose = true;
			}
			else if (m_phase != 0)
			{
				m_openedTimer = 0f;
				m_phase = Phase.ReadyClosing;
			}
			break;
		case "CloseOff":
			m_enable = false;
			m_phase = Phase.Closing;
			break;
		}
	}

	public void Update()
	{
		switch (m_phase)
		{
		case Phase.Opening:
		{
			float num2 = Time.deltaTime * 10f;
			m_openWidth += num2;
			base.gameObject.transform.Translate(m_direction * num2, Space.World);
			if (m_openWidth >= m_openWidthMax)
			{
				m_phase = Phase.Opened;
				m_openedTimer = 0f;
			}
			break;
		}
		case Phase.Opened:
			if (m_bAutoClose)
			{
				m_openedTimer = 0f;
				m_phase = Phase.ReadyClosing;
			}
			break;
		case Phase.Closing:
		{
			float num = Time.deltaTime * 10f;
			m_openWidth -= num;
			if (m_openWidth <= 0f)
			{
				m_openWidth = 0f;
				m_phase = Phase.Closed;
				base.transform.position = m_originalPosition;
				if (!m_enable && closedObj != null)
				{
					closedObj.SetActive(true);
					base.gameObject.SetActive(false);
				}
			}
			else
			{
				base.gameObject.transform.Translate(m_direction * (0f - num), Space.World);
			}
			break;
		}
		case Phase.Closed:
			break;
		case Phase.ReadyClosing:
			m_openedTimer += Time.deltaTime;
			if (m_openedTimer >= m_openedTimeLimit)
			{
				m_phase = Phase.Closing;
			}
			break;
		}
	}
}
