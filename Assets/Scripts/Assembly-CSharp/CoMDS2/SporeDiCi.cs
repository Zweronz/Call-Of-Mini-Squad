using UnityEngine;

namespace CoMDS2
{
	public class SporeDiCi : DS2ActiveObject
	{
		private float m_appearTime;

		private float m_lifeTime;

		private bool m_appeared;

		private float m_diciTime;

		private Transform m_attackIndicate;

		private UnityEngine.AI.NavMeshObstacle m_navMeshObstacle;

		private CapsuleCollider m_capsuleCollider;

		private AttackColliderSimple m_attackCollider;

		public SporeDiCi()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			base.clique = Clique.Computer;
			Transform transform = GetTransform();
			m_attackIndicate = transform.Find("AttackIndicate");
			m_navMeshObstacle = transform.GetComponent<UnityEngine.AI.NavMeshObstacle>();
			m_capsuleCollider = transform.GetComponent<CapsuleCollider>();
			m_attackCollider = transform.GetComponentInChildren<AttackColliderSimple>();
			m_attackCollider.gameObject.SetActive(false);
			m_attackCollider.belong = GetGameObject();
		}

		public void DiCiAppear(HitInfo hitInfo, float appear_time, float life_time)
		{
			m_appearTime = appear_time;
			m_lifeTime = life_time;
			m_diciTime = 0f;
			m_appeared = false;
			GetGameObject().SetActive(true);
			m_attackIndicate.gameObject.SetActive(true);
			m_navMeshObstacle.enabled = false;
			m_capsuleCollider.enabled = false;
			m_attackCollider.hitInfo = new HitInfo(hitInfo);
			m_attackCollider.gameObject.SetActive(false);
			GameBattle.m_instance.AddObjToInteractObjectList(this);
		}

		public override void Update(float deltaTime)
		{
			if (!GetGameObject().activeInHierarchy)
			{
				return;
			}
			m_diciTime += deltaTime;
			if (!m_appeared)
			{
				if (m_diciTime > m_appearTime)
				{
					m_appeared = true;
					m_attackIndicate.gameObject.SetActive(false);
					effectPlayManager.PlayEffect("Attack");
					m_diciTime = 0f;
					m_navMeshObstacle.enabled = true;
					m_capsuleCollider.enabled = true;
					m_attackCollider.enabled = true;
					m_attackCollider.gameObject.SetActive(true);
				}
			}
			else if (m_diciTime > m_lifeTime)
			{
				effectPlayManager.StopEffect("Attack");
				GetGameObject().SetActive(false);
				GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
			}
		}
	}
}
