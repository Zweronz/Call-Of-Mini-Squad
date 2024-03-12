using UnityEngine;

namespace CoMDS2
{
	public class EnemyHaoke : Enemy
	{
		protected DS2ObjectBuffer m_iceBallBuffer;

		protected DS2ObjectBuffer m_fireBallBuffer;

		private Transform m_emitBallPosition;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			float num = baseAttribute.moveSpeed * Random.Range(-0.25f, 0.25f);
			SetNavSpeed(baseAttribute.moveSpeed + num);
			base.meleeRange = (m_baseMeleeRange = 2.2f);
			SetAttackCollider(false);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			m_emitBallPosition = GetTransform().Find("EmitPosition");
			SpecialHitInfo specialHitInfo = new SpecialHitInfo();
			specialHitInfo.time = 1f;
			specialHitInfo.disposable = false;
			base.hitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.STUN, specialHitInfo);
			base.shootRange = 8f;
			CreateBulletBuffer();
			isBig = true;
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			string text = "Dummy03/Dummy02/Bip01/Bip01 Pelvis/Bip01 Spine";
			Transform mix = m_transform.Find(text);
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
				if (base.currentSkill == null)
				{
					ChangeToDefaultAIState();
					m_skillTimer = 0f;
					SetGodTime(0f);
				}
				break;
			case AIState.AIPhase.Exit:
				base.shootAble = false;
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
					effectPlayManager.PlayEffect("ThudIndicate", m_skillTargetPos, true);
					EffectControl effectControl = effectPlayManager.GetEffectControl("ThudIndicate");
					effectControl.GetGameObject().transform.position = m_skillTarget.GetTransform().position;
					effectPlayManager.PlayEffect("Power", GetTransform().position, true);
					m_skillTimer = 0f;
				}
				else if (base.currentSkill.type == SkillType.Dash)
				{
					effectPlayManager.PlayEffect("AttackIndicate");
				}
			}
		}

		public override void OnReleaseSkillBegin()
		{
			base.OnReleaseSkillBegin();
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null && base.currentSkill.type != SkillType.Jump && base.currentSkill.type == SkillType.Dash)
			{
				EffectControl effectControl = effectPlayManager.GetEffectControl("AttackIndicate");
				effectControl.GetGameObject().transform.forward = m_skillTarget.GetTransform().position - effectControl.GetGameObject().transform.position;
			}
		}

		public override void OnReleaseSkillToUpdate()
		{
			base.OnReleaseSkillToUpdate();
			if (m_releaseSkillState == ReleaseSkillState.Release)
			{
				if (base.currentSkill.type == SkillType.Dash)
				{
					effectPlayManager.PlayEffect("Dash");
				}
				else if (base.currentSkill.type != SkillType.Jump)
				{
				}
			}
		}

		public override void OnReleaseSkillUpdate()
		{
			base.OnReleaseSkillUpdate();
			if (m_releaseSkillState == ReleaseSkillState.Release && base.currentSkill != null && base.currentSkill.type != 0 && base.currentSkill.type != SkillType.Jump)
			{
			}
		}

		public override void OnReleaseSkillToEnd()
		{
			base.OnReleaseSkillToEnd();
			if (m_releaseSkillState == ReleaseSkillState.End && base.currentSkill != null)
			{
				if (base.currentSkill.type == SkillType.Jump)
				{
					effectPlayManager.StopEffect("ThudIndicate");
				}
				else if (base.currentSkill.type == SkillType.Dash)
				{
					effectPlayManager.StopEffect("Dash");
				}
			}
		}

		public override void OnReleaseSkillEnd()
		{
			base.OnReleaseSkillEnd();
			if (m_releaseSkillState == ReleaseSkillState.End && base.currentSkill != null && m_releaseSkillState != ReleaseSkillState.Over)
			{
			}
		}

		private void CreateBulletBuffer()
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(38);
			m_iceBallBuffer = new DS2ObjectBuffer(6);
			GameObject gameObject = new GameObject();
			gameObject.name = "HaoKeSkillBullet";
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < m_iceBallBuffer.Size; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				gameObject2.AddComponent<BulletTriggerScript>();
				bullet.hitInfo.bSkill = true;
				bullet.hitInfo.damage = new NumberSection<float>(80f, 80f);
				bullet.hitInfo.source = this;
				bullet.hitInfo.hitEffect = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				if (bullet.attribute == null)
				{
					bullet.attribute = new Bullet.BulletAttribute();
				}
				bullet.attribute.speed = 10f;
				bullet.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_iceBallBuffer.AddObj(bullet);
			}
		}

		private void EmitBall()
		{
			for (int i = 0; i < m_iceBallBuffer.Size; i++)
			{
				Quaternion rotation = Quaternion.AngleAxis((float)i * 360f / (float)m_iceBallBuffer.Size, Vector3.up);
				EmitIceBall(20f, m_emitBallPosition.position, rotation);
			}
		}

		private void ThrowBall()
		{
			EmitIceBall(20f, m_emitBallPosition.position, GetTransform().rotation);
		}

		private void EmitFireBall(float distanceLife, Vector3 firePoint, Quaternion rotation)
		{
			Bullet bullet = m_fireBallBuffer.GetObject() as Bullet;
			if (base.clique == Clique.Computer)
			{
				bullet.GetGameObject().layer = 23;
			}
			else
			{
				bullet.GetGameObject().layer = 22;
			}
			if (bullet != null)
			{
				bullet.SetBullet(this, firePoint, rotation);
				bullet.Emit(distanceLife);
			}
		}

		private void EmitIceBall(float distanceLife, Vector3 firePoint, Quaternion rotation)
		{
			Bullet bullet = m_iceBallBuffer.GetObject() as Bullet;
			if (base.clique == Clique.Computer)
			{
				bullet.GetGameObject().layer = 23;
			}
			else
			{
				bullet.GetGameObject().layer = 22;
			}
			if (bullet != null)
			{
				bullet.SetBullet(this, firePoint, rotation);
				bullet.Emit(distanceLife);
			}
		}

		public override void OnDeath()
		{
			base.OnDeath();
			effectPlayManager.StopEffect("ThudIndicate");
			effectPlayManager.StopEffect("Dash");
		}
	}
}
