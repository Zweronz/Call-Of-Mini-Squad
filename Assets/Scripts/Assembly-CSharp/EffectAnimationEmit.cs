using UnityEngine;

public class EffectAnimationEmit : MonoBehaviour
{
	private float m_length;

	private bool m_play;

	private float m_time;

	public void Start()
	{
		base.gameObject.GetComponent<Animation>().playAutomatically = false;
		m_length = base.gameObject.GetComponent<Animation>().clip.length;
		base.gameObject.GetComponent<Renderer>().enabled = false;
		m_play = false;
		m_time = 0f;
	}

	public void Emit()
	{
		base.gameObject.GetComponent<Renderer>().enabled = true;
		base.gameObject.GetComponent<Animation>().clip.wrapMode = WrapMode.Once;
		base.gameObject.GetComponent<Animation>().Play();
		m_play = true;
		m_time = 0f;
	}

	public void Update()
	{
		if (m_play)
		{
			m_time += Time.deltaTime;
			if (m_time >= m_length)
			{
				base.gameObject.GetComponent<Animation>().Stop();
				base.gameObject.GetComponent<Renderer>().enabled = false;
				m_play = false;
			}
		}
	}
}
