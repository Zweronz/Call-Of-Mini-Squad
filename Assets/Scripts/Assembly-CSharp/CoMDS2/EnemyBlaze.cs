using UnityEngine;

namespace CoMDS2
{
	public class EnemyBlaze : Enemy
	{
		private enum FireShieldState
		{
			Ready = 0,
			Use = 1,
			During = 2,
			CD = 3
		}

		protected DS2ObjectBuffer m_bulletBuffer;

		private float m_fBulletLife = 15f;

		private AIState.AIPhase m_skillPhase;

		private float m_useSkillTimer;

		private int m_useSkillCount;

		private Quaternion m_flameStartRotation;

		private Quaternion m_flameTargetRotation;

		private float m_flameKeepTime;

		private Transform m_flamePoint;

		private ParticleSystem[] m_flamePS;

		private float[] m_flamePSLifeTime;

		private Transform m_fireballPoint;

		private FireShieldState m_fireShieldState = FireShieldState.CD;

		private float m_fireShieldCD = 25f;

		private float m_fireShieldDuring = 10f;

		private float m_fireShieldDeltaTime;

		private BlazeFireShield m_fireShield;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			AIStateTimerSkill aIStateTimerSkill = new AIStateTimerSkill(this, "TimerSkill");
			base.meleeRange = (m_baseMeleeRange = 1.3f);
			SetAttackCollider(false);
			base.meleeRange = (m_baseMeleeRange = 6f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			AddAIState(aIStateTimerSkill.name, aIStateTimerSkill);
			base.shootRange = 8f;
			base.shootAble = false;
			SetBullet();
			m_fireballPoint = GetTransform().Find("FireBallPoint");
			m_flamePoint = GetTransform().Find("Effects").Find("Fire_02");
			m_flamePS = m_flamePoint.GetComponentsInChildren<ParticleSystem>(true);
			m_flamePSLifeTime = new float[m_flamePS.Length];
			for (int i = 0; i < m_flamePS.Length; i++)
			{
				m_flamePSLifeTime[i] = m_flamePS[i].startLifetime;
			}
			m_fireShield = ((GameObject)Object.Instantiate(Resources.Load<GameObject>("Models/ActiveObjects/Blaze/BlazeFireShield"))).GetComponent<BlazeFireShield>();
			m_fireShield.hostTransform = GetTransform();
			AttackColliderSimple[] componentsInChildren = m_fireShield.transform.GetComponentsInChildren<AttackColliderSimple>();
			HitInfo hitInfo = new HitInfo(base.hitInfo);
			hitInfo.repelDistance = new NumberSection<float>(0f, 0f);
			hitInfo.repelTime = 0f;
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].hitInfo = hitInfo;
				componentsInChildren[j].belong = GetGameObject();
			}
			m_fireShield.rotatePoint.gameObject.SetActive(false);
			m_fireShieldDeltaTime = m_fireShieldCD;
			isBig = true;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.NINJIA_ATTACK_FIRE_BULLET, 3);
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
			m_fireShieldDeltaTime += Time.deltaTime;
			switch (m_fireShieldState)
			{
			case FireShieldState.Ready:
				SwitchFSM(GetAIState("Idle"));
				SwitchFSM(GetAIState("TimerSkill"));
				base.animLowerBody = GetAnimationName("Attack_Special_01_A");
				AnimationCrossFade(base.animLowerBody, false);
				effectPlayManager.PlayEffect("Burn");
				m_fireShieldState = FireShieldState.Use;
				break;
			case FireShieldState.Use:
				if (!AnimationPlaying(base.animLowerBody))
				{
					m_fireShieldState = FireShieldState.During;
					m_fireShieldDeltaTime = 0f;
					SwitchFSM(GetAIState("Chase"));
				}
				break;
			case FireShieldState.During:
				if (m_fireShieldDeltaTime > m_fireShieldDuring)
				{
					m_fireShield.rotatePoint.gameObject.SetActive(false);
					m_fireShieldDeltaTime = 0f;
					m_fireShieldState = FireShieldState.CD;
				}
				break;
			case FireShieldState.CD:
				if (m_fireShieldDeltaTime >= m_fireShieldCD)
				{
					AIState currentAIState = GetCurrentAIState();
					if (currentAIState.name != "Shoot" && currentAIState.name != "Melee")
					{
						m_fireShieldState = FireShieldState.Ready;
					}
				}
				break;
			}
			if (base.shootAble)
			{
				return;
			}
			AIState currentAIState2 = GetCurrentAIState();
			if (!(currentAIState2.name != "Shoot") || !(currentAIState2.name != "TimerSkill"))
			{
				return;
			}
			m_shootAbleTimer += Time.deltaTime;
			if (CheckUseSkill(ref m_shootAbleTimer))
			{
				base.shootAble = true;
				if (currentAIState2.name != "Melee")
				{
					ChangeAIState("Shoot", false);
				}
			}
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

		public override void OnDeath()
		{
			base.OnDeath();
			m_fireShield.rotatePoint.gameObject.SetActive(false);
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
					ChangeAIState("Shoot", false);
				}
				break;
			}
		}

		public override void OnSpecialAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				int skillId = Random.Range(0, base.skillInfos.Count);
				UseSkill(skillId);
				base.isRage = true;
				break;
			}
			case AIState.AIPhase.Update:
				if (base.currentSkill == null && m_releaseSkillState == ReleaseSkillState.Over)
				{
					ChangeToDefaultAIState();
				}
				break;
			case AIState.AIPhase.Exit:
				base.shootAble = false;
				base.isRage = false;
				m_skillTimer = 0f;
				SetGodTime(0f);
				break;
			}
		}

		public override void OnReleaseSkillToBegin()
		{
			m_releaseSkillState = ReleaseSkillState.Ready;
			base.isRage = true;
			if (base.lockedTarget != null)
			{
				m_skillTarget = base.lockedTarget;
			}
			else
			{
				m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
			}
			if (base.currentSkill.type == SkillType.Dash)
			{
				m_useSkillCount = 2;
			}
		}

		public override void OnReleaseSkillBegin()
		{
			if (m_releaseSkillState == ReleaseSkillState.Ready)
			{
				switch (base.currentSkill.type)
				{
				case SkillType.Jump:
					m_skillPhase = AIState.AIPhase.Enter;
					m_useSkillTimer = 0f;
					base.animLowerBody = base.currentSkill.animReady;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					effectPlayManager.PlayEffect("AttackIndicate");
					OnReleaseSkillToUpdate();
					effectPlayManager.PlayEffect("Power");
					break;
				case SkillType.Dash:
					m_skillPhase = AIState.AIPhase.Enter;
					m_useSkillTimer = 0f;
					effectPlayManager.PlayEffect("AttackIndicate");
					OnReleaseSkillToUpdate();
					break;
				}
			}
		}

		public override void OnReleaseSkillToUpdate()
		{
			m_releaseSkillState = ReleaseSkillState.Release;
			SetSkillHitInfo();
		}

		public override void OnReleaseSkillUpdate()
		{
			if (m_releaseSkillState != ReleaseSkillState.Release)
			{
				return;
			}
			switch (base.currentSkill.type)
			{
			case SkillType.Jump:
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					m_useSkillTimer += Time.deltaTime;
					if (m_useSkillTimer < 0.6f && m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						effectPlayManager.PlayEffect("Fire_02");
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
						m_useSkillTimer = 0f;
						m_flameStartRotation = GetTransform().rotation;
						m_flameTargetRotation = Quaternion.AngleAxis(m_flameStartRotation.eulerAngles.y + ((Random.Range(0, 100) >= 50) ? base.currentSkill.time : (0f - base.currentSkill.time)), Vector3.up);
						m_flameKeepTime = base.currentSkill.time / base.currentSkill.speed;
					}
					ChangeColor(Util.s_color_bleeding);
					GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_C);
					break;
				case AIState.AIPhase.Update:
					m_useSkillTimer += Time.deltaTime;
					FlameFire();
					if (m_useSkillTimer > 0.1f)
					{
						SetAttackCollider(true);
					}
					if (m_useSkillTimer >= m_flameKeepTime)
					{
						base.animLowerBody = base.currentSkill.animEnd;
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Exit;
						effectPlayManager.StopEffect("Fire_02");
						SetAttackCollider(false);
					}
					else
					{
						GetTransform().rotation = Quaternion.Lerp(m_flameStartRotation, m_flameTargetRotation, m_useSkillTimer / m_flameKeepTime);
					}
					break;
				case AIState.AIPhase.Exit:
					if (!AnimationPlaying(base.animLowerBody))
					{
						OnReleaseSkillToEnd();
						ResetColor();
					}
					break;
				}
				break;
			case SkillType.Dash:
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					m_useSkillTimer = 0f;
					base.animLowerBody = base.currentSkill.animReady;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					effectPlayManager.PlayEffect("Fire");
					m_skillPhase = AIState.AIPhase.Update;
					ChangeColor(Util.s_color_bleeding);
					GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_C);
					break;
				case AIState.AIPhase.Update:
					m_useSkillTimer += Time.deltaTime;
					if (m_useSkillTimer < 1f && m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						effectPlayManager.StopEffect("Fire");
						m_useSkillCount--;
						if (m_useSkillCount > 0)
						{
							m_skillPhase = AIState.AIPhase.Enter;
							break;
						}
						OnReleaseSkillToEnd();
						ResetColor();
					}
					break;
				}
				break;
			}
		}

		public override void OnReleaseSkillToEnd()
		{
			m_releaseSkillState = ReleaseSkillState.End;
		}

		public override void OnReleaseSkillEnd()
		{
			m_releaseSkillState = ReleaseSkillState.Over;
			base.currentSkill = null;
			base.isRage = false;
			SkillEnd();
		}

		protected virtual void SetBullet()
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(32);
			int num = 8;
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
				bullet.attribute.speed = 7f;
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
			for (int i = 0; i < 4; i++)
			{
				Bullet bulletFromBuffer = GetBulletFromBuffer();
				bulletFromBuffer.GetGameObject().layer = 23;
				if (bulletFromBuffer != null)
				{
					bulletFromBuffer.hitInfo = GetHitInfo();
					Quaternion rotation = Quaternion.AngleAxis(m_fireballPoint.eulerAngles.y - 45f + (float)(30 * i), Vector3.up);
					bulletFromBuffer.SetBullet(this, null, m_fireballPoint.position, rotation);
					bulletFromBuffer.Emit(m_fBulletLife);
				}
			}
		}

		private void FireBallShoot()
		{
			EmitBullet(m_fBulletLife);
		}

		private void FlameFire()
		{
			float attackRange = base.currentSkill.attackRange;
			float num = 1f;
			RaycastHit raycastHit;
			if (Physics.Raycast(m_flamePoint.position, m_flamePoint.forward, out raycastHit, attackRange, 327680))
			{
				attackRange = Vector3.Distance(m_flamePoint.position, raycastHit.point);
				num = attackRange / base.currentSkill.attackRange;
			}
			m_flamePoint.transform.localScale = Vector3.one * num;
			for (int i = 0; i < m_flamePS.Length; i++)
			{
				m_flamePS[i].startLifetime = m_flamePSLifeTime[i] * num;
			}
		}

		private void TriggerFireShield()
		{
			m_fireShield.rotatePoint.gameObject.SetActive(true);
		}
	}
}
