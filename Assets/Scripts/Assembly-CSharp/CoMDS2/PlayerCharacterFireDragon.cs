using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterFireDragon : Player
	{
		public enum NinjiaType
		{
			Fire = 0,
			Ice = 1
		}

		private enum SkillState
		{
			Ready = 0,
			Update = 1,
			End = 2
		}

		private NinjiaType m_ninjiaType;

		private float m_emitTime = 0.3f;

		private float m_emitTimer;

		private float m_originalSpeed;

		private float m_reduceSpeed;

		private Character[] m_skillTargets;

		private Transform m_firePoint;

		private EffectParticleContinuous m_effectShadowClosing;

		private EffectControl[] m_effectShadowFireX;

		private EffectPlayedByAnimation m_effectShadowSwipe;

		private Vector3 m_skillBeginPosition;

		private Vector3 m_skillEndPosition;

		private float m_skillMoveDistance = 8f;

		private float m_skillMoveCurrDis;

		private float m_skillDamageRadius = 2f;

		private SkillState m_skillState;

		private float m_skillEffectClosingTimer;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Shoot");
			aIState.SetCustomFunc(OnAttack);
			AIState aIState2 = new AIState(this, "MoveShoot");
			aIState2.SetCustomFunc(OnMoveAttack);
			AIState aIState3 = new AIState(this, "Skill");
			aIState3.SetCustomFunc(OnSkillShadowKill);
			AIState aIState4 = new AIState(this, "UpperFire");
			aIState4.SetCustomFunc(OnUpperFire);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(7f);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIState3.name, aIState3);
			AddAIState(aIState4.name, aIState4);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			m_firePoint = GetTransform().Find("FirePoint");
			GameObject gameObject = GetTransform().Find("EffectShadowX").gameObject;
			m_effectShadowFireX = gameObject.GetComponentsInChildren<EffectControl>();
			for (int i = 0; i < m_effectShadowFireX.Length; i++)
			{
				m_effectShadowFireX[i].Root.SetActive(false);
			}
			GameObject gameObject2 = GetTransform().Find("EffectShadowSwipe").gameObject;
			m_effectShadowSwipe = gameObject2.GetComponentInChildren<EffectPlayedByAnimation>();
			m_effectShadowSwipe.gameObject.SetActive(false);
			GameObject gameObject3 = GetTransform().Find("EffectShadowClosing").gameObject;
			m_effectShadowClosing = gameObject3.GetComponent<EffectParticleContinuous>();
			m_effectShadowClosing.gameObject.SetActive(false);
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
			DataConf.SkillFireDragon skillFireDragon = (DataConf.SkillFireDragon)(base.skillInfo = (DataConf.SkillFireDragon)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillFireDragon.GetATK();
			base.skillHitInfo.damage = aTK;
			m_skillMoveDistance = skillFireDragon.dashDistance;
			m_skillDamageRadius = 1.5f;
			base.skillHitInfo.repelDistance = skillFireDragon.repelDis;
			base.skillHitInfo.repelTime = 0.2f;
			GameObject gameObject4 = new GameObject();
			gameObject4.name = "NinjiaBullet";
			gameObject4.transform.parent = BattleBufferManager.s_objectRoot.transform;
			DataConf.HeroData heroDataByIndex = DataCenter.Conf().GetHeroDataByIndex(base.playerData.heroIndex);
			DataConf.WeaponData weaponDataByType = DataCenter.Conf().GetWeaponDataByType(heroDataByIndex.weaponType);
			DS2ObjectBuffer dS2ObjectBuffer = new DS2ObjectBuffer(10);
			int bulletType = weaponDataByType.bulletType;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(bulletType);
			for (int j = 0; j < dS2ObjectBuffer.Size; j++)
			{
				Bullet bullet = new Bullet();
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject5 = bullet.GetGameObject();
				gameObject5.AddComponent<LinearMoveToDestroy>();
				gameObject5.AddComponent<BulletTriggerScript>();
				gameObject5.SetActive(false);
				bullet.GetTransform().parent = gameObject4.transform;
				dS2ObjectBuffer.AddObj(bullet);
			}
			BattleBufferManager.Instance.AddObjectToPreloadBuffer(BattleBufferManager.PreLoadObjectBufferType.NinjiaKillSpawnBulletFire, dS2ObjectBuffer);
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_SKILL_SHADOW_HIT_FIRE);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.NINJIA_ATTACK_FIRE_SWING);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.NINJIA_ATTACK_FIRE_BULLET);
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

		public void OnSkillShadowKill(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				stateToSkillReady();
				m_weapon.StopFire();
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				base.isRage = true;
				break;
			case AIState.AIPhase.Update:
				switch (m_skillState)
				{
				case SkillState.Ready:
					updateStateReady();
					break;
				case SkillState.Update:
					updateStateUpdate();
					break;
				case SkillState.End:
					updateStateEnd();
					break;
				}
				break;
			case AIState.AIPhase.Exit:
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = false;
				}
				SetAttackCollider(false);
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				break;
			}
		}

		private void stateToSkillReady()
		{
			AnimationStop(base.animUpperBody);
			base.animLowerBody = "Ninja_Skill_A";
			AnimationCrossFade(base.animLowerBody, false);
			m_skillState = SkillState.Ready;
			m_skillBeginPosition = GetTransform().position;
		}

		private void updateStateReady()
		{
			if (!AnimationPlaying(base.animLowerBody))
			{
				stateToSkillUpdate();
			}
		}

		private void stateToSkillUpdate()
		{
			base.animLowerBody = "Ninja_Skill_B";
			AnimationCrossFade(base.animLowerBody, true, false, 0.1f);
			m_skillState = SkillState.Update;
			m_skillMoveCurrDis = 0f;
			int layerMask = ((base.clique != 0) ? 1536 : 2048);
			Ray ray = new Ray(m_effectPoint.position, GetModelTransform().forward);
			RaycastHit[] array = Physics.SphereCastAll(ray, m_skillDamageRadius, m_skillMoveDistance + 2f, layerMask);
			int num = array.Length;
			m_skillTargets = null;
			m_skillTargets = new Character[num];
			for (int i = 0; i < num; i++)
			{
				m_skillTargets[i] = DS2ObjectStub.GetObject<Character>(array[i].collider.gameObject);
				if (m_skillTargets[i].Alive() && !m_skillTargets[i].isRage)
				{
					m_skillTargets[i].SwitchFSM(m_skillTargets[i].GetAIState("Idle"));
				}
			}
		}

		private void updateStateUpdate()
		{
			if (GameBattle.m_instance != null)
			{
				float num = 180f * Time.deltaTime;
				m_skillMoveCurrDis += num;
				m_characterController.Move(FaceDirection.normalized * num);
				if (!(m_skillMoveCurrDis >= m_skillMoveDistance))
				{
					return;
				}
				for (int num2 = m_skillTargets.Length - 1; num2 >= 0; num2--)
				{
					Character character = m_skillTargets[num2];
					if (character.Alive())
					{
						character.OnHit(base.skillHitInfo);
						if (m_ninjiaType == NinjiaType.Fire)
						{
							BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_SKILL_SHADOW_HIT_FIRE, character.m_effectPointFoward.position);
						}
						else
						{
							BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_SKILL_SHADOW_HIT_ICE, character.m_effectPointFoward.position);
						}
					}
				}
				stateToSkillEnd();
			}
			else
			{
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_SKILL_SHADOW_HIT_FIRE, m_effectPoint.position);
				stateToSkillEnd();
			}
		}

		private void stateToSkillEnd()
		{
			base.animLowerBody = "Ninja_Skill_C";
			AnimationCrossFade(base.animLowerBody, false);
			m_skillState = SkillState.End;
			m_effectShadowSwipe.gameObject.SetActive(true);
			m_effectShadowSwipe.StartEmit();
			m_effectShadowClosing.gameObject.SetActive(true);
			m_effectShadowClosing.StartEmit();
			m_skillEndPosition = GetTransform().position;
			m_skillEffectClosingTimer = 0f;
			Vector3 position = (m_skillEndPosition + m_skillBeginPosition) / 2f;
			m_effectShadowFireX[0].transform.position = position;
			if (GameBattle.m_instance == null)
			{
				m_effectShadowFireX[0].transform.position = m_effectPoint.position;
			}
			for (int i = 0; i < m_effectShadowFireX.Length; i++)
			{
				m_effectShadowFireX[i].Root.SetActive(true);
				m_effectShadowFireX[i].StartEmit();
			}
		}

		private void updateStateEnd()
		{
			if (!AnimationPlaying(base.animLowerBody))
			{
				ChangeToDefaultAIState();
			}
		}

		public override bool CheckSkillConditions()
		{
			if (SkillInCDTime())
			{
				return false;
			}
			bool result = false;
			int layerMask = ((base.clique != 0) ? 1536 : 2048);
			Ray ray = new Ray(m_effectPoint.position, FaceDirection);
			RaycastHit[] array = Physics.SphereCastAll(ray, 1.5f, m_skillMoveDistance, layerMask);
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
			else if (num > 5)
			{
				result = true;
			}
			return result;
		}
	}
}
