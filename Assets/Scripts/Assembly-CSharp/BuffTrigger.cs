using System.Collections.Generic;
using CoMDS2;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BuffTrigger : MonoBehaviour
{
	public Buff.AffectType affectType;

	public float value;

	public float extraValue;

	public float interal;

	public float time;

	public Buff.CalcType calcType;

	private float m_timer;

	private new SphereCollider collider;

	private Dictionary<Character, Buff> m_container;

	public float TriggerDistance
	{
		set
		{
			collider.radius = value;
		}
	}

	private void Awake()
	{
		collider = base.gameObject.GetComponent<SphereCollider>();
	}

	private void Start()
	{
		m_timer = 0f;
		m_container = new Dictionary<Character, Buff>();
		collider.isTrigger = true;
		collider.center = Vector3.zero;
	}

	private void Update()
	{
		if (time != -1f)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= time)
			{
				m_timer = 0f;
				base.gameObject.SetActive(false);
			}
		}
	}

	public void SetTrigger(Buff.AffectType affectType, float value, float interal, float time, Buff.CalcType calcType)
	{
		this.affectType = affectType;
		this.value = value;
		this.interal = interal;
		this.time = time;
		this.calcType = calcType;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 18)
		{
			return;
		}
		Character @object = DS2ObjectStub.GetObject<Character>(other.GetComponent<Collider>().gameObject);
		if (!m_container.ContainsKey(@object))
		{
			IBuffManager buffManager = @object.GetBuffManager();
			if (buffManager != null)
			{
				Buff buff = new Buff(affectType, value, interal, time, calcType, 0f);
				m_container.Add(@object, buff);
				buffManager.AddBuff(buff);
			}
		}
	}

	public void OnTriggerStay(Collider other)
	{
	}

	public void OnTriggerExit(Collider other)
	{
		Character @object = DS2ObjectStub.GetObject<Character>(other.GetComponent<Collider>().gameObject);
		if (m_container.ContainsKey(@object))
		{
			IBuffManager buffManager = @object.GetBuffManager();
			buffManager.RemoveBuff(m_container[@object].affectType);
			m_container.Remove(@object);
		}
	}

	public void OnDisable()
	{
		if (m_container == null || m_container.Count == 0)
		{
			return;
		}
		foreach (Character key in m_container.Keys)
		{
			IBuffManager buffManager = key.GetBuffManager();
			buffManager.RemoveBuff(m_container[key].affectType);
		}
		m_container.Clear();
	}
}
