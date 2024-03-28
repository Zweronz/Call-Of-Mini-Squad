using UnityEngine;

namespace CoMDS2
{
	public class EnemyButcher : Enemy
	{
		private AIState.AIPhase m_skillPhase;

		private float m_useSkillTimer;

		private int m_useSkillCount;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
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
			isBig = true;
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
			if (!Alive())
			{
				return;
			}
			AIState currentAIState = GetCurrentAIState();
			if (currentAIState.name != "Shoot")
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
			if (base.currentSkill.type == SkillType.Dash)
			{
				m_useSkillCount = 2;
			}
		}

		public override void OnReleaseSkillBegin()
		{
			if (m_releaseSkillState != 0)
			{
				return;
			}
			SkillType type = base.currentSkill.type;
			if (type == SkillType.Dash || type == SkillType.Jump)
			{
				m_skillPhase = AIState.AIPhase.Enter;
				m_useSkillTimer = 0f;
				m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
				base.animLowerBody = base.currentSkill.animReady;
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				if (base.currentSkill.type == SkillType.Dash)
				{
					effectPlayManager.PlayEffect("AttackIndicate");
				}
				OnReleaseSkillToUpdate();
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
					if (!AnimationPlaying(base.animLowerBody))
					{
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
						effectPlayManager.PlayEffect("Attack_02");
						SetAttackCollider(true, AttackCollider.AttackColliderType.Thud);
					}
					break;
				case AIState.AIPhase.Update:
					m_useSkillTimer += Time.deltaTime;
					if (m_characterController != null)
					{
						m_characterController.Move(GetModelTransform().forward * base.currentSkill.speed * Time.deltaTime);
					}
					if (m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					if (m_useSkillTimer > base.currentSkill.time)
					{
						SetAttackCollider(false, AttackCollider.AttackColliderType.Thud);
						effectPlayManager.StopEffect("Attack_02");
						base.animLowerBody = base.currentSkill.animEnd;
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Exit;
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
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						effectPlayManager.PlayEffect("Dash");
						m_skillPhase = AIState.AIPhase.Update;
					}
					break;
				case AIState.AIPhase.Update:
					SetAttackCollider(true, AttackCollider.AttackColliderType.Dash);
					if (m_characterController != null)
					{
						float num = base.currentSkill.speed * Time.deltaTime;
						m_useSkillTimer += num;
						m_characterController.Move(GetModelTransform().forward * num);
						if (m_useSkillTimer >= base.currentSkill.time)
						{
							SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
							m_skillPhase = AIState.AIPhase.Exit;
							effectPlayManager.StopEffect("Dash");
							effectPlayManager.PlayEffect("Fire");
							base.animLowerBody = base.currentSkill.animEnd;
							AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						}
					}
					break;
				case AIState.AIPhase.Exit:
					if (AnimationPlaying(base.animLowerBody))
					{
						break;
					}
					m_useSkillCount--;
					if (m_useSkillCount > 0)
					{
						m_useSkillTimer = 0f;
						if (m_skillTarget == null)
						{
							m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
						}
						if (m_skillTarget != null)
						{
							GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
						}
						base.animLowerBody = base.currentSkill.animProcess;
						AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
						effectPlayManager.PlayEffect("Dash");
						m_skillPhase = AIState.AIPhase.Update;
					}
					else
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

		private void DrillOut()
		{
			m_useSkillCount--;
			if (m_useSkillCount > 0)
			{
				m_releaseSkillState = ReleaseSkillState.Ready;
				OnReleaseSkillBegin();
			}
		}
	}
}
