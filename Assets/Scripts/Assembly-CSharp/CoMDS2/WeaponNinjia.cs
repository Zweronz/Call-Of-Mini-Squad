using UnityEngine;

namespace CoMDS2
{
	public class WeaponNinjia : Weapon
	{
		private PlayerNinjia.NinjiaType m_ninjiaType;

		private AttackCollider m_attackCollider;

		public override Character owner
		{
			get
			{
				return base.owner;
			}
			set
			{
				base.owner = value;
				if (owner != null)
				{
					for (int i = 0; i < m_effectFire.Length; i++)
					{
						m_effectFire[i].Root.transform.parent = owner.GetTransform();
						m_effectFire[i].Root.transform.localRotation = Quaternion.identity;
						m_effectFire[i].Root.transform.localPosition = Vector3.zero;
					}
				}
			}
		}

		public WeaponNinjia(WeaponType weaponType, PlayerNinjia.NinjiaType ninjiaType)
			: base(weaponType)
		{
			m_ninjiaType = ninjiaType;
		}

		public override void Initialize(int level)
		{
			base.Initialize(level);
			if (GameBattle.m_instance != null)
			{
				m_attackCollider = m_gameObject.GetComponentInChildren<AttackCollider>();
			}
		}

		public override void Mount(Transform character, Transform weaponPoint, Transform fireLightPoint)
		{
			base.Mount(character, weaponPoint, fireLightPoint);
			if (m_attackCollider != null)
			{
				m_attackCollider.belong = owner.GetGameObject();
				m_attackCollider.gameObject.SetActive(false);
				owner.AddAttackCollider(m_attackCollider);
			}
		}

		public override void Unmount()
		{
			base.Unmount();
			if (m_attackCollider != null)
			{
				owner.RemoveAttackCollider(m_attackCollider);
			}
		}

		public override void Update(bool fire)
		{
			base.Update(fire);
		}

		public override void DoReload()
		{
			base.DoReload();
		}

		public override bool NeedReload()
		{
			return false;
		}

		public override void UpdateFire(float deltaTime)
		{
			float shootRange = owner.shootRange;
			m_fireTimer = 0f;
			m_bulletRotation = owner.GetModelTransform().rotation;
			EmitBullet(shootRange);
		}

		public override void EmitBullet(float distanceLife = 0f)
		{
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			if (attribute.hitType == HIT_TYPE.TRAJECTORY)
			{
				bulletFromBuffer.GetGameObject().layer = 12;
			}
			else if (owner.clique == DS2ActiveObject.Clique.Computer)
			{
				bulletFromBuffer.GetGameObject().layer = 23;
			}
			else
			{
				Player player = (Player)owner;
				if (player.isAlly)
				{
					bulletFromBuffer.GetGameObject().layer = 22;
				}
				else
				{
					bulletFromBuffer.GetGameObject().layer = 21;
				}
			}
			if (bulletFromBuffer != null)
			{
				bulletFromBuffer.hitInfo = owner.GetHitInfo();
				bulletFromBuffer.SetBullet(owner, bulletAttribute, new Vector3(m_firePoint.position.x, m_firePoint.position.y, m_firePoint.position.z), m_bulletRotation);
				bulletFromBuffer.Emit(distanceLife);
			}
		}

		public override void EffectFireStart(int effectIndex, bool playAudio = true)
		{
			if (effectIndex >= 0 && effectIndex < m_effectFire.Length && m_effectFire[effectIndex] != null)
			{
				m_effectFire[effectIndex].Root.SetActive(true);
				m_effectFire[effectIndex].StartEmit(playAudio);
			}
		}
	}
}
