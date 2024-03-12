using UnityEngine;

namespace CoMDS2
{
	public class WeaponPistolDoctorSingle : WeaponPistolSingle
	{
		public WeaponPistolDoctorSingle(WeaponType weaponType, WeaponPistol.PistolType pistolType)
			: base(weaponType, pistolType)
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
			b = Mathf.Max(1, b) * Mathf.Max(1, attribute.rapid);
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < b; i++)
			{
				BulletDoctor bulletDoctor = new BulletDoctor(null);
				bulletDoctor.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bulletDoctor.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				gameObject2.AddComponent<BulletTriggerScript>();
				bulletDoctor.hitInfo.damage = attribute.damage;
				bulletDoctor.hitInfo.repelTime = 0.2f;
				bulletDoctor.hitInfo.repelDistance = attribute.repelDis;
				bulletDoctor.changeHpPercent = attribute.extra[0];
				gameObject2.SetActive(false);
				bulletDoctor.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bulletDoctor);
			}
			m_bulletTransCopy = new GameObject();
			m_bulletTransCopy.name = "bulletTransCopy";
			m_bulletTransCopy.transform.parent = GetTransform();
		}

		public override void UpdateFire(float deltaTime)
		{
			m_iBulletCombo++;
			EffectFireStart();
			EffectCartridgeEmit();
			PlayEffectLight();
			float shootRange = owner.shootRange;
			m_fireTimer = 0f;
			m_time = 0f;
			m_bulletRotation = owner.GetModelTransform().rotation;
			EmitBullet(shootRange);
		}
	}
}
