using UnityEngine;

namespace CoMDS2
{
	public class WeaponShotgunNPC : Weapon
	{
		public WeaponShotgunNPC(Defined.RANK_TYPE rank)
			: base(WeaponType.ShotGun_01)
		{
			Initialize(1);
		}

		public override void Initialize(int level)
		{
			base.Initialize(level);
			if (GameBattle.m_instance != null)
			{
				m_effectFire[0].transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			}
		}

		protected override void SetBullet()
		{
		}

		public void SetBulletExternal()
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
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				gameObject2.AddComponent<BulletTriggerScript>();
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

		public override void UpdateFire(float deltaTime)
		{
			if (NeedReload())
			{
				return;
			}
			owner.m_fire = true;
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
			if (array == null || (array.Length > 0 && array[array.Length - 1].collider.gameObject.layer == 67108864))
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
				HitInfo hitInfo = new HitInfo(owner.GetHitInfo());
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
					hitInfo.damage = new NumberSection<float>(hitInfo.damage.left * 0.2f, hitInfo.damage.right * 0.2f);
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
