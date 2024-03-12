using UnityEngine;

namespace CoMDS2
{
	public class WeaponPistol : Weapon
	{
		public enum PistolType
		{
			Scatter = 0,
			Buff = 1
		}

		public enum HandType
		{
			Single = 0,
			Dual = 1
		}

		protected WeaponPistolSingle m_pistolLeft;

		protected WeaponPistolSingle m_pistolRight;

		protected float m_otherSideFireTimer;

		protected float m_otherSideFireInterval = 0.35f;

		protected bool m_bOtherSideFire;

		public PistolType pistolType { get; set; }

		public HandType handType { get; set; }

		public override Character owner
		{
			get
			{
				return base.owner;
			}
			set
			{
				base.owner = value;
				m_pistolLeft.owner = value;
				m_pistolRight.owner = value;
			}
		}

		public WeaponPistol(WeaponType weaponType, PistolType pistolType)
			: base(weaponType)
		{
			this.pistolType = pistolType;
			handType = HandType.Dual;
			m_pistolLeft = new WeaponPistolSingle(weaponType, pistolType);
			m_pistolRight = new WeaponPistolSingle(weaponType, pistolType);
		}

		public override void Initialize(int level)
		{
			DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(base.type);
			SetAttribute(weaponDataByType, level);
			if (GameBattle.m_instance != null)
			{
				m_iBulletCount = attribute.clip;
				m_bRunningFire = attribute.fireFrequency >= 300f;
			}
			m_name = weaponDataByType.name;
			m_pistolLeft.Initialize(level);
			m_pistolRight.Initialize(level);
		}

		public override void Mount(Transform character, Transform weaponPoint, Transform fireLightPoint)
		{
			m_pistolLeft.Mount(character, character.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 Prop2"), fireLightPoint);
			m_pistolRight.Mount(character, character.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1"), fireLightPoint);
		}

		public override void SetActive(bool active)
		{
			if (handType != 0)
			{
				m_pistolLeft.SetActive(active);
			}
			m_pistolRight.SetActive(active);
		}

		public override void Update(bool fire)
		{
			if (!fire && m_fBulletDevitionAngle > 0f)
			{
				DoDevitionRecover();
			}
			bool flag = NeedReload();
			if (attribute.rapid > 1)
			{
				if (rapidFiring)
				{
					if (flag)
					{
						rapidFiring = false;
						return;
					}
					if (!owner.Alive() || !owner.m_fire || owner.isStuck)
					{
						rapidFiring = false;
						return;
					}
					if (handType == HandType.Single)
					{
						if (m_rapidDeltaTime <= 0f)
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
					else if (!m_bOtherSideFire)
					{
						if (m_rapidDeltaTime <= 0f)
						{
							ShootBullet();
							owner.AnimationPlay(owner.animUpperBody, false, true);
						}
						else
						{
							m_rapidDeltaTime -= Time.deltaTime;
						}
					}
					else
					{
						m_otherSideFireTimer += Time.deltaTime;
						if (m_otherSideFireTimer >= m_otherSideFireInterval)
						{
							if (m_iBulletCount != -999)
							{
								m_iBulletCount--;
							}
							m_otherSideFireTimer = 0f;
							m_bOtherSideFire = false;
							m_pistolLeft.UpdateFire(Time.deltaTime);
							m_rapidCount++;
							m_rapidDeltaTime = attribute.rapidInterval;
							if (m_rapidCount >= attribute.rapid)
							{
								rapidFiring = false;
							}
						}
					}
				}
			}
			else if (!flag && m_bOtherSideFire && handType != 0)
			{
				m_otherSideFireTimer += Time.deltaTime;
				if (m_otherSideFireTimer >= m_otherSideFireInterval)
				{
					if (m_iBulletCount != -999)
					{
						m_iBulletCount--;
					}
					m_otherSideFireTimer = 0f;
					m_bOtherSideFire = false;
					m_pistolLeft.UpdateFire(Time.deltaTime);
				}
			}
			if (m_fireTimer < attribute.fireFrequency)
			{
				m_fireTimer += Time.deltaTime;
			}
			if (flag && owner.isInReload)
			{
				m_reloadTimer += Time.deltaTime;
			}
		}

		public override void DoReload()
		{
			base.DoReload();
			m_bOtherSideFire = false;
		}

		protected override void ShootBullet()
		{
			if (m_iBulletCount != -999)
			{
				m_iBulletCount--;
			}
			m_pistolRight.UpdateFire(Time.deltaTime);
			m_bOtherSideFire = handType != HandType.Single;
			m_otherSideFireTimer = 0f;
			m_fireTimer = 0f;
		}

		public WeaponPistolSingle GetLeftGun()
		{
			return m_pistolLeft;
		}

		public WeaponPistolSingle GetRightGun()
		{
			return m_pistolRight;
		}

		public override void PlayAudioReload()
		{
			if (GetLeftGun() != null && GetLeftGun().Active)
			{
				GetLeftGun().PlayAudioReload();
			}
			if (GetRightGun() != null && GetRightGun().Active)
			{
				GetRightGun().PlayAudioReload();
			}
		}

		public override Transform GetTransform()
		{
			if (m_pistolRight != null)
			{
				return m_pistolRight.GetTransform();
			}
			if (m_pistolLeft != null)
			{
				return m_pistolLeft.GetTransform();
			}
			return null;
		}
	}
}
