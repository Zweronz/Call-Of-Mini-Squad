using UnityEngine;

namespace CoMDS2
{
	public class EnemyPestilence : Enemy
	{
		protected DS2ObjectBuffer m_bulletBuffer;

		public Bullet.BulletAttribute bulletAttribute;

		private float m_fBulletLife = 20f;

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
			base.meleeRange = (m_baseMeleeRange = 7f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 7f;
			base.shootAble = false;
			SetBullet();
			m_shootPoint = GetTransform().Find("ShootPoint");
			isBig = true;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.Zombie_Pestilence_L_hit);
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
			if (!Alive() || base.shootAble)
			{
				return;
			}
			AIState currentAIState = GetCurrentAIState();
			if (!(currentAIState.name != "Shoot"))
			{
				return;
			}
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
				if (Random.Range(0, 100) < 100)
				{
					ChangeAIState("Shoot", false);
					return;
				}
				base.shootAble = false;
				m_shootAbleTimer = 0f;
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
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					ChangeToDefaultAIState();
				}
				else if (base.lockedTarget != null)
				{
					LookAt(base.lockedTarget.GetTransform());
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
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(28);
			int num = 6;
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
				bullet.hitInfo.specialHit.Add(Defined.SPECIAL_HIT_TYPE.CAMERA_QUAKE, null);
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

		public virtual void EmitBullet(bool homing, float distanceLife = 0f)
		{
			effectPlayManager.PlayEffect("Fire");
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			GameObject gameObject = bulletFromBuffer.GetGameObject();
			gameObject.layer = 23;
			if (bulletFromBuffer == null)
			{
				return;
			}
			bulletFromBuffer.SetBullet(this, null, m_shootPoint.position, m_shootPoint.rotation);
			if (homing)
			{
				if (gameObject.GetComponent<LinearMoveToDestroy>() != null)
				{
					Object.Destroy(gameObject.GetComponent<LinearMoveToDestroy>());
				}
				if (gameObject.GetComponent<HomingMoveToDestroy>() == null)
				{
					gameObject.AddComponent<HomingMoveToDestroy>();
				}
				bulletFromBuffer.EmitHoming(base.lockedTarget.GetGameObject(), 0.3f, 2f, distanceLife);
			}
			else
			{
				if (gameObject.GetComponent<HomingMoveToDestroy>() != null)
				{
					Object.Destroy(gameObject.GetComponent<HomingMoveToDestroy>());
				}
				if (gameObject.GetComponent<LinearMoveToDestroy>() == null)
				{
					gameObject.AddComponent<LinearMoveToDestroy>();
				}
				bulletFromBuffer.Emit(distanceLife);
			}
		}

		private void EmitGasBomb()
		{
			EmitBullet(false, m_fBulletLife);
		}

		private void EmitHomingGasBomb()
		{
			EmitBullet(true, m_fBulletLife);
		}

		private void EmitRandomGasBomb()
		{
			if (Random.Range(0, 100) < 50 || base.lockedTarget == null)
			{
				EmitGasBomb();
			}
			else
			{
				EmitHomingGasBomb();
			}
		}
	}
}
