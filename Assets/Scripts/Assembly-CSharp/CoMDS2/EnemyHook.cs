using UnityEngine;

namespace CoMDS2
{
	public class EnemyHook : Enemy
	{
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
				if (Random.Range(0, 100) < 30)
				{
					ChangeAIState("Shoot", false);
				}
				else
				{
					base.shootAble = false;
				}
			}
		}

		public override void OnSpecialAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				base.isRage = true;
				UseSkill(0);
				break;
			case AIState.AIPhase.Update:
				if (base.currentSkill == null)
				{
					ChangeToDefaultAIState();
					m_skillTimer = 0f;
					SetGodTime(0f);
				}
				break;
			case AIState.AIPhase.Exit:
				base.shootAble = false;
				base.skillAttack = false;
				base.isRage = false;
				m_skillTimer = 0f;
				break;
			}
		}

		public override void OnReleaseSkillToBegin()
		{
			base.OnReleaseSkillToBegin();
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null && base.currentSkill.type == SkillType.Dash)
			{
				effectPlayManager.PlayEffect("AttackIndicate");
			}
		}

		public override void OnReleaseSkillBegin()
		{
			base.OnReleaseSkillBegin();
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null && base.currentSkill.type == SkillType.Dash)
			{
				EffectControl effectControl = effectPlayManager.GetEffectControl("AttackIndicate");
				effectControl.GetGameObject().transform.forward = m_skillTarget.GetTransform().position - effectControl.GetGameObject().transform.position;
			}
		}

		public override void OnReleaseSkillToEnd()
		{
			base.OnReleaseSkillToEnd();
			AnimationPlay(base.currentSkill.animEnd, false);
			SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
		}
	}
}
