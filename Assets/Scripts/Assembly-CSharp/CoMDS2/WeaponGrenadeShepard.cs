using UnityEngine;

namespace CoMDS2
{
	public class WeaponGrenadeShepard : WeaponGrenade
	{
		public WeaponGrenadeShepard(WeaponType type)
			: base(type)
		{
		}

		protected override void SetBullet()
		{
			bulletAttribute = new Bullet.BulletAttribute();
			bulletAttribute.speed = attribute.bulletSpeed;
			bulletAttribute.life = 2f;
			bulletAttribute.effectHit = attribute.effectHit;
			bulletAttribute.damageType = Bullet.BULLET_DAMAGE_TYPE.MULTI;
			bulletAttribute.damageRange = attribute.extra[0];
			bulletAttribute.damageDecayRange = attribute.extra[1];
			bulletAttribute.damageDecayPercent = attribute.extra[2];
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(attribute.bulletType);
			int b = (int)(3f / attribute.fireFrequency);
			b = Mathf.Max(1, b) * Mathf.Max(1, attribute.rapid) * (int)attribute.extra[4];
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			GameObject prefab = Resources.Load<GameObject>("Models/ActiveObjects/ChargeBolt");
			for (int i = 0; i < b; i++)
			{
				Bullet bullet = ((attribute.bulletType != 10) ? new Bullet() : new BulletGrenade());
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				BulletTriigerChargeBolt bulletTriigerChargeBolt = gameObject2.AddComponent<BulletTriigerChargeBolt>();
				bullet.hitInfo.damage = attribute.damage;
				bullet.hitInfo.repelTime = 0.2f;
				bullet.hitInfo.repelDistance = attribute.repelDis;
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bullet);
				ChargeBolt chargeBolt = new ChargeBolt();
				chargeBolt.Initialize(prefab, "ChargeBolt", Vector3.zero, Quaternion.identity, 0);
				chargeBolt.GetGameObject().SetActive(false);
				chargeBolt.GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
				chargeBolt.burstInterval = attribute.extra[3];
				chargeBolt.burstLife = attribute.extra[4];
				bulletTriigerChargeBolt.chargeBolt = chargeBolt;
				bulletTriigerChargeBolt.chargeBolt.hitInfo = new HitInfo(bullet.hitInfo);
				bulletTriigerChargeBolt.chargeBolt.hitInfo.damage = new NumberSection<float>(bulletTriigerChargeBolt.chargeBolt.hitInfo.damage.left * 0.4f, bulletTriigerChargeBolt.chargeBolt.hitInfo.damage.right * 0.4f);
				bulletTriigerChargeBolt.chargeBolt.hitInfo.repelTime = 0f;
				bulletTriigerChargeBolt.chargeBolt.hitInfo.source = bulletTriigerChargeBolt.chargeBolt;
			}
			m_bulletTransCopy = new GameObject();
			m_bulletTransCopy.name = "bulletTransCopy";
			m_bulletTransCopy.transform.parent = GetTransform();
		}
	}
}
