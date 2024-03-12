using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class EnemySpore : Enemy
	{
		private AIState.AIPhase m_skillPhase;

		private Transform m_shootPoint;

		protected DS2ObjectBuffer m_bulletBuffer;

		private bool m_sporeCrossShoot;

		private float m_bulletLife = 15f;

		private int m_diciCount = 5;

		private SporeDiCi[] m_dici;

		private int m_teleportCount = 3;

		private float m_teleportTime;

		private SporeTeleport m_teleport;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			RemoveAIState("Chase");
			SetDefaultAIState(GetAIState("Idle"));
			AIStateMelee aIStateMelee = new AIStateMelee(this, "Melee");
			aIStateMelee.SetCustomFunc(OnMelee);
			AIStateEnemySpecialAttack aIStateEnemySpecialAttack = new AIStateEnemySpecialAttack(this, "Shoot");
			aIStateEnemySpecialAttack.SetCustomFunc(OnSpecialAttack);
			base.meleeRange = (m_baseMeleeRange = 5f);
			SetAttackCollider(false);
			AddAIState(aIStateMelee.name, aIStateMelee);
			AddAIState(aIStateEnemySpecialAttack.name, aIStateEnemySpecialAttack);
			base.shootRange = 15f;
			base.shootAble = false;
			m_shootAbleTimer = 1f;
			m_unableRepelTime = 9999999f;
			SetBullet();
			m_shootPoint = GetTransform().Find("ShootPoint");
			m_dici = new SporeDiCi[m_diciCount];
			for (int i = 0; i < m_diciCount; i++)
			{
				m_dici[i] = new SporeDiCi();
				string text = ((base.enemyType != EnemyType.Spore) ? "Spore/SporePurpleDiCi" : "Spore/SporeGreenDiCi");
				GameObject prefab2 = Resources.Load<GameObject>("Models/ActiveObjects/" + text);
				m_dici[i].Initialize(prefab2, "SporeDiCi", Vector3.zero, Quaternion.identity, 25);
				m_dici[i].GetGameObject().SetActive(false);
				m_dici[i].GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			}
			m_teleport = new SporeTeleport();
			GameObject prefab3 = Resources.Load<GameObject>("Models/ActiveObjects/Spore/SporeTeleport");
			m_teleport.Initialize(prefab3, "SporeTeleport", Vector3.zero, Quaternion.identity, 0);
			m_teleport.GetGameObject().SetActive(false);
			m_teleport.GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			isBig = true;
		}

		protected override void SetAnimationsMixing()
		{
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
			if (!(currentAIState.name != "Shoot"))
			{
				return;
			}
			if (currentAIState.name == "Idle")
			{
				if (!AnimationPlaying(GetAnimationName("Hurt")) && !AnimationPlaying(GetAnimationName("Idle")))
				{
					AnimationPlay(GetAnimationName("Idle"), true);
				}
				m_shootAbleTimer += Time.deltaTime;
				if (m_shootAbleTimer > 1f)
				{
					base.lockedTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
					m_shootAbleTimer = 0f;
				}
				if (base.lockedTarget != null)
				{
					GetTransform().forward = base.lockedTarget.GetTransform().position - GetTransform().position;
				}
				float num = base.meleeRange * base.meleeRange;
				float sqrMagnitude = (base.lockedTarget.GetTransform().position - GetTransform().position).sqrMagnitude;
				if (sqrMagnitude <= num && base.meleeAble)
				{
					SwitchFSM(GetAIState("Melee"));
				}
			}
			m_skillTimer += Time.deltaTime;
			if (CheckUseSkill(ref m_skillTimer))
			{
				base.shootAble = true;
				if (currentAIState.name != "Melee")
				{
					SwitchFSM(GetAIState("Shoot"));
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
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				m_attackAnimName = GetAnimationName("Attack");
				int num2 = Random.Range(0, 100);
				if ((float)num2 < m_proFirstAttackLose)
				{
					m_waitAttackAnimName = GetAnimationName("Idle");
					AnimationCrossFade(m_waitAttackAnimName, true);
				}
				else
				{
					AnimationCrossFade(m_attackAnimName, false);
				}
				m_attackTimer = 0f;
				break;
			}
			case AIState.AIPhase.Update:
			{
				DS2ActiveObject nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
				if (nearestObjFromTargetList != null)
				{
					LookAt(nearestObjFromTargetList.GetTransform());
					float num = base.meleeRange * base.meleeRange;
					float sqrMagnitude = (nearestObjFromTargetList.GetTransform().position - GetTransform().position).sqrMagnitude;
					if (sqrMagnitude > num && !AnimationPlaying(m_attackAnimName))
					{
						ChangeAIState("Idle");
						m_shootAbleTimer = 1f;
						break;
					}
				}
				m_attackTimer += Time.deltaTime;
				if (m_attackTimer >= m_attFrequency)
				{
					if (!AnimationPlaying(m_attackAnimName))
					{
						m_attackTimer = 0f;
						m_attackAnimName = GetAnimationName("Attack");
						AnimationPlay(m_attackAnimName, false);
					}
				}
				else if (!AnimationPlaying(m_attackAnimName))
				{
					m_waitAttackAnimName = GetAnimationName("Idle");
					AnimationCrossFade(m_waitAttackAnimName, true);
				}
				break;
			}
			case AIState.AIPhase.Exit:
				SetAttackCollider(false);
				break;
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
			if (base.lockedTarget != null)
			{
				m_skillTarget = base.lockedTarget;
			}
			else
			{
				m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
			}
			switch (base.currentSkill.type)
			{
			case SkillType.Throw:
				m_sporeCrossShoot = base.hpPercent < 0.5f;
				m_skillPhase = AIState.AIPhase.Enter;
				OnReleaseSkillToUpdate();
				break;
			case SkillType.Dash:
				m_skillTarget = GameBattle.m_instance.GetPlayer();
				OnReleaseSkillToUpdate();
				break;
			case SkillType.Jump:
				base.animLowerBody = base.currentSkill.animReady;
				m_skillPhase = AIState.AIPhase.Enter;
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				break;
			}
		}

		public override void OnReleaseSkillBegin()
		{
			if (m_releaseSkillState == ReleaseSkillState.Ready && m_skillTarget != null && base.currentSkill.type == SkillType.Jump)
			{
				GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
				if (!AnimationPlaying(base.animLowerBody))
				{
					base.animLowerBody = base.currentSkill.animProcess;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					m_teleport.GetTransform().position = GetTransform().position;
					m_teleport.TeleportStart();
					Visible = false;
					OnReleaseSkillToUpdate();
				}
			}
		}

		public override void OnReleaseSkillToUpdate()
		{
			switch (base.currentSkill.type)
			{
			case SkillType.Throw:
				if (m_sporeCrossShoot)
				{
					base.animLowerBody = GetAnimationName("Attack_Special_02_A");
				}
				else
				{
					base.animLowerBody = GetAnimationName("Attack_Special_01_A");
				}
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				m_releaseSkillState = ReleaseSkillState.Release;
				break;
			case SkillType.Dash:
				base.animLowerBody = base.currentSkill.animProcess;
				AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
				m_releaseSkillState = ReleaseSkillState.Release;
				break;
			case SkillType.Jump:
				m_releaseSkillState = ReleaseSkillState.Release;
				break;
			}
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
			case SkillType.Throw:
				if (m_skillTarget != null)
				{
					GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
				}
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					if (!AnimationPlaying(base.animLowerBody))
					{
						if (m_sporeCrossShoot)
						{
							base.animLowerBody = GetAnimationName("Attack_Special_02_B");
						}
						else
						{
							base.animLowerBody = GetAnimationName("Attack_Special_01_B");
						}
						AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						m_skillPhase = AIState.AIPhase.Update;
					}
					break;
				case AIState.AIPhase.Update:
					if (!AnimationPlaying(base.animLowerBody))
					{
						if (!m_sporeCrossShoot)
						{
							base.animLowerBody = GetAnimationName("Attack_Special_01_C");
							AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
						}
						m_skillPhase = AIState.AIPhase.Exit;
					}
					break;
				case AIState.AIPhase.Exit:
					if (!m_sporeCrossShoot)
					{
						if (!AnimationPlaying(base.animLowerBody))
						{
							OnReleaseSkillToEnd();
						}
					}
					else
					{
						OnReleaseSkillToEnd();
					}
					break;
				}
				break;
			case SkillType.Dash:
				if (m_skillTarget != null)
				{
					GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
				}
				if (!AnimationPlaying(base.animLowerBody))
				{
					OnReleaseSkillToEnd();
				}
				break;
			case SkillType.Jump:
				m_teleportTime += Time.deltaTime;
				switch (m_skillPhase)
				{
				case AIState.AIPhase.Enter:
					if (m_teleportTime > 1.5f)
					{
						if (m_skillTarget == null || !m_skillTarget.Alive())
						{
							m_skillTarget = GameBattle.m_instance.GetNearestObjFromTargetList(this);
						}
						if (m_skillTarget != null)
						{
							m_teleport.SetAttackIndicate(m_skillTarget.GetTransform().position);
							GetTransform().position = m_skillTarget.GetTransform().position;
						}
						else
						{
							m_teleport.SetAttackIndicate(GetTransform().position);
						}
						m_teleportTime = 0f;
						m_skillPhase = AIState.AIPhase.Update;
					}
					break;
				case AIState.AIPhase.Update:
				{
					if (!(m_teleportTime > 1.5f))
					{
						break;
					}
					m_teleport.TeleportEnd();
					GetGameObject().SetActive(true);
					GetTransform().position = GetTransform().position + Vector3.forward * 0.01f;
					if (m_skillTarget != null)
					{
						GetTransform().forward = m_skillTarget.GetTransform().position - GetTransform().position;
					}
					base.animLowerBody = base.currentSkill.animEnd;
					AnimationCrossFade(base.animLowerBody, false, false, 0.1f);
					effectPlayManager.PlayEffect("Attack");
					HitInfo hitInfo = new HitInfo();
					foreach (DataConf.SkillInfo skillInfo in base.skillInfos)
					{
						if (skillInfo.type == SkillType.Throw)
						{
							DataConf.GameLevelData currentGameLevelData = DataCenter.Conf().GetCurrentGameLevelData();
							hitInfo.repelDistance = new NumberSection<float>(0f, 0f);
							hitInfo.repelTime = 0f;
							hitInfo.damage = new NumberSection<float>(skillInfo.damage, skillInfo.damage);
							hitInfo.damage = new NumberSection<float>(hitInfo.damage.left + (float)((currentGameLevelData.level - 1) * 1), hitInfo.damage.left + (float)((currentGameLevelData.level - 1) * 3));
							SetEliteSkillAttribute();
							hitInfo.critDamage = hitInfo.damage.left + hitInfo.damage.left * base.critDamage;
							hitInfo.percentDamage = skillInfo.percentDamage;
						}
					}
					hitInfo.source = this;
					Vector3 position = new Vector3(GetTransform().position.x, m_shootPoint.position.y, GetTransform().position.z);
					for (int i = 0; i < 2; i++)
					{
						BulletSpore bulletSpore = m_bulletBuffer.GetObject() as BulletSpore;
						bulletSpore.GetGameObject().layer = 23;
						if (bulletSpore != null)
						{
							bulletSpore.hitInfo = hitInfo;
							bulletSpore.crossExplode = true;
							Quaternion rotation = Quaternion.AngleAxis(m_shootPoint.eulerAngles.y + 90f + (float)(180 * i), Vector3.up);
							bulletSpore.SetBullet(this, null, position, rotation);
							bulletSpore.Emit(m_bulletLife);
						}
					}
					for (int j = 0; j < 2; j++)
					{
						BulletSpore bulletSpore2 = m_bulletBuffer.GetObject() as BulletSpore;
						bulletSpore2.GetGameObject().layer = 23;
						if (bulletSpore2 != null)
						{
							bulletSpore2.hitInfo = hitInfo;
							bulletSpore2.crossExplode = true;
							Quaternion rotation2 = Quaternion.AngleAxis(m_shootPoint.eulerAngles.y + (float)(180 * j), Vector3.up);
							bulletSpore2.SetBullet(this, null, position, rotation2);
							bulletSpore2.Emit(m_bulletLife);
						}
					}
					m_skillPhase = AIState.AIPhase.Exit;
					Visible = true;
					break;
				}
				case AIState.AIPhase.Exit:
					if (!AnimationPlaying(base.animLowerBody))
					{
						m_teleportCount--;
						if (m_teleportCount > 0)
						{
							OnReleaseSkillToBegin();
							break;
						}
						m_releaseSkillState = ReleaseSkillState.End;
						m_teleportCount = 3;
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

		private void SetBullet()
		{
			int index = ((base.enemyType != EnemyType.Spore) ? 31 : 30);
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(index);
			int num = 12;
			m_bulletBuffer = new DS2ObjectBuffer(num);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < num; i++)
			{
				BulletSpore bulletSpore = new BulletSpore(null);
				bulletSpore.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bulletSpore.GetGameObject();
				if (gameObject2.GetComponent<LinearMoveToDestroy>() == null)
				{
					gameObject2.AddComponent<LinearMoveToDestroy>();
				}
				if (gameObject2.GetComponent<BulletTriggerScript>() == null)
				{
					gameObject2.AddComponent<BulletTriggerScript>();
				}
				bulletSpore.attribute = new Bullet.BulletAttribute();
				bulletSpore.attribute.speed = 6.5f;
				bulletSpore.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				gameObject2.SetActive(false);
				bulletSpore.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bulletSpore);
			}
		}

		private void ShootLineSpore()
		{
			BulletSpore bulletSpore = m_bulletBuffer.GetObject() as BulletSpore;
			bulletSpore.GetGameObject().layer = 23;
			if (bulletSpore != null)
			{
				bulletSpore.hitInfo = GetHitInfo();
				bulletSpore.crossExplode = false;
				bulletSpore.SetBullet(this, null, m_shootPoint.position, m_shootPoint.rotation);
				bulletSpore.Emit(m_bulletLife);
			}
		}

		private void ShootCrossSpore()
		{
			for (int i = 0; i < 4; i++)
			{
				BulletSpore bulletSpore = m_bulletBuffer.GetObject() as BulletSpore;
				bulletSpore.GetGameObject().layer = 23;
				if (bulletSpore != null)
				{
					bulletSpore.hitInfo = GetHitInfo();
					bulletSpore.crossExplode = true;
					Quaternion rotation = Quaternion.AngleAxis(m_shootPoint.eulerAngles.y - 45f + (float)(30 * i), Vector3.up);
					bulletSpore.SetBullet(this, null, m_shootPoint.position, rotation);
					bulletSpore.Emit(m_bulletLife);
				}
			}
		}

		private void UseDiCi()
		{
			List<Vector3> list = new List<Vector3>();
			NodeGroup nearestNodeGroup = GameBattle.m_instance.GetNearestNodeGroup();
			for (int i = 0; i < m_diciCount; i++)
			{
				m_dici[i].DiCiAppear(base.skillHitInfo, base.currentSkill.speed, base.currentSkill.time);
				Vector3 randomDiciPos = GetRandomDiciPos(nearestNodeGroup);
				if (list.Count > 0)
				{
					while (true)
					{
						bool flag = true;
						foreach (Vector3 item in list)
						{
							if (Vector3.Distance(item, randomDiciPos) < 3f)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							randomDiciPos = GetRandomDiciPos(nearestNodeGroup);
							continue;
						}
						break;
					}
				}
				m_dici[i].GetTransform().position = randomDiciPos;
				list.Add(randomDiciPos);
			}
		}

		private Vector3 GetRandomDiciPos(NodeGroup nodeGroup)
		{
			Vector3 randomSpawnPointPosition = nodeGroup.GetRandomSpawnPointPosition();
			Vector3 randomSpawnPointPosition2 = nodeGroup.GetRandomSpawnPointPosition();
			return new Vector3(Random.Range(randomSpawnPointPosition.x, randomSpawnPointPosition2.x), randomSpawnPointPosition.y, Random.Range(randomSpawnPointPosition.z, randomSpawnPointPosition2.z));
		}

		private void StartTeleport()
		{
			GetGameObject().SetActive(false);
			m_skillPhase = AIState.AIPhase.Enter;
			m_releaseSkillState = ReleaseSkillState.Release;
			m_teleportTime = 0f;
		}
	}
}
