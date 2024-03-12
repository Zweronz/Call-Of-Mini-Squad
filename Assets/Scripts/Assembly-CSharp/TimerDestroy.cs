using UnityEngine;

public class TimerDestroy : MonoBehaviour
{
	public bool destroy;

	public float time;

	private bool m_active;

	public void Start()
	{
		if (!m_active)
		{
			TimeDestroy(time, destroy);
		}
	}

	public void TimeDestroy(float time, bool destroy)
	{
		m_active = true;
		this.time = time;
		this.destroy = destroy;
		CancelInvoke();
		Invoke("Destroy", time);
	}

	private void Destroy()
	{
		m_active = false;
		if (destroy)
		{
			Object.Destroy(this);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void Update()
	{
		if (!m_active)
		{
			m_active = true;
			TimeDestroy(time, destroy);
		}
	}
}
