using UnityEngine;

namespace CoMDS2
{
	public class WeaponSniperWesker : WeaponSniper
	{
		public WeaponSniperWesker(WeaponType type)
			: base(type)
		{
		}

		protected override void SetBullet()
		{
			bulletAttribute = new Bullet.BulletAttribute();
			bulletAttribute.speed = attribute.bulletSpeed;
			bulletAttribute.life = 2f;
			bulletAttribute.effectHit = attribute.effectHit;
			bulletAttribute.damageRange = 2.5f;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(attribute.bulletType);
			int b = (int)(3f / attribute.fireFrequency);
			b = Mathf.Max(1, b) * Mathf.Max(1, attribute.rapid) * (int)attribute.extra[0];
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < b; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load("Models/Bullets/" + bulletDataByIndex.fileNmae) as GameObject, bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				BulletDelayShoot bulletDelayShoot = gameObject2.AddComponent<BulletDelayShoot>();
				bulletDelayShoot.SetBullet(bullet);
				if (attribute.bulletSpeed < 60f)
				{
					gameObject2.AddComponent<BulletTriggerScript>();
				}
				bullet.hitInfo.damage = attribute.damage;
				bullet.hitInfo.repelTime = 0.2f;
				bullet.hitInfo.repelDistance = attribute.repelDis;
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bullet);
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
			m_bulletRotation = owner.GetTransform().rotation;
			RaycastHit[] array = Physics.RaycastAll(m_weaponBonePoint.position, owner.GetTransform().forward, num, layermask);
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
						if (magnitude > attribute.attackRange / 2f)
						{
						}
						float num4 = (num - magnitude) / num;
						hitInfo.repelDistance = new NumberSection<float>(hitInfo.repelDistance.left * num4, hitInfo.repelDistance.right * num4);
						for (int j = 0; j < (int)attribute.extra[0]; j++)
						{
							@object.OnHit(hitInfo);
						}
						num3++;
						BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, raycastHit.point, 1f);
					}
					else
					{
						BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, raycastHit.point, 1f, null, false);
					}
					if (((uint)(1 << raycastHit.collider.gameObject.layer) & 0x50000u) != 0)
					{
						num = magnitude;
						break;
					}
				}
			}
			EmitBullet(num);
		}

		public override void EmitBullet(float distanceLife = 0f)
		{
			for (int i = 0; i < (int)attribute.extra[0]; i++)
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
					Quaternion rotation = Quaternion.AngleAxis(m_bulletRotation.eulerAngles.y - attribute.extra[2] * (attribute.extra[0] - 1f) * 0.5f + attribute.extra[2] * (float)i, Vector3.up);
					bulletFromBuffer.SetBullet(owner, bulletAttribute, new Vector3(m_firePoint.position.x, m_firePoint.position.y, m_firePoint.position.z), rotation);
					bulletFromBuffer.GetGameObject().GetComponent<BulletDelayShoot>().ShootBulletDelay(Random.Range(0f, attribute.extra[1]), distanceLife);
				}
			}
		}
	}
}
