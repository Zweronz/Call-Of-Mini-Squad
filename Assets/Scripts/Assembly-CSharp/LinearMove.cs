using UnityEngine;

public class LinearMove : MonoBehaviour
{
	protected float m_speed;

	protected Vector3 m_direction = Vector3.zero;

	public virtual void Move(float speed, Vector3 direction)
	{
		m_speed = speed;
		m_direction = direction.normalized;
	}

	public virtual void Stop()
	{
		m_speed = 0f;
		m_direction = Vector3.zero;
	}

	protected virtual void Update()
	{
		base.transform.Translate(m_direction * m_speed * Time.deltaTime, Space.World);
	}

	private void OnBecameInvisible()
	{
	}

	private void OnBecameVisible()
	{
	}
}
