using UnityEngine;

namespace CoMDS2
{
	public class EnemyZombieNurse : Enemy
	{
		private AIState.AIPhase m_spitPhase;

		protected DS2ObjectBuffer m_bulletBuffer;

		public Bullet.BulletAttribute bulletAttribute;

		private float m_fBulletLife = 8f;

		private Transform m_spitPoint;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			base.meleeRange = (m_baseMeleeRange = 1.3f);
			SetAttackCollider(false);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 6f;
			base.shootAble = true;
			base.skillHitInfo.damage = base.hitInfo.damage;
			base.skillHitInfo.hitSpawnInfo = new DeadSpawnInfo(DeadSpawnInfo.DeadSpawnType.Zombie_Nurse_Venom, 1);
			SetBullet();
			m_spitPoint = GetTransform().Find("SpitPoint");
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.ZOMBIE_NURSE_DAMAGE_VENOM);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.ZOMBIE_NURSE_VENOM_ATTACK);
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			Transform mix = m_transform.Find("Bip01/Bip01 Spine");
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
				base.isRage = true;
				base.skillAttack = true;
				StateToSpitEnter();
				break;
			case AIState.AIPhase.Update:
				switch (m_spitPhase)
				{
				case AIState.AIPhase.Enter:
					UpdateStateSpitEnter();
					break;
				case AIState.AIPhase.Update:
					UpdateStateSpitUpdate();
					break;
				case AIState.AIPhase.Exit:
					UpdateStateSpitEnd();
					break;
				}
				break;
			case AIState.AIPhase.Exit:
				base.skillAttack = false;
				base.isRage = false;
				break;
			}
		}

		private void StateToSpitEnter()
		{
			m_spitPhase = AIState.AIPhase.Enter;
			base.animLowerBody = GetAnimationName("Attack_Special_01_A");
			AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
			DS2ActiveObject dS2ActiveObject = (base.lockedTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this));
			GetTransform().forward = dS2ActiveObject.GetTransform().position - GetTransform().position;
		}

		private void UpdateStateSpitEnter()
		{
			if (!AnimationPlaying(base.animLowerBody))
			{
				StateToSpitUpdate();
			}
			else
			{
				GetTransform().forward = base.lockedTarget.GetTransform().position - GetTransform().position;
			}
		}

		private void StateToSpitUpdate()
		{
			m_spitPhase = AIState.AIPhase.Update;
			base.animLowerBody = GetAnimationName("Attack_Special_01_B");
			AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
			effectPlayManager.PlayEffect("Fire");
			EmitBullet(m_fBulletLife);
		}

		private void UpdateStateSpitUpdate()
		{
			if (!AnimationPlaying(base.animLowerBody))
			{
				StateToSpitEnd();
			}
		}

		private void StateToSpitEnd()
		{
			m_spitPhase = AIState.AIPhase.Exit;
			base.animLowerBody = GetAnimationName("Attack_Special_01_C");
			AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
		}

		private void UpdateStateSpitEnd()
		{
			if (!AnimationPlaying(base.animLowerBody))
			{
				ChangeToDefaultAIState();
			}
		}

		protected virtual void SetBullet()
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(24);
			int b = (int)(3f / m_attFrequency);
			b = Mathf.Max(1, b);
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < b; i++)
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
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			bulletFromBuffer.GetGameObject().layer = 23;
			if (bulletFromBuffer != null)
			{
				bulletFromBuffer.hitInfo = GetHitInfo();
				bulletFromBuffer.SetBullet(this, null, m_spitPoint.position, m_spitPoint.rotation);
				bulletFromBuffer.Emit(distanceLife);
			}
		}
	}
}
