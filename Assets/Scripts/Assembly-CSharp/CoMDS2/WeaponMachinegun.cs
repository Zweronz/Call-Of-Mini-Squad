using UnityEngine;

namespace CoMDS2
{
	public class WeaponMachinegun : Weapon
	{
		public WeaponMachinegun(WeaponType type)
			: base(type)
		{
			if (m_effectLight != null)
			{
				m_effectLight.gameObject.GetComponent<Animation>()["Anim_Light"].speed *= 10f;
			}
		}

		protected override void ShootBullet()
		{
			m_iBulletCombo++;
			if (m_iBulletCount != -999)
			{
				m_iBulletCount--;
			}
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
			m_bulletTransCopy.transform.rotation = owner.GetModelTransform().rotation;
			m_bulletTransCopy.transform.Rotate(Vector3.up, angle);
			m_bulletRotation = m_bulletTransCopy.transform.rotation;
			RaycastHit[] array = Physics.RaycastAll(owner.m_effectPoint.position, m_bulletTransCopy.transform.forward, num, layermask);
			if (array != null && array.Length > 0)
			{
				if (array[array.Length - 1].collider.gameObject.layer == 67108864)
				{
					return;
				}
				RaycastHit firstHit;
				Util.RaycastHitFirstTarget(array, m_weaponBonePoint.position, out firstHit);
				float magnitude = (firstHit.point - owner.GetTransform().position).magnitude;
				if (((1 << firstHit.collider.gameObject.layer) & num2) != 0)
				{
					DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(firstHit.collider.gameObject);
					if (!@object.Alive())
					{
						EmitBullet(magnitude);
						return;
					}
					HitInfo hitInfo = owner.GetHitInfo();
					hitInfo.hitPoint = firstHit.point;
					hitInfo.repelDirection = owner.GetModelTransform().forward;
					float num3 = (attribute.attackRange - magnitude) / attribute.attackRange;
					hitInfo.repelDistance = new NumberSection<float>(hitInfo.repelDistance.left * num3, hitInfo.repelDistance.right * num3);
					@object.OnHit(hitInfo);
					BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit.point, 0.2f);
				}
				else
				{
					BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit.point, 0.1f, null, false);
				}
				num = magnitude;
			}
			EmitBullet(num);
		}

		public override void StopFire()
		{
			base.StopFire();
			if (attribute.rapid <= 1)
			{
				EffectFireStop();
			}
		}
	}
}
