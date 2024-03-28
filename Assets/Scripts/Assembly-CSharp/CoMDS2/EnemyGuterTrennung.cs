using UnityEngine;

namespace CoMDS2
{
	public class EnemyGuterTrennung : Enemy
	{
		private AIState.AIPhase m_skillPhase;

		private float m_useSkillTimer;

		private int m_useSkillCount;

		private bool m_frenzyBreak;

		private Vector3 m_dashLastPos;

		private GameObject m_finGameObject;

		private EffectControl m_effectSwimStart;

		private EffectControl m_effectSwimEnd;

		private GameObject m_effectFloor;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateChase aIStateChase = (AIStateChase)GetAIState("Chase");
			aIStateChase.animStartMoveName = GetAnimationName("StartMove");
			aIStateChase.animStopMoveName = GetAnimationName("StopMove");
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			base.meleeRange = (m_baseMeleeRange = 3f);
			SetAttackCollider(false);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 8f;
			base.shootAble = false;
			RemoveAIState("Chase");
			m_finGameObject = GetTransform().Find("Fin").gameObject;
			m_finGameObject.SetActive(false);
			m_effectSwimStart = GetTransform().Find("SwimEffectStart").GetComponentInChildren<EffectControl>();
			m_effectSwimStart.Root.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			m_effectSwimStart.StopEmit();
			m_effectSwimEnd = GetTransform().Find("SwimEffectEnd").GetComponentInChildren<EffectControl>();
			m_effectSwimEnd.Root.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			m_effectSwimEnd.StopEmit();
			m_effectFloor = m_transform.Find("Dummy_All/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/Bip01 Prop1/Weapon_Dummy_All/Weapon_Bone03/Fin_Bone03/floor").gameObject;
			m_effectFloor.SetActive(false);
			isBig = true;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.laceration_DOT);
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			Transform mix = m_transform.Find("Dummy_All/Bip01/Bip01 Pelvis/Bip01 Spine");
			DataConf.AnimData enemyAnim = DataCenter.Conf().GetEnemyAnim(enemyAnimTag, "Hurt");
			if (enemyAnim.count > 1)
			{
				for (int i = 0; i < enemyAnim.count; i++)
				{
					m_gameObject.GetComponent<Animation>()[AnimationHelper.TryGetAnimationName(m_gameObject.GetComponent<Animation>(), enemyAnim.name + "0" + (i + 1))].layer = 2;
					m_gameObject.GetComponent<Animation>()[AnimationHelper.TryGetAnimationName(m_gameObject.GetComponent<Animation>(), enemyAnim.name + "0" + (i + 1))].AddMixingTransform(mix);
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
			AIState currentAIState = GetCurrentAIState();
			if (currentAIState.name == "Chase" && !AnimationPlaying(base.animUpperBody))
			{
				m_effectFloor.SetActive(true);
			}
			else
			{
				m_effectFloor.SetActive(false);
			}
			if (Alive() && currentAIState.name != "Shoot")
			{
				m_skillTimer += Time.deltaTime;
				if (CheckUseSkill(ref m_skillTimer))
				{
					base.shootAble = true;
				}
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
			if (phase == AIState.AIPhase.Update && !AnimationPlaying(m_attackAnimName) && base.shootAble)
			{
				ChangeAIState("Shoot", false);
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
			switch (base.currentSkill.type)
			{
			case SkillType.Jump:
				m_useSkillCount = 3;
				break;
			case SkillType.Dash:
			case SkillType.Grab:
				m_useSkillCount = 2;
				break;
			case SkillType.Throw:
				break;
			}
		}

		public override void OnReleaseSkillBegin()
		{
			if (m_releaseSkillState == ReleaseSkillState.Ready)
			{
				switch (base.currentSkill.type)
				{
				case SkillType.Dash:
				case SkillType.Jump:
					m_skillPhase = AIState.AIPhase.Enter;
					m_useSkillTimer = 0f;
					m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
					base.animLowerBody = base.currentSkill.animReady;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					effectPlayManager.PlayEffect("AttackIndicate");
					OnReleaseSkillToUpdate();
					break;
				case SkillType.Throw:
				case SkillType.Grab:
					m_skillPhase = AIState.AIPhase.Enter;
					m_useSkillTimer = 0f;
					base.animLowerBody = base.currentSkill.animReady;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					OnReleaseSkillToUpdate();
					m_effectSwimStart.transform.position = GetTransform().position;
					m_effectSwimStart.StartEmit();
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
					if (m_useSkillTimer < 1f && m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						effectPlayManager.PlayEffect("Burn");
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
					}
					break;
				case AIState.AIPhase.Update:
					if (m_characterController != null)
					{
						m_characterController.Move(GetModelTransform().forward * base.currentSkill.speed * Time.deltaTime);
					}
					if (AnimationPlaying(base.animLowerBody))
					{
						break;
					}
					m_useSkillCount--;
					if (m_useSkillCount > 0)
					{
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						if (m_skillTarget == null)
						{
							m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
						}
						if (m_skillTarget != null)
						{
							GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
						}
					}
					else
					{
						base.animLowerBody = base.currentSkill.animEnd;
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_frenzyBreak = false;
						m_skillPhase = AIState.AIPhase.Exit;
					}
					break;
				case AIState.AIPhase.Exit:
					if (m_characterController != null && !m_frenzyBreak)
					{
						m_characterController.Move(GetModelTransform().forward * base.currentSkill.speed * Time.deltaTime);
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						effectPlayManager.StopEffect("Burn");
						OnReleaseSkillToEnd();
					}
					break;
				}
				break;
			case SkillType.Dash:
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					m_useSkillTimer += Time.deltaTime;
					if (m_useSkillTimer < 1f && m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					if (!AnimationPlaying(base.animLowerBody))
					{
						m_useSkillTimer = 0f;
						m_dashLastPos = GetTransform().position;
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						effectPlayManager.PlayEffect("Dash");
						m_skillPhase = AIState.AIPhase.Update;
						MakeBleeding();
					}
					break;
				case AIState.AIPhase.Update:
				{
					SetAttackCollider(true, AttackCollider.AttackColliderType.Dash);
					if (!(m_characterController != null))
					{
						break;
					}
					float num3 = base.currentSkill.speed * Time.deltaTime;
					m_useSkillTimer += num3;
					m_characterController.Move(GetModelTransform().forward * num3);
					Vector3 position2 = GetTransform().position;
					if (m_useSkillTimer >= 12f || Vector3.Distance(position2, m_dashLastPos) < 0.1f)
					{
						m_useSkillCount--;
						if (m_useSkillCount > 0)
						{
							m_useSkillTimer = 0f;
							GetTransform().Rotate(Vector3.up * 180f);
						}
						else
						{
							effectPlayManager.StopEffect("Dash");
							SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
							base.animLowerBody = base.currentSkill.animEnd;
							AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
							m_frenzyBreak = false;
							m_skillPhase = AIState.AIPhase.Exit;
						}
					}
					m_dashLastPos = position2;
					break;
				}
				case AIState.AIPhase.Exit:
					if (!AnimationPlaying(base.animLowerBody))
					{
						OnReleaseSkillToEnd();
					}
					break;
				}
				break;
			case SkillType.Throw:
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					if (!AnimationPlaying(base.animLowerBody))
					{
						m_useSkillTimer = 0f;
						m_dashLastPos = GetTransform().position;
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
						effectPlayManager.PlayEffect("Thud");
						if (m_skillTarget != null)
						{
							GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
						}
						MakeBleeding();
					}
					break;
				case AIState.AIPhase.Update:
					SetAttackCollider(true, AttackCollider.AttackColliderType.Thud);
					if (m_characterController != null)
					{
						float num2 = base.currentSkill.speed * Time.deltaTime;
						m_characterController.Move(GetModelTransform().forward * num2);
						Vector3 position = GetTransform().position;
						m_useSkillTimer += Time.deltaTime;
						if (m_useSkillTimer >= 5f)
						{
							effectPlayManager.StopEffect("Thud");
							SetAttackCollider(false, AttackCollider.AttackColliderType.Thud);
							base.animLowerBody = base.currentSkill.animEnd;
							AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
							m_frenzyBreak = false;
							m_skillPhase = AIState.AIPhase.Exit;
						}
						else if (Vector3.Distance(position, m_dashLastPos) < 0.1f)
						{
							GetTransform().Rotate(Vector3.up * 150f);
						}
						m_dashLastPos = position;
					}
					break;
				case AIState.AIPhase.Exit:
					if (!AnimationPlaying(base.animLowerBody))
					{
						OnReleaseSkillToEnd();
					}
					break;
				}
				break;
			case SkillType.Grab:
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					if (!AnimationPlaying(base.animLowerBody))
					{
						m_useSkillTimer = 0f;
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
						m_skillTarget = GameBattle.m_instance.GetRandomObjFromTargetList(this);
						if (null != m_characterController)
						{
							m_characterController.enabled = false;
						}
						m_finGameObject.SetActive(true);
						Visible = false;
					}
					break;
				case AIState.AIPhase.Update:
				{
					float num = base.currentSkill.speed * Time.deltaTime;
					GetTransform().position = GetTransform().position + GetTransform().forward * num;
					if (m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					m_useSkillTimer += Time.deltaTime;
					if (m_useSkillTimer >= 4f || m_skillTarget == null || (m_skillTarget != null && Vector3.Distance(m_skillTarget.GetTransform().position, GetTransform().position) < 2f))
					{
						base.animLowerBody = base.currentSkill.animEnd;
						effectPlayManager.PlayEffect("Fire");
						effectPlayManager.PlayEffect("Fire_02");
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Exit;
						if (null != m_characterController)
						{
							m_characterController.enabled = true;
						}
						m_finGameObject.SetActive(false);
						Visible = true;
						m_effectSwimEnd.transform.position = GetTransform().position;
						m_effectSwimEnd.StartEmit();
					}
					break;
				}
				case AIState.AIPhase.Exit:
					if (!AnimationPlaying(base.animLowerBody))
					{
						OnReleaseSkillToEnd();
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

		private void FrenzyBreak()
		{
			m_frenzyBreak = true;
		}

		private void DrillOut()
		{
			m_useSkillCount--;
			if (m_useSkillCount > 0)
			{
				m_releaseSkillState = ReleaseSkillState.Ready;
				OnReleaseSkillBegin();
			}
		}

		private void MakeBleeding()
		{
			HitInfo hitInfo = GetHitInfo();
			if (Random.Range(0, 100) < 100)
			{
				Buff item = new Buff(Buff.AffectType.Bleeding, (float)(-Random.Range(2, 6)) * 0.01f, 0.5f, 5f, Buff.CalcType.Percentage, 0f);
				hitInfo.buffs.Add(item);
			}
			else
			{
				hitInfo.buffs.Clear();
			}
		}
	}
}
