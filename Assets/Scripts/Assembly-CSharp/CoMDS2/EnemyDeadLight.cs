using UnityEngine;

namespace CoMDS2
{
	public class EnemyDeadLight : Enemy
	{
		private AIState.AIPhase m_laserPhase;

		private float m_laserShootingTime;

		private bool m_laserCharging;

		private bool m_laserRotate;

		private Quaternion m_laserStartRotation;

		private Quaternion m_laserTargetRotation;

		private float m_laserRotateTime;

		private Transform m_laserPoint;

		private GameObject m_laserBullet;

		private GameObject m_laserHitEffect;

		private float m_laserReviseDistance;

		private float m_laserAttackInterval;

		protected DS2ObjectBuffer m_bulletBuffer;

		public Bullet.BulletAttribute bulletAttribute;

		private float m_fBulletLife = 15f;

		private Transform m_shootPoint;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			base.meleeRange = (m_baseMeleeRange = 1.3f);
			SetAttackCollider(false);
			base.meleeRange = (m_baseMeleeRange = 8f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 12f;
			base.shootAble = false;
			m_shootPoint = GetTransform().Find("ShootPoint");
			m_laserPoint = GetTransform().Find("LaserPoint");
			if (base.enemyType == EnemyType.DeadLight)
			{
				m_laserBullet = GetTransform().Find("DeathLight_Attack_2_G_Bullet").gameObject;
				m_laserHitEffect = GetTransform().Find("DeathLight_Attack_2_G_Attack").gameObject;
			}
			else
			{
				m_laserBullet = GetTransform().Find("DeathLight_Attack_2_Z_Bullet").gameObject;
				m_laserHitEffect = GetTransform().Find("DeathLight_Attack_2_Z_Attack").gameObject;
			}
			m_laserBullet.SetActive(false);
			m_laserHitEffect.SetActive(false);
			m_laserReviseDistance = Vector3.Distance(Vector3.forward * m_laserPoint.position.z, Vector3.forward * m_laserBullet.transform.position.z);
			SetBullet();
			isBig = true;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.ZOMBIE_NURSE_DAMAGE_VENOM, 3);
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			Transform mix = m_transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine");
			DataConf.AnimData enemyAnim = DataCenter.Conf().GetEnemyAnim(enemyAnimTag, "Hurt");
			if (enemyAnim.count > 1)
			{
				for (int i = 0; i < enemyAnim.count; i++)
				{
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].AddMixingTransform(mix);
				}
			}
		}

		public new virtual void SetEnemy(Vector3 position, Quaternion rotation)
		{
			m_transform.position = position;
			m_transform.rotation = rotation;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (!Alive())
			{
				return;
			}
			if (!base.shootAble)
			{
				AIState currentAIState = GetCurrentAIState();
				if (currentAIState.name != "Shoot")
				{
					m_shootAbleTimer += Time.deltaTime;
					if (CheckUseSkill(ref m_shootAbleTimer))
					{
						base.shootAble = true;
						if (currentAIState.name != "Melee")
						{
							ChangeAIState("Shoot", false);
						}
					}
				}
			}
			if (m_laserRotate)
			{
				GetTransform().rotation = Quaternion.Lerp(m_laserStartRotation, m_laserTargetRotation, m_laserRotateTime / 3f);
				m_laserRotateTime += Time.deltaTime;
			}
		}

		private void AddAnimationEvents()
		{
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo result = new HitResultInfo();
			AIState currentAIState = GetCurrentAIState();
			if (currentAIState.name == "Born")
			{
				return result;
			}
			return base.OnHit(hitInfo);
		}

		public override void OnMelee(AIState.AIPhase phase)
		{
			base.OnMelee(phase);
			switch (phase)
			{
			case AIState.AIPhase.Update:
				if (AnimationPlaying(m_attackAnimName))
				{
					break;
				}
				goto case AIState.AIPhase.Exit;
			case AIState.AIPhase.Exit:
				if (base.shootAble)
				{
					if (Random.Range(0, 100) < 100)
					{
						ChangeAIState("Shoot", false);
						break;
					}
					base.shootAble = false;
					m_shootAbleTimer = 0f;
				}
				break;
			}
		}

		public override void OnSpecialAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				base.isRage = true;
				base.skillAttack = true;
				base.animLowerBody = GetAnimationName("Attack_Special_01_A");
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				m_laserPhase = AIState.AIPhase.Enter;
				base.currentSkill = base.skillInfos[0];
				SetSkillHitInfo();
				base.currentSkill = null;
				break;
			case AIState.AIPhase.Update:
				switch (m_laserPhase)
				{
				case AIState.AIPhase.Enter:
					if (base.lockedTarget != null)
					{
						LookAt(base.lockedTarget.GetTransform());
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						m_laserShootingTime = 0f;
						m_laserCharging = true;
						base.animLowerBody = GetAnimationName("Attack_Special_01_B");
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						m_laserPhase = AIState.AIPhase.Update;
						effectPlayManager.PlayEffect("Attack");
						effectPlayManager.PlayEffect("Attack_02");
					}
					break;
				case AIState.AIPhase.Update:
					m_laserShootingTime += Time.deltaTime;
					if (m_laserCharging)
					{
						if (base.lockedTarget != null && m_laserShootingTime <= 1f)
						{
							LookAt(base.lockedTarget.GetTransform());
						}
						if (m_laserShootingTime >= 1.5f)
						{
							m_laserCharging = false;
							if (Random.Range(0, 100) < 100)
							{
								m_laserRotate = true;
								m_laserRotateTime = 0f;
								m_laserStartRotation = GetTransform().rotation;
								m_laserTargetRotation = Quaternion.AngleAxis(m_laserStartRotation.eulerAngles.y + (float)((Random.Range(0, 100) >= 50) ? 90 : (-90)), Vector3.up);
							}
							m_laserBullet.SetActive(true);
							LaserShoot();
							m_laserAttackInterval = 0f;
						}
					}
					else
					{
						LaserShoot();
						if (m_laserShootingTime >= 4.5f)
						{
							SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
							m_laserBullet.SetActive(false);
							m_laserHitEffect.SetActive(false);
							base.animLowerBody = GetAnimationName("Attack_Special_01_C");
							AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
							m_laserPhase = AIState.AIPhase.Exit;
							effectPlayManager.StopEffect("Attack");
							effectPlayManager.StopEffect("Attack_02");
						}
					}
					break;
				case AIState.AIPhase.Exit:
					m_laserRotate = false;
					if (!AnimationPlaying(base.animLowerBody))
					{
						ChangeToDefaultAIState();
					}
					break;
				}
				break;
			case AIState.AIPhase.Exit:
				base.shootAble = false;
				base.skillAttack = false;
				base.isRage = false;
				break;
			}
		}

		protected virtual void SetBullet()
		{
			int index = ((base.enemyType != EnemyType.DeadLight) ? 37 : 29);
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(index);
			int num = 9;
			m_bulletBuffer = new DS2ObjectBuffer(num);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < num; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				if (gameObject2.GetComponent<LinearMoveToDestroy>() == null)
				{
					gameObject2.AddComponent<LinearMoveToDestroy>();
				}
				if (gameObject2.GetComponent<BulletTriggerScript>() == null)
				{
					gameObject2.AddComponent<BulletTriggerScript>();
				}
				bullet.hitInfo.damage = base.hitInfo.damage;
				bullet.attribute = new Bullet.BulletAttribute();
				bullet.attribute.speed = 6.5f;
				bullet.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bullet);
			}
		}

		public virtual Bullet GetBulletFromBuffer()
		{
			return m_bulletBuffer.GetObject() as Bullet;
		}

		public virtual void EmitBullet(float distanceLife = 0f)
		{
			effectPlayManager.PlayEffect("Fire");
			for (int i = -1; i < 2; i++)
			{
				Bullet bulletFromBuffer = GetBulletFromBuffer();
				bulletFromBuffer.GetGameObject().layer = 23;
				if (bulletFromBuffer != null)
				{
					Quaternion rotation = Quaternion.AngleAxis(m_shootPoint.eulerAngles.y + (float)(30 * i), Vector3.up);
					bulletFromBuffer.SetBullet(this, null, m_shootPoint.position, rotation);
					bulletFromBuffer.Emit(distanceLife);
				}
			}
		}

		private void GunShoot()
		{
			EmitBullet(m_fBulletLife);
		}

		private void LaserShoot()
		{
			float num = 50f + m_laserReviseDistance;
			float num2 = num;
			RaycastHit raycastHit;
			if (Physics.Raycast(m_laserPoint.position, m_laserPoint.forward, out raycastHit, num, 327680))
			{
				num = Vector3.Distance(m_laserPoint.position, raycastHit.point);
				num2 = num - m_laserReviseDistance;
				if (num2 <= 0f)
				{
					num2 = 0f;
					m_laserHitEffect.SetActive(false);
				}
				else
				{
					m_laserHitEffect.transform.position = new Vector3(raycastHit.point.x, m_laserBullet.transform.position.y, raycastHit.point.z);
					m_laserHitEffect.SetActive(true);
				}
			}
			else
			{
				m_laserHitEffect.SetActive(false);
			}
			m_laserBullet.transform.localScale = new Vector3(1f, 1f, num2 / 12f);
			m_laserAttackInterval -= Time.deltaTime;
			if (m_laserAttackInterval <= 0f)
			{
				SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
				SetAttackCollider(true, AttackCollider.AttackColliderType.Dash);
				m_laserAttackInterval = 0.5f;
			}
		}
	}
}
