using UnityEngine;

namespace CoMDS2
{
	public class WeaponSniper : Weapon
	{
		public WeaponSniper(WeaponType type)
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
			if (m_effectLight != null)
			{
				m_effectLight.gameObject.GetComponent<Animation>()["Anim_Light"].speed *= 10f;
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

		public override void Mount(Transform character, Transform weaponPoint, Transform fireLightPoint)
		{
			base.Mount(character, weaponPoint, fireLightPoint);
			if (m_effectLight != null)
			{
				m_effectLight.Root.transform.localPosition = new Vector3(m_effectLight.Root.transform.localPosition.x, m_effectLight.Root.transform.localPosition.y, m_effectLight.Root.transform.localPosition.z + 0.8f);
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
			int layerMask = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 67962880 : 67962368);
			int num2 = ((owner.clique != DS2ActiveObject.Clique.Computer) ? 526336 : 1536);
			if (owner.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && owner.clique == DS2ActiveObject.Clique.Player)
			{
				Player player = (Player)owner;
				if (!player.CurrentController)
				{
					layerMask = 329728;
					num2 = 2048;
				}
			}
			m_bulletRotation = owner.GetTransform().rotation;
			Ray ray = new Ray(m_weaponBonePoint.position, owner.GetTransform().forward);
			RaycastHit[] array = Physics.RaycastAll(ray, num, layerMask);
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
						@object.OnHit(hitInfo);
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
