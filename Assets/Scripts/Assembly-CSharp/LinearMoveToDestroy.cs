using CoMDS2;
using UnityEngine;

public class LinearMoveToDestroy : LinearMove
{
	private float m_distance;

	private float m_value;

	private bool m_destroy;

	public void Move(float speed, Vector3 direction, float lifeDistance = 0f, bool destroy = false)
	{
		base.Move(speed, direction);
		m_distance = lifeDistance;
		m_destroy = destroy;
		m_value = 0f;
	}

	protected void Destroy()
	{
		DS2Object @object = DS2ObjectStub.GetObject<DS2Object>(base.gameObject);
		@object.Destroy(m_destroy);
	}

	protected override void Update()
	{
		float num = m_speed * Time.deltaTime;
		base.transform.Translate(m_direction * num, Space.World);
		m_value += num;
		if (m_distance > 0f && m_value > m_distance)
		{
			m_value = 0f;
			Destroy();
		}
	}
}
