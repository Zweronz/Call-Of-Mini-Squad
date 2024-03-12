using System.Collections;
using UnityEngine;

namespace CoMDS2
{
	public class SummonDragon : MonoBehaviour
	{
		public GameObject DragonEffect;

		private GameObject m_owner;

		private HitInfo m_hitInfo;

		private float m_summonTime = 5f;

		private float m_damageInterval = 0.5f;

		private void Start()
		{
		}

		private void Update()
		{
			if (null != m_owner)
			{
				base.transform.position = m_owner.transform.position;
			}
		}

		public void Init(GameObject owner, HitInfo hitInfo, float summon_time, float damage_interval)
		{
			m_owner = owner;
			m_hitInfo = hitInfo;
			m_summonTime = summon_time;
			m_damageInterval = damage_interval;
		}

		public void DragonAppear()
		{
			base.transform.position = m_owner.transform.position;
			DragonEffect.SetActive(true);
			Invoke("DragonDisappear", m_summonTime);
			StartCoroutine(DragonDamage());
		}

		public void DragonDisappear()
		{
			DragonEffect.SetActive(false);
			StopCoroutine("DragonDamage");
		}

		private IEnumerator DragonDamage()
		{
			while (DragonEffect.activeSelf)
			{
				if (GameBattle.m_instance != null)
				{
					DS2ActiveObject[] enemy_list = GameBattle.m_instance.GetEnemyList();
					DS2ActiveObject[] array = enemy_list;
					foreach (DS2ActiveObject enemy in array)
					{
						Vector3 e_pos = enemy.GetTransform().position;
						if (Mathf.Abs(e_pos.x - m_owner.transform.position.x) <= (float)Screen.width * 0.5f && Mathf.Abs(e_pos.z - m_owner.transform.position.z) <= (float)Screen.height * 0.5f)
						{
							enemy.OnHit(m_hitInfo);
						}
					}
				}
				yield return new WaitForSeconds(m_damageInterval);
			}
		}
	}
}
