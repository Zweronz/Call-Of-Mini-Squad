using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class EnemyFatCook : Enemy
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			base.meleeRange = (m_baseMeleeRange = 1.3f);
			SetAttackCollider(false);
			base.meleeRange = (m_baseMeleeRange = 3f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 8f;
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
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].AddMixingTransform(mix);
				}
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (!Alive() || base.shootAble)
			{
				return;
			}
			AIState currentAIState = GetCurrentAIState();
			if (currentAIState.name != "Shoot")
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
			if (base.enemyType == EnemyType.ZombieBomb && currentAIState.name == "Death")
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
				base.audioManager.PlayAudio("Skill");
				int skillId = Random.Range(0, base.skillInfos.Count);
				UseSkill(skillId);
				base.isRage = true;
				DoSummon();
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
				break;
			}
		}

		private void DoSummon()
		{
			for (int i = 0; i < 4; i++)
			{
				EnemySpawnPointMain.EnemyWaitSpawnInfo item = default(EnemySpawnPointMain.EnemyWaitSpawnInfo);
				item.type = EnemyType.Zombie;
				item.spawnInfo = new SpawnInfo();
				item.isBoss = false;
				item.eliteType = EnemyEliteType.None;
				item.specialAttribute = new List<SpecialAttribute.AttributeType>();
				GameBattle.s_enemyWaitToVisibleList.Add(item);
			}
			for (int j = 0; j < 2; j++)
			{
				EnemySpawnPointMain.EnemyWaitSpawnInfo item2 = default(EnemySpawnPointMain.EnemyWaitSpawnInfo);
				item2.type = EnemyType.ZombieBomb;
				item2.spawnInfo = new SpawnInfo();
				item2.isBoss = false;
				item2.eliteType = EnemyEliteType.None;
				item2.specialAttribute = new List<SpecialAttribute.AttributeType>();
				GameBattle.s_enemyWaitToVisibleList.Add(item2);
			}
		}
	}
}
