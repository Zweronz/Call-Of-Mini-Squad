using UnityEngine;

namespace CoMDS2
{
	public class WeaponPistolSingle : Weapon
	{
		protected WeaponPistol.PistolType pistolType;

		protected ushort m_bulletEmitOnTime = 2;

		public WeaponPistolSingle(WeaponType weaponType, WeaponPistol.PistolType pistolType)
			: base(weaponType)
		{
			this.pistolType = pistolType;
			if (m_effectLight != null)
			{
				m_effectLight.gameObject.GetComponent<Animation>()["Anim_Light"].speed *= 10f;
			}
		}

		public void SetBulletEmitOnTime(ushort count)
		{
			m_bulletEmitOnTime = count;
		}

		public override void UpdateFire(float deltaTime)
		{
			m_iBulletCombo++;
			EffectFireStart();
			EffectCartridgeEmit();
			PlayEffectLight();
			float num = owner.shootRange;
			m_fireTimer = 0f;
			m_time = 0f;
			int layermask = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 67962880 : 67962368);
			int num2 = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 526336 : 1536);
			if (owner.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && owner.clique == DS2ActiveObject.Clique.Player)
			{
				Player player = (Player)owner;
				if (!player.CurrentController)
				{
					layermask = 329728;
					num2 = 2048;
				}
			}
			if (pistolType == WeaponPistol.PistolType.Scatter)
			{
				for (int i = 0; i < m_bulletEmitOnTime; i++)
				{
					m_bulletTransCopy.transform.rotation = owner.GetModelTransform().rotation;
					if (m_bulletEmitOnTime != 1)
					{
						if (i == 0)
						{
							m_bulletTransCopy.transform.Rotate(Vector3.up, -15f);
						}
						else
						{
							m_bulletTransCopy.transform.Rotate(Vector3.up, 15f);
						}
					}
					m_bulletRotation = m_bulletTransCopy.transform.rotation;
					RaycastHit[] array = Physics.RaycastAll(m_weaponBonePoint.position, m_bulletTransCopy.transform.forward, num, layermask);
					if (array != null && array.Length > 0)
					{
						RaycastHit firstHit;
						Util.RaycastHitFirstTarget(array, m_weaponBonePoint.position, out firstHit);
						float magnitude = (firstHit.point - owner.GetTransform().position).magnitude;
						if (((1 << firstHit.collider.gameObject.layer) & num2) != 0)
						{
							DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(firstHit.collider.gameObject);
							if (!@object.Alive())
							{
								EmitBullet(magnitude);
								break;
							}
							HitInfo hitInfo = owner.GetHitInfo();
							hitInfo.hitPoint = firstHit.point;
							hitInfo.repelDirection = owner.GetModelTransform().forward;
							float num3 = (attribute.attackRange - magnitude) / attribute.attackRange;
							hitInfo.repelDistance = new NumberSection<float>(hitInfo.repelDistance.left * num3, hitInfo.repelDistance.right * num3);
							@object.OnHit(hitInfo);
							BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit.point, 0.3f);
						}
						else
						{
							BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit.point, 0.3f, null, false);
						}
						num = magnitude;
					}
					EmitBullet(num);
				}
				return;
			}
			m_bulletTransCopy.transform.rotation = owner.GetModelTransform().rotation;
			if (attribute.deviationStart > 0 && m_iBulletCombo >= attribute.deviationStart)
			{
				m_fBulletDevitionAngle += attribute.deviationDeltaAngle;
				m_fBulletDevitionAngle = Mathf.Min(m_fBulletDevitionAngle, attribute.deviationMaxAngle);
			}
			float angle = 0f;
			if (m_iBulletCombo >= attribute.deviationStart)
			{
				angle = ((((uint)m_iBulletCombo & (true ? 1u : 0u)) != 0) ? (0f - m_fBulletDevitionAngle) : m_fBulletDevitionAngle);
			}
			m_bulletTransCopy.transform.Rotate(Vector3.up, angle);
			m_bulletRotation = m_bulletTransCopy.transform.rotation;
			RaycastHit[] array2 = Physics.RaycastAll(m_weaponBonePoint.position, m_bulletTransCopy.transform.forward, num, layermask);
			if (array2 != null && array2.Length > 0)
			{
				RaycastHit firstHit2;
				Util.RaycastHitFirstTarget(array2, m_weaponBonePoint.position, out firstHit2);
				float magnitude2 = (firstHit2.point - owner.GetTransform().position).magnitude;
				if (((1 << firstHit2.collider.gameObject.layer) & num2) != 0)
				{
					DS2ActiveObject object2 = DS2ObjectStub.GetObject<DS2ActiveObject>(firstHit2.collider.gameObject);
					if (!object2.Alive())
					{
						EmitBullet(magnitude2);
						return;
					}
					HitInfo hitInfo2 = owner.GetHitInfo();
					hitInfo2.hitPoint = firstHit2.point;
					hitInfo2.repelDirection = owner.GetModelTransform().forward;
					float num4 = (attribute.attackRange - magnitude2) / attribute.attackRange;
					hitInfo2.repelDistance = new NumberSection<float>(hitInfo2.repelDistance.left * num4, hitInfo2.repelDistance.right * num4);
					object2.OnHit(hitInfo2);
					BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit2.point, 0.3f);
				}
				else
				{
					BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit2.point, 0.3f, null, false);
				}
				num = magnitude2;
			}
			EmitBullet(num);
		}
	}
}
