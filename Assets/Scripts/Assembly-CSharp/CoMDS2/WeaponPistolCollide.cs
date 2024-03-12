using UnityEngine;

namespace CoMDS2
{
	public class WeaponPistolCollide : Weapon
	{
		public WeaponPistolCollide()
			: base(WeaponType.Pistol_04)
		{
			Initialize(1);
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
			b = Mathf.Max(1, b);
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
			if (!NeedReload())
			{
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
				HitInfo hitInfo = owner.GetHitInfo();
				hitInfo.repelDirection = owner.GetModelTransform().forward;
				hitInfo.source = owner;
				m_bulletRotation = owner.GetModelTransform().rotation;
				EmitBullet(shootRange);
			}
		}
	}
}
