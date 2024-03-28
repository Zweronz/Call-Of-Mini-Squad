using UnityEngine;

namespace CoMDS2
{
	public class EnemyWrestler : Enemy
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			AIStateSkillGnaw aIStateSkillGnaw = new AIStateSkillGnaw(this, "SkillGnaw");
			base.meleeRange = (m_baseMeleeRange = 2f);
			SetAttackCollider(false);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			AddAIState(aIStateSkillGnaw.name, aIStateSkillGnaw);
			base.shootRange = 8f;
			isBig = true;
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			string text = "Bip01/Bip01 Pelvis/Bip01 Spine";
			Transform mix = m_transform.Find(text);
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
			if (!base.shootAble && currentAIState.name != "Shoot")
			{
				m_shootAbleTimer += Time.deltaTime;
				if (CheckUseSkill(ref m_shootAbleTimer))
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

		public override void OnSpecialAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				int skillId = Random.Range(0, base.skillInfos.Count - 1);
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

		public override void OnMelee(AIState.AIPhase phase)
		{
			base.OnMelee(phase);
			if (phase == AIState.AIPhase.Update && !AnimationPlaying(m_attackAnimName) && base.shootAble)
			{
				ChangeAIState("Shoot", false);
			}
		}

		public override void OnReleaseSkillToBegin()
		{
			base.OnReleaseSkillToBegin();
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null)
			{
				if (base.currentSkill.type == SkillType.Jump)
				{
					effectPlayManager.PlayEffect("Power", GetTransform().position, true);
					effectPlayManager.PlayEffect("ThudIndicate", GetTransform().position, true);
					EffectControl effectControl = effectPlayManager.GetEffectControl("ThudIndicate");
					effectControl.GetGameObject().transform.position = m_skillTarget.GetTransform().position;
					m_skillTimer = 0f;
				}
				else if (base.currentSkill.type == SkillType.Grab)
				{
					effectPlayManager.PlayEffect("AttackIndicate");
				}
			}
		}

		public override void OnReleaseSkillBegin()
		{
			base.OnReleaseSkillBegin();
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null && base.currentSkill.type != SkillType.Jump && base.currentSkill.type == SkillType.Grab)
			{
				EffectControl effectControl = effectPlayManager.GetEffectControl("AttackIndicate");
				effectControl.GetGameObject().transform.forward = m_skillTarget.GetTransform().position - effectControl.GetGameObject().transform.position;
				GetTransform().forward = effectControl.GetGameObject().transform.forward;
			}
		}

		public override void OnReleaseSkillUpdate()
		{
			base.OnReleaseSkillUpdate();
			if (m_releaseSkillState != ReleaseSkillState.Release || base.currentSkill == null)
			{
				return;
			}
			if (base.currentSkill.type == SkillType.Grab)
			{
				if (m_grabList.Count > 0)
				{
					if (HasNavigation())
					{
						StopNav(false);
					}
					StateToGnaw();
				}
			}
			else if (base.currentSkill.type != SkillType.Jump)
			{
			}
		}

		public override void OnReleaseSkillToEnd()
		{
			base.OnReleaseSkillToEnd();
			if (m_releaseSkillState == ReleaseSkillState.End && base.currentSkill != null && base.currentSkill.type == SkillType.Jump)
			{
				effectPlayManager.StopEffect("ThudIndicate");
				effectPlayManager.PlayEffect("Thud");
			}
		}

		public override void OnReleaseSkillEnd()
		{
			base.OnReleaseSkillEnd();
			if (m_releaseSkillState == ReleaseSkillState.End && base.currentSkill != null && m_releaseSkillState != ReleaseSkillState.Over)
			{
			}
		}

		private void StateToGnaw()
		{
			base.currentSkill = null;
			SetAttackCollider(false, AttackCollider.AttackColliderType.Grab);
			Character character = m_grabList[0];
			AIStateSkillGnaw aIStateSkillGnaw = GetAIState("SkillGnaw") as AIStateSkillGnaw;
			base.skillHitInfo.damage = new NumberSection<float>(base.skillInfos[base.skillInfos.Count - 1].damage);
			base.skillHitInfo.percentDamage = base.skillInfos[base.skillInfos.Count - 1].percentDamage;
			aIStateSkillGnaw.SetSkillInfo(base.skillInfos[base.skillInfos.Count - 1], character);
			SwitchFSM(aIStateSkillGnaw);
			if (!character.isRage)
			{
				if (character.HasNavigation())
				{
					character.StopNav(false);
				}
				else
				{
					character.m_move = false;
				}
				character.animLowerBody = character.GetAnimationName("Idle");
				character.AnimationCrossFade(character.animLowerBody, true);
			}
		}

		public override HitInfo GetHitInfo()
		{
			return base.GetHitInfo();
		}

		public override void OnDeath()
		{
			base.OnDeath();
			effectPlayManager.StopEffect("ThudIndicate");
			effectPlayManager.StopEffect("Dash");
		}
	}
}
