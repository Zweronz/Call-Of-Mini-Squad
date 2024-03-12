using UnityEngine;

namespace CoMDS2
{
	public class BulletBlackholeLogic : MonoBehaviour
	{
		private bool m_hitStage;

		private float m_life;

		private float m_dealDmgDelta = 1f;

		private Bullet m_bullet;

		public DS2ActiveObject.Clique clique;

		public float radius = 3f;

		public float life = 6f;

		public float damageInterval = 0.5f;

		private void Start()
		{
			m_bullet = DS2ObjectStub.GetObject<Bullet>(base.gameObject);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.BlackholeEnd, 1);
		}

		private void OnEnable()
		{
			m_hitStage = false;
			m_life = 0f;
			m_dealDmgDelta = 1f;
		}

		public void OnTriggerEnter(Collider other)
		{
			if ((other.gameObject.layer == 18 || other.gameObject.layer == 16) && !m_hitStage)
			{
				base.gameObject.GetComponent<LinearMoveToDestroy>().Move(0f, Vector3.zero);
			}
		}

		private void Update()
		{
			m_life += Time.deltaTime;
			if (m_life >= life)
			{
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.BlackholeEnd, base.transform.position, 1f);
				m_bullet.Destroy();
				return;
			}
			m_dealDmgDelta += Time.deltaTime;
			if (!(m_dealDmgDelta >= damageInterval))
			{
				return;
			}
			m_dealDmgDelta = 0f;
			int layerMask = ((clique != 0) ? 1536 : 526336);
			Collider[] array = Physics.OverlapSphere(base.transform.position, radius, layerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
				if (@object == null)
				{
				}
				@object.OnHit(m_bullet.hitInfo);
			}
		}
	}
}
