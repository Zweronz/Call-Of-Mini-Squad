using UnityEngine;

namespace CoMDS2
{
	public class WeaponLaser : Weapon
	{
		public WeaponLaser(WeaponType type)
			: base(type)
		{
		}

		protected override void SetBullet()
		{
			bulletAttribute = new Bullet.BulletAttribute();
			bulletAttribute.speed = attribute.bulletSpeed;
			bulletAttribute.life = 2f;
			bulletAttribute.effectHit = attribute.effectHit;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(attribute.bulletType);
			int b = (int)(3f / attribute.fireFrequency);
			b = Mathf.Max(1, b) * Mathf.Max(1, attribute.rapid);
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < b; i++)
			{
				DS2HalfStaticObject dS2HalfStaticObject = new DS2HalfStaticObject();
				dS2HalfStaticObject.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				dS2HalfStaticObject.GetGameObject().transform.parent = gameObject.transform;
				dS2HalfStaticObject.GetGameObject().name = bulletDataByIndex.fileNmae;
				dS2HalfStaticObject.GetGameObject().SetActive(false);
				m_bulletBuffer.AddObj(dS2HalfStaticObject);
			}
			m_bulletTransCopy = new GameObject();
			m_bulletTransCopy.name = "bulletTransCopy";
			m_bulletTransCopy.transform.parent = GetTransform();
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
			float angle = 0f;
			if (m_iBulletCombo >= attribute.deviationStart)
			{
				angle = ((((uint)m_iBulletCombo & (true ? 1u : 0u)) != 0) ? (0f - m_fBulletDevitionAngle) : m_fBulletDevitionAngle);
			}
			m_bulletTransCopy.transform.Rotate(Vector3.up, angle);
			m_bulletRotation = m_bulletTransCopy.transform.rotation;
			RaycastHit[] array = Physics.RaycastAll(m_weaponBonePoint.position, m_bulletTransCopy.transform.forward, num, layermask);
			Util.s_compareRaycastHitPosition = m_weaponBonePoint.position;
			Util.SortHitListFromNearToFar(array);
			if (array != null && array.Length > 0)
			{
				if (array[array.Length - 1].collider.gameObject.layer == 67108864)
				{
					return;
				}
				int num3 = 0;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					float magnitude = (raycastHit.point - owner.GetTransform().position).magnitude;
					if (((1 << raycastHit.collider.gameObject.layer) & num2) != 0)
					{
						DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(raycastHit.collider.gameObject);
						if (!@object.Alive())
						{
							EmitBullet(num);
							continue;
						}
						HitInfo hitInfo = owner.GetHitInfo();
						hitInfo.damage = new NumberSection<float>(hitInfo.damage.left - hitInfo.damage.left * (float)num3 * 0.1f, hitInfo.damage.right - hitInfo.damage.right * (float)num3 * 0.1f);
						hitInfo.hitPoint = raycastHit.point;
						hitInfo.repelTime = 0.25f;
						hitInfo.repelDistance = attribute.repelDis;
						hitInfo.repelDirection = owner.GetModelTransform().forward;
						hitInfo.source = owner;
						hitInfo.repelDistance = new NumberSection<float>(hitInfo.repelDistance.left, hitInfo.repelDistance.right);
						@object.OnHit(hitInfo);
						num3++;
						BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, raycastHit.point, 1f);
					}
					else
					{
						BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, raycastHit.point, 0.1f, null, false);
					}
					if (((uint)(1 << raycastHit.collider.gameObject.layer) & 0x50000u) != 0)
					{
						num = magnitude - 1f;
						break;
					}
				}
			}
			EmitBullet(num);
		}

		protected DS2HalfStaticObject GetBulletEffectFromBuffer()
		{
			return m_bulletBuffer.GetObject() as DS2HalfStaticObject;
		}

		public override void EmitBullet(float range)
		{
			DS2HalfStaticObject bulletEffectFromBuffer = GetBulletEffectFromBuffer();
			if (bulletEffectFromBuffer != null)
			{
				bulletEffectFromBuffer.GetGameObject().SetActive(true);
				bulletEffectFromBuffer.GetTransform().localScale = new Vector3(1f, 1f, range);
				bulletEffectFromBuffer.GetTransform().position = m_firePoint.position;
				bulletEffectFromBuffer.GetTransform().forward = owner.GetTransform().forward;
			}
		}

		public override void StopFire()
		{
			base.StopFire();
		}
	}
}
