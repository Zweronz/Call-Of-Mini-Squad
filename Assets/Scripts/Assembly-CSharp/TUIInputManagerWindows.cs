using UnityEngine;

internal class TUIInputManagerWindows
{
	private static TUIInput[] m_input = null;

	private static bool m_buttonDown = false;

	private static Vector2 m_lastPosition = new Vector2(0f, 0f);

	public static void UpdateInput()
	{
		if (Input.GetMouseButton(0))
		{
			if (m_buttonDown)
			{
				if (Input.mousePosition.x != m_lastPosition.x || Input.mousePosition.y != m_lastPosition.y)
				{
					m_input = new TUIInput[1];
					m_input[0].fingerId = 0;
					m_input[0].inputType = TUIInputType.Moved;
					m_input[0].position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					m_lastPosition = m_input[0].position;
				}
				else
				{
					m_input = new TUIInput[0];
				}
			}
			else
			{
				m_input = new TUIInput[1];
				m_input[0].fingerId = 0;
				m_input[0].inputType = TUIInputType.Began;
				m_input[0].position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				m_buttonDown = true;
				m_lastPosition = m_input[0].position;
			}
		}
		else if (m_buttonDown)
		{
			m_input = new TUIInput[1];
			m_input[0].fingerId = 0;
			m_input[0].inputType = TUIInputType.Ended;
			m_input[0].position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			m_buttonDown = false;
		}
		else
		{
			m_input = new TUIInput[0];
		}
	}

	public static TUIInput[] GetInput()
	{
		return m_input;
	}
}
