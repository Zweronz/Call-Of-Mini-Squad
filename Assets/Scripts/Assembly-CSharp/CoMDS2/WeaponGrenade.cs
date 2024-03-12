using UnityEngine;

namespace CoMDS2
{
	public class WeaponGrenade : Weapon
	{
		private int m_effectFireIndex;

		public WeaponGrenade(WeaponType type)
			: base(type)
		{
		}

		public override void Initialize(int level)
		{
			base.Initialize(level);
			for (int i = 0; i < m_effectFire.Length; i++)
			{
				m_effectFire[i].transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			}
		}

		protected override void SetBullet()
		{
			bulletAttribute = new Bullet.BulletAttribute();
			bulletAttribute.speed = attribute.bulletSpeed;
			bulletAttribute.life = 2f;
			bulletAttribute.effectHit = attribute.effectHit;
			bulletAttribute.damageType = Bullet.BULLET_DAMAGE_TYPE.MULTI;
			bulletAttribute.damageRange = 2.5f;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(attribute.bulletType);
			int b = (int)(3f / attribute.fireFrequency);
			b = Mathf.Max(1, b) * Mathf.Max(1, attribute.rapid);
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < b; i++)
			{
				Bullet bullet = ((attribute.bulletType != 10) ? new Bullet() : new BulletGrenade());
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
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

		public override void DoReload()
		{
			base.DoReload();
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
			HitInfo hitInfo = owner.GetHitInfo();
			hitInfo.repelDirection = owner.GetModelTransform().forward;
			hitInfo.source = owner;
			m_bulletRotation = owner.GetModelTransform().rotation;
			EmitBullet(shootRange);
		}

		public override void EffectFireStart(bool playAudio = true)
		{
			if (m_effectFire != null)
			{
				m_effectFire[m_effectFireIndex].transform.rotation = m_firePoint.transform.rotation;
				m_effectFire[m_effectFireIndex].transform.position = m_firePoint.transform.position;
				m_effectFire[m_effectFireIndex].Root.SetActive(true);
				m_effectFire[m_effectFireIndex].StartEmit(playAudio);
				m_effectFireIndex++;
				if (m_effectFireIndex >= m_effectFire.Length)
				{
					m_effectFireIndex = 0;
				}
			}
		}
	}
}
