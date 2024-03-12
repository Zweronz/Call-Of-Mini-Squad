using CoMDS2;
using UnityEngine;

public class HomingMoveToDestroy : LinearMove
{
	private float m_distance;

	private float m_value;

	private bool m_destroy;

	private float m_homing_interval;

	private float m_homing_life;

	private float m_homing_value;

	private GameObject m_homing_target;

	private Quaternion m_start_rotation = Quaternion.identity;

	private Quaternion m_target_rotation = Quaternion.identity;

	private float m_rotate_time;

	private float m_start_homing_time = 0.5f;

	public void Move(float speed, Vector3 direction, GameObject target, float homing_interval, float homing_life, float lifeDistance = 0f, bool destroy = false)
	{
		m_homing_target = target;
		if (null != target && target.activeSelf)
		{
			Vector3 vector = new Vector3(target.transform.position.x, base.transform.position.y, target.transform.position.z);
			Vector3 forward = vector - base.transform.position;
			m_start_rotation = base.transform.rotation;
			m_target_rotation = Quaternion.LookRotation(forward);
			m_direction = base.transform.forward;
			m_rotate_time = 0f;
		}
		else
		{
			m_direction = direction;
		}
		base.Move(speed, direction);
		m_distance = lifeDistance;
		m_destroy = destroy;
		m_value = 0f;
		m_homing_interval = homing_interval;
		m_homing_life = homing_life;
		m_homing_value = homing_interval;
		m_start_homing_time = 0.3f;
	}

	protected void Destroy()
	{
		DS2Object @object = DS2ObjectStub.GetObject<DS2Object>(base.gameObject);
		@object.Destroy(m_destroy);
	}

	protected override void Update()
	{
		m_start_homing_time -= Time.deltaTime;
		if (m_start_homing_time <= 0f)
		{
			m_homing_life -= Time.deltaTime;
			if (m_homing_life > 0f && null != m_homing_target && m_homing_target.activeSelf)
			{
				m_homing_value -= Time.deltaTime;
				if (m_homing_value <= 0f)
				{
					Vector3 vector = new Vector3(m_homing_target.transform.position.x, base.transform.position.y, m_homing_target.transform.position.z);
					m_homing_value = m_homing_interval;
					Vector3 forward = vector - base.transform.position;
					m_start_rotation = base.transform.rotation;
					m_target_rotation = Quaternion.LookRotation(forward);
					m_rotate_time = 0f;
				}
				base.transform.rotation = Quaternion.Lerp(m_start_rotation, m_target_rotation, m_rotate_time / 0.3f);
				m_rotate_time += Time.deltaTime;
				m_direction = base.transform.forward;
			}
		}
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
