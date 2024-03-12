using UnityEngine;

public class ShowFPS1 : MonoBehaviour
{
	private float m_fps;

	private float m_minFps = 999.9f;

	private float m_maxFps;

	public void Awake()
	{
		m_fps = 0f;
	}

	public void Update()
	{
		float num = 1f / Time.deltaTime;
		m_fps = m_fps * 0.4f + num * 0.6f;
		if (m_minFps > m_fps)
		{
			m_minFps = m_fps;
		}
		if (m_maxFps < m_fps)
		{
			m_maxFps = m_fps;
		}
	}

	public void OnGUI()
	{
		GUI.Label(new Rect(10f, 5f, 60f, 30f), "fps:" + (int)m_fps);
		GUI.Label(new Rect(70f, 5f, 80f, 30f), "fps min:" + (int)m_minFps);
		GUI.Label(new Rect(160f, 5f, 80f, 30f), "fps max:" + (int)m_maxFps);
	}
}
