using UnityEngine;

namespace CoMDS2
{
	public class EnemyPestilenceJar : Enemy
	{
		private enum MiasmaState
		{
			Ready = 0,
			Use = 1,
			CD = 2
		}

		protected DS2ObjectBuffer m_bulletBuffer;

		private float m_fBulletLife = 15f;

		private int m_useSkillCount = 5;

		private Transform m_shootPoint;

		private MiasmaState m_miasmaState;

		private float m_miasmaCD = 25f;

		private float m_miasmaDeltaTime;

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
			base.meleeRange = (m_baseMeleeRange = 10f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			AddAIState(aIStateTimerSkill.name, aIStateTimerSkill);
			base.shootRange = 10f;
			base.shootAble = false;
			Buff item = new Buff(Buff.AffectType.MoveSpeed, -0.3f, 2f, 2f, Buff.CalcType.Percentage, 0f);
			base.hitInfo.buffs.Add(item);
			base.skillHitInfo.buffs.Add(item);
			SetBullet();
			m_shootPoint = GetTransform().Find("ShootPoint");
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.pest_Attack_Attack, 10);
			m_miasmaDeltaTime = m_miasmaCD;
			isBig = true;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			if (null == BattleBufferManager.Instance.m_miasmaUI)
			{
				BattleBufferManager.Instance.m_miasmaUI = Object.Instantiate(Resources.Load("Game/MiasmaUI")) as GameObject;
				BattleBufferManager.Instance.m_miasmaCoverOn = BattleBufferManager.Instance.m_miasmaUI.transform.Find("Camera/CoverOn").gameObject.GetComponentInChildren<EffectControl>();
				BattleBufferManager.Instance.m_miasmaCoverEnd = BattleBufferManager.Instance.m_miasmaUI.transform.Find("Camera/CoverEnd").gameObject.GetComponentInChildren<EffectControl>();
				BattleBufferManager.Instance.m_miasmaCoverOn.Root.SetActive(true);
				BattleBufferManager.Instance.m_miasmaCoverEnd.gameObject.SetActive(false);
				BattleBufferManager.Instance.m_miasmaUI.SetActive(false);
			}
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
			m_miasmaDeltaTime += Time.deltaTime;
			switch (m_miasmaState)
			{
			case MiasmaState.Ready:
				SwitchFSM(GetAIState("Idle"));
				SwitchFSM(GetAIState("TimerSkill"));
				base.animLowerBody = GetAnimationName("Attack_Special_01_A");
				AnimationCrossFade(base.animLowerBody, false);
				m_miasmaState = MiasmaState.Use;
				break;
			case MiasmaState.Use:
				if (!AnimationPlaying(base.animLowerBody))
				{
					m_miasmaState = MiasmaState.CD;
					m_miasmaDeltaTime = 0f;
					SwitchFSM(GetAIState("Chase"));
				}
				break;
			case MiasmaState.CD:
				if (m_miasmaDeltaTime >= m_miasmaCD)
				{
					AIState currentAIState = GetCurrentAIState();
					if (currentAIState.name != "Shoot" && currentAIState.name != "Melee")
					{
						m_miasmaState = MiasmaState.Ready;
					}
				}
				break;
			}
			if (base.shootAble)
			{
				return;
			}
			AIState currentAIState2 = GetCurrentAIState();
			if (currentAIState2.name != "Shoot" && currentAIState2.name != "TimerSkill")
			{
				m_shootAbleTimer += Time.deltaTime;
				if (CheckUseSkill(ref m_shootAbleTimer))
				{
					base.shootAble = true;
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

		public override void OnMelee(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				m_useSkillCount = 5;
				IPathFinding pathFinding = GetPathFinding();
				if (pathFinding != null && pathFinding.HasNavigation())
				{
					pathFinding.StopNav();
				}
				m_attackAnimName = GetAnimationName("Attack");
				AnimationStop(base.animUpperBody);
				int num = Random.Range(0, 100);
				if ((float)num < m_proFirstAttackLose)
				{
					m_waitAttackAnimName = GetAnimationName("Idle");
					AnimationCrossFade(m_waitAttackAnimName, true);
				}
				else
				{
					AnimationCrossFade(m_attackAnimName, false);
				}
				m_attackTimer = 0f;
				break;
			}
			case AIState.AIPhase.Update:
			{
				DS2ActiveObject nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
				if (nearestObjFromTargetList != null)
				{
					LookAt(nearestObjFromTargetList.GetTransform());
					float num2 = base.meleeRange * base.meleeRange;
					float sqrMagnitude = (nearestObjFromTargetList.GetTransform().position - GetTransform().position).sqrMagnitude;
					if (sqrMagnitude > num2 && !AnimationPlaying(m_attackAnimName))
					{
						m_useSkillCount--;
						if (m_useSkillCount <= 0)
						{
							if (base.shootAble)
							{
								ChangeAIState("Shoot", false);
							}
							else
							{
								ChangeAIState("Chase");
							}
							break;
						}
						AnimationCrossFade(m_attackAnimName, false);
					}
				}
				m_attackTimer += Time.deltaTime;
				if (m_attackTimer >= m_attFrequency)
				{
					if (AnimationPlaying(m_attackAnimName))
					{
						break;
					}
					m_useSkillCount--;
					if (m_useSkillCount == 0)
					{
						m_useSkillCount = 5;
						m_attackTimer = 0f;
						if (base.shootAble)
						{
							ChangeAIState("Shoot", false);
						}
					}
					else
					{
						m_attackAnimName = GetAnimationName("Attack");
						AnimationPlay(m_attackAnimName, false);
					}
				}
				else
				{
					if (AnimationPlaying(m_attackAnimName))
					{
						break;
					}
					m_useSkillCount--;
					if (m_useSkillCount == 0)
					{
						m_useSkillCount = 5;
						if (base.shootAble)
						{
							ChangeAIState("Shoot", false);
							break;
						}
						m_waitAttackAnimName = GetAnimationName("Idle");
						AnimationCrossFade(m_waitAttackAnimName, true);
					}
					else
					{
						m_attackAnimName = GetAnimationName("Attack");
						AnimationPlay(m_attackAnimName, false);
					}
				}
				break;
			}
			case AIState.AIPhase.Exit:
				if (base.shootAble)
				{
					ChangeAIState("Shoot", false);
				}
				SetAttackCollider(false);
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
		}

		public override void OnReleaseSkillBegin()
		{
			if (m_releaseSkillState == ReleaseSkillState.Ready)
			{
				switch (base.currentSkill.type)
				{
				case SkillType.Dash:
				case SkillType.Throw:
					base.animLowerBody = base.currentSkill.animReady;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					OnReleaseSkillToUpdate();
					break;
				case SkillType.Jump:
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
			case SkillType.Dash:
			case SkillType.Throw:
				if (!AnimationPlaying(base.animLowerBody))
				{
					OnReleaseSkillToEnd();
				}
				break;
			case SkillType.Jump:
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
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(33);
			int num = 15;
			m_bulletBuffer = new DS2ObjectBuffer(num);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < num; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				if (gameObject2.GetComponent<BulletTriggerScript>() == null)
				{
					gameObject2.AddComponent<BulletTriggerScript>();
				}
				bullet.hitInfo.damage = base.hitInfo.damage;
				bullet.attribute = new Bullet.BulletAttribute();
				bullet.attribute.speed = 8f;
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
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			bulletFromBuffer.GetGameObject().layer = 23;
			if (bulletFromBuffer != null)
			{
				bulletFromBuffer.hitInfo = GetHitInfo();
				bulletFromBuffer.SetBullet(this, null, m_shootPoint.position, m_shootPoint.rotation);
				if (bulletFromBuffer.GetGameObject().GetComponent<LinearMoveToDestroy>() == null)
				{
					bulletFromBuffer.GetGameObject().AddComponent<LinearMoveToDestroy>();
				}
				if (bulletFromBuffer.GetGameObject().GetComponent<HomingMoveToDestroy>() != null)
				{
					Object.Destroy(bulletFromBuffer.GetGameObject().GetComponent<HomingMoveToDestroy>());
				}
				bulletFromBuffer.attribute.speed = 10f;
				bulletFromBuffer.Emit(m_fBulletLife);
			}
		}

		private void GasBallShoot()
		{
			effectPlayManager.PlayEffect("Fire");
			EmitBullet(m_fBulletLife);
		}

		private void RoundShoot()
		{
			Vector3 position = GetTransform().position + m_shootPoint.position.y * Vector3.up;
			for (int i = 0; i < 8; i++)
			{
				Bullet bulletFromBuffer = GetBulletFromBuffer();
				bulletFromBuffer.GetGameObject().layer = 23;
				if (bulletFromBuffer != null)
				{
					bulletFromBuffer.hitInfo = GetHitInfo();
					bulletFromBuffer.SetBullet(this, null, position, Quaternion.AngleAxis(GetTransform().rotation.eulerAngles.y + (float)(45 * i), Vector3.up));
					if (bulletFromBuffer.GetGameObject().GetComponent<LinearMoveToDestroy>() == null)
					{
						bulletFromBuffer.GetGameObject().AddComponent<LinearMoveToDestroy>();
					}
					if (bulletFromBuffer.GetGameObject().GetComponent<HomingMoveToDestroy>() != null)
					{
						Object.Destroy(bulletFromBuffer.GetGameObject().GetComponent<HomingMoveToDestroy>());
					}
					bulletFromBuffer.attribute.speed = 10f;
					bulletFromBuffer.Emit(m_fBulletLife);
				}
			}
		}

		private void RoundHomingShoot()
		{
			Vector3 position = GetTransform().position + m_shootPoint.position.y * Vector3.up;
			for (int i = 0; i < 8; i++)
			{
				Bullet bulletFromBuffer = GetBulletFromBuffer();
				bulletFromBuffer.GetGameObject().layer = 23;
				if (bulletFromBuffer != null)
				{
					bulletFromBuffer.hitInfo = GetHitInfo();
					bulletFromBuffer.SetBullet(this, null, position, Quaternion.AngleAxis(GetTransform().rotation.eulerAngles.y + (float)(45 * i), Vector3.up));
					if (bulletFromBuffer.GetGameObject().GetComponent<HomingMoveToDestroy>() == null)
					{
						bulletFromBuffer.GetGameObject().AddComponent<HomingMoveToDestroy>();
					}
					if (bulletFromBuffer.GetGameObject().GetComponent<LinearMoveToDestroy>() != null)
					{
						Object.Destroy(bulletFromBuffer.GetGameObject().GetComponent<LinearMoveToDestroy>());
					}
					GameObject target = null;
					DS2ActiveObject randomObjFromTargetList = GameBattle.m_instance.GetRandomObjFromTargetList(this);
					if (randomObjFromTargetList != null)
					{
						target = randomObjFromTargetList.GetGameObject();
					}
					bulletFromBuffer.attribute.speed = 6f;
					bulletFromBuffer.EmitHoming(target, 0.5f, 3f, m_fBulletLife * 3f);
				}
			}
		}

		private void EmitMiasma()
		{
			DS2ActiveObject[] playerList = GameBattle.m_instance.GetPlayerList();
			DS2ActiveObject[] array = playerList;
			foreach (DS2ActiveObject dS2ActiveObject in array)
			{
				dS2ActiveObject.GetBuffManager().AddBuff(new Buff(Buff.AffectType.Miasma, 0f, 10f, 10f, Buff.CalcType.Percentage, 0f));
				dS2ActiveObject.GetBuffManager().AddBuff(new Buff(Buff.AffectType.HitRate, -0.3f, 10f, 10f, Buff.CalcType.Percentage, 0f));
			}
		}
	}
}
