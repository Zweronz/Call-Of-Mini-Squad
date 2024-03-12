using CoMDS2;
using UnityEngine;

internal class DS2ObjectDestroy : MonoBehaviour
{
	private bool m_destroy = true;

	private bool m_active = true;

	private float m_time = 1f;

	public void TimeDestroy(float time, bool destroy)
	{
		m_active = true;
		m_time = time;
		m_destroy = destroy;
		CancelInvoke();
		Invoke("Destroy", time);
	}

	public void TimeDestroy()
	{
		m_active = true;
		CancelInvoke();
		Invoke("Destroy", m_time);
	}

	private void Destroy()
	{
		CancelInvoke("Destroy");
		m_active = false;
		DS2Object @object = DS2ObjectStub.GetObject<DS2Object>(base.gameObject);
		@object.Destroy(m_destroy);
	}

	public void Destroy(DS2Object obj)
	{
		m_active = false;
		CancelInvoke("Destroy");
		obj.Destroy();
	}

	public void Update()
	{
		if (!m_active)
		{
			m_active = true;
			TimeDestroy(m_time, m_destroy);
		}
	}
}
