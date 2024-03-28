using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class EnemyCowboy : Enemy
	{
		protected DS2ObjectBuffer m_bulletBuffer;

		protected DS2ObjectBuffer m_sniperBulletBuffer;

		public Bullet.BulletAttribute bulletAttribute;

		private float m_fBulletLife = 15f;

		private int m_sniperCount = 1;

		private float m_lockTargetTime;

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
			base.meleeRange = (m_baseMeleeRange = 6f);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 8f;
			base.shootAble = false;
			SetBullet();
			m_shootPoint = GetTransform().Find("ShootPoint");
			isBig = true;
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			Transform mix = m_transform.Find("Bip01/Spine_00/Bip01 Spine");
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
			if (currentAIState.name == "Born")
			{
				return result;
			}
			return base.OnHit(hitInfo);
		}

		public override void OnMelee(AIState.AIPhase phase)
		{
			base.OnMelee(phase);
		}

		public override void OnSpecialAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				base.isRage = true;
				base.skillAttack = true;
				m_lockTargetTime = 0f;
				base.animLowerBody = GetAnimationName("Attack_Special_01_A");
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				base.currentSkill = base.skillInfos[0];
				SetSkillHitInfo();
				base.currentSkill = null;
				effectPlayManager.PlayEffect("AttackIndicate");
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					ChangeToDefaultAIState();
				}
				else if (base.lockedTarget != null && m_lockTargetTime < 1.5f)
				{
					m_lockTargetTime += Time.deltaTime;
					LookAt(base.lockedTarget.GetTransform());
				}
				break;
			case AIState.AIPhase.Exit:
				m_sniperCount--;
				if (m_sniperCount > 0)
				{
					ChangeAIState("Shoot", false);
					break;
				}
				base.shootAble = false;
				base.skillAttack = false;
				base.isRage = false;
				break;
			}
		}

		protected virtual void SetBullet()
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(27);
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
			bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(2);
			num = 2;
			m_sniperBulletBuffer = new DS2ObjectBuffer(num);
			gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int j = 0; j < num; j++)
			{
				Bullet bullet2 = new Bullet(null);
				bullet2.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject3 = bullet2.GetGameObject();
				if (gameObject3.GetComponent<LinearMoveToDestroy>() == null)
				{
					gameObject3.AddComponent<LinearMoveToDestroy>();
				}
				if (gameObject3.GetComponent<BulletTriggerScript>() == null)
				{
					gameObject3.AddComponent<BulletTriggerScript>();
				}
				bullet2.hitInfo.damage = base.hitInfo.damage;
				bullet2.attribute = new Bullet.BulletAttribute();
				bullet2.attribute.speed = 60f;
				bullet2.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				gameObject3.SetActive(false);
				bullet2.GetTransform().parent = gameObject.transform;
				m_sniperBulletBuffer.AddObj(bullet2);
			}
		}

		public virtual Bullet GetBulletFromBuffer()
		{
			return m_bulletBuffer.GetObject() as Bullet;
		}

		public virtual void EmitBullet(float distanceLife = 0f)
		{
			Bullet bulletFromBuffer = GetBulletFromBuffer();
			effectPlayManager.PlayEffect("Fire");
			bulletFromBuffer.GetGameObject().layer = 23;
			if (bulletFromBuffer != null)
			{
				bulletFromBuffer.hitInfo = GetHitInfo();
				bulletFromBuffer.SetBullet(this, null, m_shootPoint.position, m_shootPoint.rotation);
				bulletFromBuffer.Emit(distanceLife);
			}
		}

		private void GunShoot()
		{
			EmitBullet(m_fBulletLife);
		}

		private void SniperShoot()
		{
			effectPlayManager.PlayEffect("Fire_02");
			List<GameObject> list = new List<GameObject>();
			for (int i = -1; i < 2; i++)
			{
				float num = 15f;
				RaycastHit raycastHit;
				if (Physics.Raycast(m_shootPoint.position + m_shootPoint.rotation * (Vector3.right * 0.5f * i), m_shootPoint.forward, out raycastHit, num, 327680))
				{
					num = Vector3.Distance(m_shootPoint.position, raycastHit.point);
				}
				if (i == 0)
				{
					Bullet bullet = m_sniperBulletBuffer.GetObject() as Bullet;
					bullet.GetGameObject().layer = 23;
					if (bullet != null)
					{
						bullet.SetBullet(this, null, m_shootPoint.position, m_shootPoint.rotation);
						bullet.Emit(num);
					}
				}
				RaycastHit[] array = Physics.RaycastAll(m_shootPoint.position + m_shootPoint.rotation * (Vector3.right * 0.5f * i), m_shootPoint.forward, num, 1536);
				if (array.Length <= 0)
				{
					continue;
				}
				RaycastHit[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					RaycastHit raycastHit2 = array2[j];
					if (!list.Contains(raycastHit2.collider.gameObject))
					{
						list.Add(raycastHit2.collider.gameObject);
					}
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			DataConf.SkillInfo skillInfo = base.skillInfos[0];
			HitInfo hitInfo = new HitInfo();
			hitInfo.damage = new NumberSection<float>(skillInfo.damage, skillInfo.damage);
			hitInfo.repelTime = skillInfo.repelTime;
			hitInfo.repelDistance = new NumberSection<float>(skillInfo.repelDis, skillInfo.repelDis);
			hitInfo.source = this;
			Buff buff = null;
			if (Random.Range(0, 100) < 30)
			{
				SpecialHitInfo specialHitInfo = new SpecialHitInfo();
				specialHitInfo.time = 12f;
				specialHitInfo.disposable = false;
				hitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.STUN, specialHitInfo);
				buff = new Buff(Buff.AffectType.MoveSpeed, -0.3f, 5f, 5f, Buff.CalcType.Percentage, 0f);
			}
			foreach (GameObject item in list)
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(item);
				if (@object != null)
				{
					hitInfo.repelDirection = @object.GetTransform().position - GetTransform().position;
					hitInfo.hitPoint = @object.GetTransform().position;
					@object.OnHit(hitInfo);
					IBuffManager buffManager = @object.GetBuffManager();
					if (buffManager != null && buff != null)
					{
						buffManager.AddBuff(buff);
					}
				}
			}
		}
	}
}
