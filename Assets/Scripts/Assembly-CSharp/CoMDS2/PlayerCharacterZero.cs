using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterZero : Player
	{
		public enum NinjiaType
		{
			Fire = 0,
			Ice = 1
		}

		private NinjiaType m_ninjiaType = NinjiaType.Ice;

		private float m_emitTime = 0.3f;

		private float m_emitTimer;

		private float m_originalSpeed;

		private float m_reduceSpeed;

		private Character[] m_skillTargets;

		private Transform m_firePoint;

		private EffectParticleContinuous m_effectFlash;

		private SummonDragon m_summonDragon;

		private ITAudioEvent m_audioSkill;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Shoot");
			aIState.SetCustomFunc(OnAttack);
			AIState aIState2 = new AIState(this, "MoveShoot");
			aIState2.SetCustomFunc(OnMoveAttack);
			AIState aIState3 = new AIState(this, "Skill");
			aIState3.SetCustomFunc(OnSkillSummonDragon);
			AIState aIState4 = new AIState(this, "UpperFire");
			aIState4.SetCustomFunc(OnUpperFire);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIState3.name, aIState3);
			AddAIState(aIState4.name, aIState4);
			m_firePoint = GetTransform().Find("FirePoint");
			GameObject gameObject = GetTransform().Find("flash").gameObject;
			m_effectFlash = gameObject.GetComponentInChildren<EffectParticleContinuous>();
			m_effectFlash.gameObject.SetActive(false);
			if (m_ninjiaType == NinjiaType.Fire)
			{
				base.hitInfo.hitEffect = Defined.EFFECT_TYPE.NINJIA_ATTACK_FIRE_SWING;
				base.hitInfo.deadSpawnInfo = new DeadSpawnInfo(DeadSpawnInfo.DeadSpawnType.Ninjia_Fire, 2);
			}
			else
			{
				base.hitInfo.hitEffect = Defined.EFFECT_TYPE.NINJIA_01_ATTACK_ICE_SWING;
				base.hitInfo.deadSpawnInfo = new DeadSpawnInfo(DeadSpawnInfo.DeadSpawnType.Ninjia_Ice, 2);
			}
			DataConf.SkillZero skillZero = (DataConf.SkillZero)(base.skillInfo = (DataConf.SkillZero)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			HitInfo hitInfo = new HitInfo(base.hitInfo);
			hitInfo.repelTime = 0f;
			hitInfo.repelDistance = new NumberSection<float>(0f, 0f);
			NumberSection<float> aTK = skillZero.GetATK();
			hitInfo.damage = aTK;
			GameObject original = Resources.Load<GameObject>("Models/ActiveObjects/ZeroSkill/SummonDragon");
			GameObject gameObject2 = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			m_summonDragon = gameObject2.GetComponent<SummonDragon>();
			m_summonDragon.Init(GetGameObject(), hitInfo, skillZero.summonTime - 1f, skillZero.damageInterval);
			m_summonDragon.DragonEffect.SetActive(false);
			GameObject gameObject3 = new GameObject();
			gameObject3.name = "NinjiaBullet";
			gameObject3.transform.parent = BattleBufferManager.s_objectRoot.transform;
			DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(base.playerData.heroIndex);
			DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(heroDataByIndex.weaponType);
			DS2ObjectBuffer dS2ObjectBuffer = new DS2ObjectBuffer(10);
			int bulletType = weaponDataByType.bulletType;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(bulletType);
			for (int i = 0; i < dS2ObjectBuffer.Size; i++)
			{
				Bullet bullet = new Bullet();
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject4 = bullet.GetGameObject();
				gameObject4.AddComponent<LinearMoveToDestroy>();
				gameObject4.AddComponent<BulletTriggerScript>();
				gameObject4.SetActive(false);
				bullet.GetTransform().parent = gameObject3.transform;
				dS2ObjectBuffer.AddObj(bullet);
			}
			BattleBufferManager.Instance.AddObjectToPreloadBuffer(BattleBufferManager.PreLoadObjectBufferType.NinjiaKillSpawnBulletIce, dS2ObjectBuffer);
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_SKILL_SHADOW_HIT_ICE);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.NINJIA_01_ATTACK_ICE_SWING);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.NINJIA_01_ATTACK_BULLET);
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		protected override void SetAnimationsMixing()
		{
			Transform mix = m_transform.Find("Bip01/Spine_00/Bip01 Spine");
			if (base.name == null || base.name == string.Empty)
			{
				return;
			}
			DataConf.AnimData newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Hurt");
			if (newCharacterAnim.count > 1)
			{
				for (int i = 0; i < newCharacterAnim.count; i++)
				{
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (i + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (i + 1)].AddMixingTransform(mix);
				}
			}
			else
			{
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			}
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shift");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shoting");
			for (int j = 0; j < 3; j++)
			{
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (j + 1)].layer = 2;
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (j + 1)].AddMixingTransform(mix);
			}
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "ReadyFire");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
		}

		public void OnAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				AIState aIState = GetAIState("Shoot");
				aIState.Push(GetAIState("LowerMove"));
				m_emitTimer = -1f;
				GetModelTransform().forward = FaceDirection;
				base.hitInfo.repelDistance = m_weapon.attribute.repelDis;
				base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
				m_weapon.m_firePoint = m_firePoint;
				break;
			}
			case AIState.AIPhase.Update:
				if (base.CurrentController && !base.AutoControl && !base.HalfAutoControl && !m_fire)
				{
					if (!AnimationPlaying(base.animUpperBody))
					{
						ChangeAIState("FireReady");
					}
					break;
				}
				if (!base.CurrentController || base.AutoControl || base.HalfAutoControl)
				{
					DS2ActiveObject nearestObjFromTargetList;
					if (DataCenter.State().isPVPMode)
					{
						nearestObjFromTargetList = base.lockedTarget;
						if (nearestObjFromTargetList == null)
						{
							nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
						}
					}
					else
					{
						nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
					}
					if (nearestObjFromTargetList != null && nearestObjFromTargetList.Alive())
					{
						LookAt(nearestObjFromTargetList.GetTransform());
						float sqrMagnitude = (nearestObjFromTargetList.GetTransform().position - GetTransform().position).sqrMagnitude;
						float num = base.shootRange * base.shootRange;
						if (sqrMagnitude > num)
						{
							ChangeToDefaultAIState();
						}
					}
					else
					{
						ChangeToDefaultAIState();
					}
				}
				if (m_emitTimer != -1f && AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer += Time.deltaTime;
					if (m_emitTimer >= m_emitTime)
					{
						m_emitTimer = -1f;
						m_weapon.UpdateFire(Time.deltaTime);
					}
				}
				if (m_weapon.firePermit() && m_emitTimer == -1f && !AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer = 0f;
					base.animUpperBody = GetAnimationNameByWeapon("Shoting", 2);
					float num2 = AnimationLength(base.animUpperBody) / m_weapon.attribute.fireFrequency;
					SetAnimationSpeed(base.animUpperBody, num2);
					if (m_iRankAnimIndex == 0)
					{
						m_weapon.EffectFireStart(0);
						m_weapon.SetEffectPlaySpeed(0, num2);
					}
					else if (m_iRankAnimIndex == 2)
					{
						m_weapon.EffectFireStart(1);
						m_weapon.SetEffectPlaySpeed(1, num2);
					}
					AnimationCrossFade(base.animUpperBody, false);
				}
				break;
			case AIState.AIPhase.Exit:
				SetAttackCollider(false);
				if (m_weapon != null)
				{
					m_weapon.StopFire();
				}
				SetAttackCollider(false);
				break;
			}
		}

		public void OnMoveAttack(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				AIState aIState = GetAIState("MoveShoot");
				aIState.Push(GetAIState("LowerMove"));
				m_emitTimer = -1f;
				m_originalSpeed = MoveSpeed;
				m_reduceSpeed = 0f;
				MoveSpeed -= m_reduceSpeed;
				GetModelTransform().forward = FaceDirection;
				base.hitInfo.repelDistance = m_weapon.attribute.repelDis;
				base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
				m_weapon.m_firePoint = m_firePoint;
				break;
			}
			case AIState.AIPhase.Update:
				if (base.CurrentController && !base.AutoControl && !base.HalfAutoControl && !m_fire)
				{
					if (!AnimationPlaying(base.animUpperBody))
					{
						ChangeAIState("FireReady");
					}
					break;
				}
				if (!base.CurrentController || base.AutoControl || base.HalfAutoControl)
				{
					DS2ActiveObject nearestObjFromTargetList;
					if (DataCenter.State().isPVPMode)
					{
						nearestObjFromTargetList = base.lockedTarget;
						if (nearestObjFromTargetList == null)
						{
							nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
						}
					}
					else
					{
						nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
					}
					if (nearestObjFromTargetList != null && nearestObjFromTargetList.Alive())
					{
						LookAt(nearestObjFromTargetList.GetTransform());
						float sqrMagnitude = (nearestObjFromTargetList.GetTransform().position - GetTransform().position).sqrMagnitude;
						float num = base.shootRange * base.shootRange;
						if (sqrMagnitude > num)
						{
							ChangeToDefaultAIState();
						}
					}
					else
					{
						ChangeToDefaultAIState();
					}
				}
				if (m_emitTimer != -1f && AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer += Time.deltaTime;
					if (m_emitTimer >= m_emitTime)
					{
						m_emitTimer = -1f;
						m_weapon.UpdateFire(Time.deltaTime);
					}
				}
				if (m_weapon.firePermit() && m_emitTimer == -1f && !AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer = 0f;
					base.animUpperBody = GetAnimationNameByWeapon("Shoting", 2);
					float num2 = AnimationLength(base.animUpperBody) / m_weapon.attribute.fireFrequency;
					SetAnimationSpeed(base.animUpperBody, num2);
					AnimationCrossFade(base.animUpperBody, false);
					if (m_iRankAnimIndex == 0)
					{
						m_weapon.EffectFireStart(0);
						m_weapon.SetEffectPlaySpeed(0, num2);
					}
					else if (m_iRankAnimIndex == 2)
					{
						m_weapon.EffectFireStart(1);
						m_weapon.SetEffectPlaySpeed(1, num2);
					}
				}
				break;
			case AIState.AIPhase.Exit:
				MoveSpeed += m_reduceSpeed;
				SetAttackCollider(false);
				if (m_weapon != null)
				{
					m_weapon.StopFire();
				}
				SetAttackCollider(false);
				break;
			}
		}

		public void OnUpperFire(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				AIState aIState = GetAIState("Shoot");
				m_emitTimer = -1f;
				m_originalSpeed = MoveSpeed;
				m_reduceSpeed = 0f;
				MoveSpeed -= m_reduceSpeed;
				GetModelTransform().forward = FaceDirection;
				base.hitInfo.repelDistance = m_weapon.attribute.repelDis;
				base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
				m_weapon.m_firePoint = m_firePoint;
				break;
			}
			case AIState.AIPhase.Update:
				if (base.CurrentController && !base.AutoControl && !base.HalfAutoControl && !m_fire)
				{
					if (!AnimationPlaying(base.animUpperBody))
					{
						ChangeAIState("FireReady");
					}
					break;
				}
				if (!base.CurrentController || base.AutoControl || base.HalfAutoControl)
				{
					DS2ActiveObject nearestObjFromTargetList;
					if (DataCenter.State().isPVPMode)
					{
						nearestObjFromTargetList = base.lockedTarget;
						if (nearestObjFromTargetList == null)
						{
							nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
						}
					}
					else
					{
						nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
					}
					if (nearestObjFromTargetList != null && nearestObjFromTargetList.Alive())
					{
						LookAt(nearestObjFromTargetList.GetTransform());
					}
				}
				if (m_emitTimer != -1f && AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer += Time.deltaTime;
					if (m_emitTimer >= m_emitTime)
					{
						m_emitTimer = -1f;
						m_weapon.UpdateFire(Time.deltaTime);
					}
				}
				if (m_weapon.firePermit() && m_emitTimer == -1f && !AnimationPlaying(base.animUpperBody))
				{
					m_emitTimer = 0f;
					base.animUpperBody = GetAnimationNameByWeapon("Shoting", 2);
					float num = AnimationLength(base.animUpperBody) / m_weapon.attribute.fireFrequency;
					SetAnimationSpeed(base.animUpperBody, num);
					AnimationCrossFade(base.animUpperBody, false);
					if (m_iRankAnimIndex == 0)
					{
						m_weapon.EffectFireStart(0);
						m_weapon.SetEffectPlaySpeed(0, num);
					}
					else if (m_iRankAnimIndex == 2)
					{
						m_weapon.EffectFireStart(1);
						m_weapon.SetEffectPlaySpeed(1, num);
					}
				}
				break;
			case AIState.AIPhase.Exit:
				MoveSpeed += m_reduceSpeed;
				SetAttackCollider(false);
				if (m_weapon != null)
				{
					m_weapon.StopFire();
				}
				break;
			}
		}

		public void OnSkillSummonDragon(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				base.isRage = true;
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				SetGodTime(float.PositiveInfinity);
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					ChangeToDefaultAIState();
				}
				break;
			case AIState.AIPhase.Exit:
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = false;
				}
				break;
			}
		}

		private void SummonDragonSkill()
		{
			m_effectFlash.gameObject.SetActive(true);
			m_effectFlash.StartEmit();
			m_summonDragon.DragonAppear();
			m_audioSkill.Trigger();
		}

		public override bool CheckSkillConditions()
		{
			if (SkillInCDTime())
			{
				return false;
			}
			bool result = false;
			int layerMask = ((base.clique != 0) ? 1536 : 2048);
			Collider[] array = Physics.OverlapSphere(GetTransform().position, 10f, layerMask);
			int num = array.Length;
			if (DataCenter.State().isPVPMode)
			{
				if (num >= 1)
				{
					m_checkSkillTimer += Time.deltaTime;
					m_checkSkillTimer = 0f;
					if (Random.Range(0, 100) < 40)
					{
						result = true;
					}
				}
			}
			else if (num > 8)
			{
				result = true;
			}
			return result;
		}

		public override void OnDeath()
		{
			m_summonDragon.DragonDisappear();
			base.OnDeath();
		}
	}
}
