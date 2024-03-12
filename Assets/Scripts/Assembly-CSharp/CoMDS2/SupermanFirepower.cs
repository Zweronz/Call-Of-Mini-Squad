using UnityEngine;

namespace CoMDS2
{
	public class SupermanFirepower : DS2HalfStaticObject
	{
		private enum FirepowerPhase
		{
			Lock = 0,
			Fire = 1
		}

		private Player m_creator;

		private GameObject m_firepowerLock;

		private GameObject m_firepowerBullet;

		private EffectParticleContinuous m_effectFirepowerLock;

		private EffectParticleContinuous m_effectFirepowerBullet;

		private Vector3 m_bulletOriginalLocalPosition;

		private float m_timer;

		public float damageRadius = 2.5f;

		private FirepowerPhase m_phase;

		public HitInfo hitInfo { get; set; }

		public SupermanFirepower(Player creator)
		{
			m_creator = creator;
		}

		public override void Initialize(GameObject gameObject)
		{
			base.Initialize(gameObject);
			m_firepowerLock = GetTransform().Find("Firepower_Regional_Lock").gameObject;
			m_effectFirepowerLock = m_firepowerLock.GetComponent<EffectParticleContinuous>();
			m_firepowerBullet = GetTransform().Find("Firepower_Bullet").gameObject;
			m_effectFirepowerBullet = m_firepowerBullet.GetComponentInChildren<EffectParticleContinuous>();
			m_firepowerBullet.AddComponent<LinearMoveToDestroy>();
			m_bulletOriginalLocalPosition = m_firepowerBullet.transform.localPosition;
			hitInfo = m_creator.skillHitInfo;
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_HIT_5, 1);
		}

		public void SetActive(bool active)
		{
			if (active)
			{
				GetGameObject().SetActive(active);
				m_firepowerBullet.transform.localPosition = m_bulletOriginalLocalPosition;
				StateToLock();
			}
			else
			{
				GetGameObject().SetActive(active);
			}
		}

		public override void Update(float deltaTime)
		{
			switch (m_phase)
			{
			case FirepowerPhase.Lock:
				UpdateStateLock();
				break;
			case FirepowerPhase.Fire:
				UpdateStateFire();
				break;
			}
		}

		private void StateToLock()
		{
			m_firepowerLock.SetActive(true);
			m_firepowerBullet.SetActive(false);
			m_timer = 0f;
			m_phase = FirepowerPhase.Lock;
			if (m_effectFirepowerLock != null)
			{
				m_effectFirepowerLock.StartEmit();
			}
		}

		private void UpdateStateLock()
		{
			m_timer += Time.deltaTime;
			if (m_timer >= 1f)
			{
				m_firepowerLock.SetActive(false);
				StateToFire();
			}
		}

		private void StateToFire()
		{
			m_firepowerBullet.SetActive(true);
			m_phase = FirepowerPhase.Fire;
			LinearMoveToDestroy component = m_firepowerBullet.GetComponent<LinearMoveToDestroy>();
			component.Move(40f, Vector3.up * -1f, 999f);
			m_effectFirepowerBullet.StartEmit();
		}

		private void UpdateStateFire()
		{
			if (m_firepowerBullet.transform.localPosition.y < 0f)
			{
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_HIT_5, GetTransform().position, 3f);
				int layerMask = ((m_creator.clique != DS2ActiveObject.Clique.Computer) ? 2048 : 1536);
				Collider[] array = Physics.OverlapSphere(GetTransform().position, damageRadius, layerMask);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
					hitInfo.repelDirection = @object.GetTransform().position - GetTransform().position;
					@object.OnHit(hitInfo);
				}
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_C);
				}
				Destroy();
			}
		}

		public override void Destroy(bool destroy = false)
		{
			base.Destroy(destroy);
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
			}
			else
			{
				BattleBufferManager.Instance.AddToInteractObjectNeedDeleteListForUIExhibition(this);
			}
			if (m_effectFirepowerLock != null)
			{
				m_effectFirepowerLock.StopEmit();
			}
			if (m_effectFirepowerBullet != null)
			{
				m_effectFirepowerBullet.StopEmit();
			}
		}
	}
}
