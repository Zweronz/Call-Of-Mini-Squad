using UnityEngine;

namespace CoMDS2
{
	public class WeaponShotgun : Weapon
	{
		public WeaponShotgun(WeaponType type)
			: base(type)
		{
		}

		public override void Initialize(int level)
		{
			base.Initialize(level);
			if (GameBattle.m_instance != null)
			{
				m_effectFire[0].transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
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
			float shootRange = owner.shootRange;
			m_fireTimer = 0f;
			m_time = 0f;
			int num = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 67962880 : 67962368);
			int num2 = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 526336 : 1536);
			if (owner.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && owner.clique == DS2ActiveObject.Clique.Player)
			{
				Player player = (Player)owner;
				if (!player.CurrentController)
				{
					num = 329728;
					num2 = 2048;
				}
			}
			Ray ray = new Ray(m_weaponBonePoint.position, owner.GetModelTransform().forward);
			RaycastHit[] array = Physics.SphereCastAll(ray, attribute.deviationMaxAngle, attribute.attackRange, num);
			if (array == null || array.Length <= 0 || (array.Length > 0 && array[array.Length - 1].collider.gameObject.layer == 67108864))
			{
				return;
			}
			bool flag = false;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (((1 << raycastHit.collider.gameObject.layer) & num2) == 0)
				{
					continue;
				}
				flag = true;
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(raycastHit.collider.gameObject);
				if (!@object.Alive())
				{
					continue;
				}
				HitInfo hitInfo = owner.GetHitInfo();
				Vector3 repelDirection = raycastHit.collider.transform.position - owner.GetTransform().position;
				hitInfo.hitPoint = raycastHit.point;
				hitInfo.repelDirection = repelDirection;
				float magnitude = repelDirection.magnitude;
				if (magnitude > attribute.attackRange / 2f)
				{
					RaycastHit[] array3 = Physics.RaycastAll(m_firePoint.position, owner.GetModelTransform().forward, magnitude, num);
					if (array3 != null && array3.Length > 1)
					{
						continue;
					}
				}
				float num3 = (attribute.attackRange - magnitude) / attribute.attackRange;
				hitInfo.repelDistance = new NumberSection<float>(hitInfo.repelDistance.left * num3, hitInfo.repelDistance.right * num3);
				@object.OnHit(hitInfo);
				BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, raycastHit.point, 1f);
			}
			if (!flag && array.Length > 0)
			{
				BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, array[0].point, 1.02f, null, false);
			}
		}

		public override void EffectFireStart(bool playAudio = true)
		{
			if (m_effectFire[0] != null)
			{
				m_effectFire[0].transform.rotation = m_firePoint.transform.rotation;
				m_effectFire[0].transform.position = m_firePoint.transform.position;
				m_effectFire[0].Root.SetActive(true);
				m_effectFire[0].StartEmit(playAudio);
			}
		}
	}
}
