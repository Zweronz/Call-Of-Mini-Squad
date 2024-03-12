using UnityEngine;

namespace CoMDS2
{
	public class Weapon
	{
		public enum WeaponType
		{
			ShotGun_01 = 0,
			Machinegun2 = 1,
			Pistol_Lili = 2,
			Sniper_03 = 3,
			Grenade_Claire = 4,
			Ninjia_Fire_03 = 5,
			Ninjia_Ice_02 = 6,
			Grenade_01 = 7,
			Laser_02 = 8,
			Magnum = 9,
			Virus_Eva = 10,
			ShotGun_02 = 11,
			Machinegun_Tanya = 12,
			Pistol_04 = 13,
			Machinegun4 = 14,
			Sniper_Wesker = 15,
			ShotGun_04 = 16,
			Grenade_Shepard = 17
		}

		public enum HIT_TYPE
		{
			TRAJECTORY = 0,
			COLLIDE = 1
		}

		public struct WeaponAttribute
		{
			public float fireFrequency;

			public HIT_TYPE hitType;

			public int bulletType;

			public int clip;

			public float reloadTime;

			public float bulletSpeed;

			public NumberSection<float> repelDis;

			public NumberSection<float> damage;

			public int deviationStart;

			public float deviationRecoverTime;

			public float deviationDeltaAngle;

			public float deviationMaxAngle;

			public float attackRange;

			public Defined.EFFECT_TYPE effectHit;

			public int hitStrength;

			public int rapid;

			public float rapidInterval;

			public float[] extra;
		}

		protected string m_name;

		protected GameObject m_gameObject;

		protected Transform m_transform;

		public Transform m_firePoint;

		protected Transform m_weaponBonePoint;

		protected float m_time;

		protected float m_devitionRecoverTimer;

		protected EffectControl[] m_effectFire;

		protected EffectAnimationEmitPool m_effectCartridge;

		protected EffectControl m_effectLight;

		public WeaponAttribute attribute;

		public Bullet.BulletAttribute bulletAttribute;

		public int m_iBulletCombo;

		public float m_fBulletDevitionAngle;

		public float m_fBulletDevitionAngleMark;

		public int m_iBulletCount;

		protected Quaternion m_bulletRotation;

		public bool m_bRunningFire;

		protected DS2ObjectBuffer m_bulletBuffer;

		protected GameObject m_bulletTransCopy;

		protected int m_rapidCount;

		protected float m_rapidDeltaTime;

		public bool rapidFiring;

		private ITAudioEvent m_audioReload;

		protected float m_fireTimer;

		protected float m_reloadTimer;

		public WeaponType type { get; set; }

		public int level { get; set; }

		public Defined.RANK_TYPE rank { get; set; }

		public Defined.EFFECT_TYPE effectHit { get; set; }

		public virtual Character owner { get; set; }

		public float emitTimeInAnimation { get; set; }

		public virtual bool Active
		{
			get
			{
				return m_gameObject.activeInHierarchy;
			}
			set
			{
				m_gameObject.SetActive(value);
			}
		}

		public Weapon()
		{
		}

		public Weapon(WeaponType type)
		{
			this.type = type;
		}

		protected void SetAttribute(DataConf.WeaponData weaponBaseData, int level)
		{
			WeaponAttribute weaponAttribute = default(WeaponAttribute);
			weaponAttribute.bulletSpeed = weaponBaseData.bulletSpeed;
			weaponAttribute.hitType = (HIT_TYPE)weaponBaseData.hitType;
			weaponAttribute.bulletType = weaponBaseData.bulletType;
			int num = level / DataConf.s_weaponLevelPerPhase;
			int num2 = level - num * DataConf.s_weaponLevelPerPhase;
			DataConf.WeaponUpgradePhase weaponUpgradePhase = weaponBaseData.upgradePhaseList[num];
			weaponAttribute.clip = weaponUpgradePhase.ammo;
			weaponAttribute.fireFrequency = weaponUpgradePhase.fireFrequence;
			weaponAttribute.reloadTime = weaponUpgradePhase.reloadTime;
			weaponAttribute.repelDis = new NumberSection<float>(weaponBaseData.repelDis);
			weaponAttribute.damage = new NumberSection<float>(weaponUpgradePhase.damage.left + weaponUpgradePhase.damageIncr * (float)num2, weaponUpgradePhase.damage.right + weaponUpgradePhase.damageIncr * (float)num2);
			weaponAttribute.deviationStart = weaponBaseData.deviationStart;
			weaponAttribute.deviationRecoverTime = weaponBaseData.deviationRecoverTime;
			weaponAttribute.deviationDeltaAngle = weaponBaseData.deviationDeltaAngle;
			weaponAttribute.deviationMaxAngle = weaponBaseData.deviationMaxAngle;
			weaponAttribute.attackRange = weaponBaseData.attackRange;
			weaponAttribute.effectHit = (Defined.EFFECT_TYPE)weaponBaseData.effectHit;
			weaponAttribute.hitStrength = weaponBaseData.hitStrength;
			weaponAttribute.rapid = weaponUpgradePhase.rapid;
			weaponAttribute.rapidInterval = weaponUpgradePhase.rapidInterval;
			weaponAttribute.extra = new float[weaponBaseData.extra.Length];
			for (int i = 0; i < weaponBaseData.extra.Length; i++)
			{
				weaponAttribute.extra[i] = weaponBaseData.extra[i];
			}
			attribute = weaponAttribute;
		}

