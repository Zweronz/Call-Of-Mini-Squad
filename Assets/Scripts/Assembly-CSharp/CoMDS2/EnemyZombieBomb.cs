using UnityEngine;

namespace CoMDS2
{
	public class EnemyZombieBomb : Enemy
	{
		private AIStateChase stateChase;

		private float m_triggerToBombRange = 6.5f;

		private bool m_bTriggerBurn;

		private float m_burnDamageDuration = 0.5f;

		private float m_burnDamage = 0.15f;

		private float m_burnTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateSmashSelf aIStateSmashSelf = new AIStateSmashSelf(this, "Death");
			AIStateSmashSelf aIStateSmashSelf2 = new AIStateSmashSelf(this, "Melee");
			aIStateSmashSelf2.SetSmash(0.1f, 2f, 67636736, true);
			base.meleeRange = (m_baseMeleeRange = 1.5f);
			SetAttackCollider(false);
			base.meleeRange = (m_baseMeleeRange = 1.5f);
			stateChase = GetAIState("Chase") as AIStateChase;
			stateChase.SetCustomFunc(OnChase);
			stateChase.SetChase(1000f, baseAttribute.moveSpeed);
			string animationName = GetAnimationName("Move");
			stateChase.animName = animationName;
			base.animLowerBody = animationName;
			aIStateSmashSelf.SetSmash(0.1f, 1.5f, 67636736, true);
			AddAIState(aIStateSmashSelf.name, aIStateSmashSelf);
			AddAIState(aIStateSmashSelf2.name, aIStateSmashSelf2);
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_BOMB_1, 1);
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

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
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

		public override void Reset()
		{
			GetTransform().localScale = Vector3.one;
			base.Reset();
			m_bTriggerBurn = false;
		}

		public void OnChase(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_burnTimer = 0f;
				break;
			case AIState.AIPhase.Update:
				if (!m_bTriggerBurn)
				{
					if (stateChase.target != null)
					{
						float sqrMagnitude = (stateChase.target.GetTransform().position - GetTransform().position).sqrMagnitude;
						if (sqrMagnitude < m_triggerToBombRange * m_triggerToBombRange)
						{
							m_bTriggerBurn = true;
							effectPlayManager.PlayEffect("Burn");
							MoveSpeed *= 1.8f;
							GetTransform().localScale *= 1.3f;
						}
					}
					break;
				}
				m_burnTimer += Time.deltaTime;
				if (m_burnTimer >= m_burnDamageDuration)
				{
					m_burnTimer = 0f;
					base.hp -= (int)((float)base.hpMax * m_burnDamage);
					if (!Alive())
					{
						ChangeAIState("Death");
					}
				}
				break;
			}
		}
	}
}
