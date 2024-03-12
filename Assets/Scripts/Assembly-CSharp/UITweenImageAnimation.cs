using UnityEngine;

public class UITweenImageAnimation : MonoBehaviour
{
	private UISprite m_sprite;

	public string[] frames;

	public float duration = 1f;

	private float speedPerFrame;

	private float m_timer;

	private int m_frameIndex;

	private void Start()
	{
		if (frames == null || frames.Length == 1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		m_sprite = GetComponent<UISprite>();
		speedPerFrame = duration / (float)frames.Length;
		m_frameIndex = 0;
		m_timer = 0f;
	}

	private void Update()
	{
		m_sprite.spriteName = frames[m_frameIndex];
		m_timer += Time.deltaTime;
		if (m_timer >= speedPerFrame)
		{
			m_timer = 0f;
			m_frameIndex++;
			if (m_frameIndex >= frames.Length)
			{
				m_frameIndex = 0;
			}
		}
	}
}