		public virtual void Initialize(int level)
		{
			m_fireTimer = 9999f;
			DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(type);
			SetAttribute(weaponDataByType, level);
			m_iBulletCount = attribute.clip;
			m_bRunningFire = attribute.fireFrequency <= 0.2f;
			emitTimeInAnimation = weaponDataByType.emitTimeInAnimation;
			type = type;
			rank = rank;
			m_name = weaponDataByType.name;
			this.level = level;
			m_gameObject = Object.Instantiate(Resources.Load("Models/Weapons/" + weaponDataByType.modelFileName), Vector3.zero, Quaternion.identity) as GameObject;
			m_transform = m_gameObject.transform;
			m_effectFire = null;
			m_firePoint = m_transform.Find("Model/Bone_Weapon/FirePoint");
			int childCount = m_firePoint.childCount;
			if (childCount > 0)
			{
				m_effectFire = new EffectControl[childCount];
				for (int i = 0; i < childCount; i++)
				{
					m_effectFire[i] = m_firePoint.GetChild(i).GetComponentInChildren<EffectControl>();
					m_effectFire[i].Root.SetActive(false);
				}
			}
			SetBullet();
			if (GameBattle.m_instance != null)
			{
				Transform transform = m_transform.Find("EffectLight");
				if (transform != null)
				{
					m_effectLight = transform.gameObject.GetComponentInChildren<EffectControl>();
					if (m_effectLight != null)
					{
						m_effectLight.Root.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
						m_effectLight.Root.transform.localPosition = Vector3.zero;
						m_effectLight.Root.SetActive(false);
					}
				}
			}
			m_weaponBonePoint = m_transform.Find("Model/Bone_Weapon");
			Transform transform2 = GetTransform().Find("AudioReload");
			if (transform2 != null)
			{
				m_audioReload = transform2.GetComponentInChildren<ITAudioEvent>();
			}
			LoadResources();
		}

		protected virtual void LoadResources()
		{
			if (GameBattle.m_instance != null && attribute.effectHit != Defined.EFFECT_TYPE.NONE)
			{
				BattleBufferManager.Instance.CreateEffectBufferByType(attribute.effectHit);
			}
		}

		protected virtual void SetBullet()
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
				Bullet bullet = new Bullet(null);
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

		public virtual void SetActive(bool active)
		{
			m_gameObject.SetActive(active);
		}

		public virtual void Mount(Transform character, Transform weaponPoint, Transform fireLightPoint)
		{
			m_transform.parent = weaponPoint;
			m_transform.localPosition = Vector3.zero;
			m_transform.localEulerAngles = Vector3.zero;
		}

		public virtual void Unmount()
		{
			m_transform.parent = null;
		}

		public string GetWeaponName()
		{
			return m_name;
		}

		public virtual Transform GetTransform()
		{
			return m_transform;
		}

		public virtual void EffectFireStart(bool playAudio = true)
		{
			if (m_effectFire != null)
			{
				m_effectFire[0].StartEmit(playAudio);
			}
		}

		public virtual void EffectFireStart(int effectIndex, bool playAudio = true)
		{
			if (m_effectFire != null)
			{
				m_effectFire[effectIndex].StartEmit(playAudio);
			}
		}

		public virtual void SetEffectPlaySpeed(int effectIndex, float speed)
		{
			if (m_effectFire != null)
			{
				m_effectFire[effectIndex].SetSpeed(speed);
			}
		}

		public virtual void EffectFireStop()
		{
			if (m_effectFire != null)
			{
				m_effectFire[0].StopEmit();
			}
		}

		public virtual void EffectCartridgeEmit()
		{
			if (m_effectCartridge != null)
			{
				m_effectCartridge.Emit();
			}
		}

		public void PlayEffectLight()
		{
			if (m_effectLight != null)
			{
				m_effectLight.Root.transform.position = new Vector3(m_firePoint.position.x, m_effectLight.Root.transform.position.y, m_firePoint.position.z);
				m_effectLight.Root.SetActive(true);
				m_effectLight.StartEmit();
			}
		}

		public void StopEffectLight()
		{
			if (m_effectLight != null)
			{
				m_effectLight.StopEmit();
				m_effectLight.Root.gameObject.SetActive(false);
			}
		}

		public virtual void EmitBullet(float distanceLife = 0f)
		{
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			if (bulletFromBuffer == null)
			{
				return;
			}
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
				bulletFromBuffer.SetBullet(owner, bulletAttribute, new Vector3(m_firePoint.position.x, m_firePoint.position.y, m_firePoint.position.z), m_bulletRotation);
				bulletFromBuffer.Emit(distanceLife);
			}
		}

		public virtual Bullet GetBulletFromBuffer()
		{
			return m_bulletBuffer.GetObject() as Bullet;
		}

		public virtual void StartFire()
		{
			EffectFireStart();
			EmitBullet();
			EffectCartridgeEmit();
			m_time = 0f;
			m_devitionRecoverTimer = 0f;
			owner.m_fire = true;
		}

		public virtual void StopFire()
		{
			m_iBulletCombo = 0;
			m_fBulletDevitionAngleMark = m_fBulletDevitionAngle;
			m_rapidCount = 0;
			rapidFiring = false;
			owner.m_fire = false;
		}

		public virtual void UpdateFire(float deltaTime)
		{
			if (NeedReload())
			{
				return;
			}
			owner.m_fire = true;
			if (attribute.rapid > 1)
			{
				if (!rapidFiring)
				{
					rapidFiring = true;
					m_rapidCount = 0;
					m_rapidDeltaTime = 0f;
				}
			}
			else
			{
				ShootBullet();
			}
		}

		protected virtual void ShootBullet()
		{
			m_iBulletCombo++;
			if (m_iBulletCount != -999)
			{
				m_iBulletCount--;
			}
			EffectFireStart();
			EffectCartridgeEmit();
			PlayEffectLight();
			float num = attribute.attackRange;
			m_fireTimer = 0f;
			m_time = 0f;
			if (attribute.hitType == HIT_TYPE.TRAJECTORY)
			{
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
				m_bulletTransCopy.transform.Rotate(Vector3.up, angle);
				m_bulletRotation = m_bulletTransCopy.transform.rotation;
				RaycastHit[] array = Physics.RaycastAll(m_weaponBonePoint.position, m_bulletTransCopy.transform.forward, num, layermask);
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
						BattleBufferManager.Instance.GenerateEffectFromBuffer(attribute.effectHit, firstHit.point, 0.2f, null, false);
					}
					num = magnitude;
				}
				EmitBullet(num);
			}
			else
			{
				HitInfo hitInfo2 = owner.GetHitInfo();
				hitInfo2.repelDirection = owner.GetModelTransform().forward;
				hitInfo2.source = owner;
				m_bulletRotation = owner.GetModelTransform().rotation;
				EmitBullet(num);
			}
		}

		public virtual bool NeedReload()
		{
			if (owner.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				return false;
			}
			return m_iBulletCount <= 0 && m_iBulletCount != -999;
		}

		public virtual void DoReload()
		{
			m_reloadTimer = 0f;
			m_iBulletCount = attribute.clip;
		}

		public virtual void Dispose()
		{
			Object.Destroy(m_gameObject);
			m_gameObject = null;
			m_transform = null;
			m_firePoint = null;
			m_effectFire = null;
			m_effectCartridge = null;
			owner = null;
		}

		public virtual void DoDevitionRecover()
		{
			m_devitionRecoverTimer += Time.deltaTime / attribute.deviationRecoverTime;
			m_fBulletDevitionAngle = Mathf.LerpAngle(m_fBulletDevitionAngleMark, 0f, m_devitionRecoverTimer);
		}

		public virtual void Update(bool fire)
		{
			if (!fire && m_fBulletDevitionAngle > 0f)
			{
				DoDevitionRecover();
			}
			if (rapidFiring)
			{
				if (NeedReload())
				{
					rapidFiring = false;
				}
				else if (!owner.Alive() || !owner.m_fire || owner.isStuck)
				{
					rapidFiring = false;
				}
				else if (m_rapidDeltaTime <= 0f)
				{
					owner.AnimationPlay(owner.animUpperBody, false, true);
					ShootBullet();
					m_rapidCount++;
					m_rapidDeltaTime = attribute.rapidInterval;
					if (m_rapidCount >= attribute.rapid)
					{
						rapidFiring = false;
					}
				}
				else
				{
					m_rapidDeltaTime -= Time.deltaTime;
				}
			}
			else
			{
				if (m_fireTimer < attribute.fireFrequency)
				{
					m_fireTimer += Time.deltaTime;
				}
				if (NeedReload() && owner.isInReload)
				{
					m_reloadTimer += Time.deltaTime;
				}
			}
		}

		public bool firePermit()
		{
			return m_fireTimer >= attribute.fireFrequency;
		}

		public bool ReloadFinished()
		{
			return m_reloadTimer >= attribute.reloadTime;
		}

		public float GetReloadProgress()
		{
			return (attribute.reloadTime - m_reloadTimer) / attribute.reloadTime;
		}

		public virtual void Reset()
		{
			m_time = 0f;
			m_devitionRecoverTimer = 0f;
			m_iBulletCombo = 0;
			m_fBulletDevitionAngle = 0f;
			m_fBulletDevitionAngleMark = 0f;
			m_iBulletCount = attribute.clip;
		}

		public virtual void PlayAudioReload()
		{
			if (m_audioReload != null)
			{
				m_audioReload.Trigger();
			}
		}
	}
}
