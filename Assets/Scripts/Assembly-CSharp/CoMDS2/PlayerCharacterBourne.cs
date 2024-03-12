using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterBourne : Player
	{
		private float m_skillTimer;

		private float m_skillTime = 3f;

		private int m_skillDamageCount = 8;

		private int m_skillDamageCounter;

		private float m_skillDamageRadius = 3f;

		private float m_skillDamageFrenquency = 0.37f;

		private EffectParticleContinuous m_effectSkillVolley;

		private EffectParticleContinuous m_effectSkillVolleyFireLeft;

		private EffectParticleContinuous m_effectSkillVolleyFireRight;

		private float m_effectSkillVolleyFireFrequency = 0.3f;

		private float m_effectSkillVolleyFireTimer = 0.3f;

		private int m_effectSkillVolleyFireMark;

		private float m_skillReduceSpeed = 0.4f;

		private float m_skillReduceSpeedTime = 5f;

		private Collider[] m_skillTargets;

		private float m_skillTimerDamage;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillVolley);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			GameObject gameObject = GetTransform().Find("EffectSkillVolley").gameObject;
			m_effectSkillVolley = gameObject.GetComponentInChildren<EffectParticleContinuous>();
			m_effectSkillVolley.gameObject.SetActive(false);
			GameObject gameObject2 = GetTransform().Find("EffectSkillVolleyFireLeft").gameObject;
			m_effectSkillVolleyFireLeft = gameObject2.GetComponentInChildren<EffectParticleContinuous>();
			m_effectSkillVolleyFireLeft.gameObject.SetActive(false);
			GameObject gameObject3 = GetTransform().Find("EffectSkillVolleyFireRight").gameObject;
			m_effectSkillVolleyFireRight = gameObject3.GetComponentInChildren<EffectParticleContinuous>();
			m_effectSkillVolleyFireRight.gameObject.SetActive(false);
			DataConf.SkillBourne skillBourne = (DataConf.SkillBourne)(base.skillInfo = (DataConf.SkillBourne)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillBourne.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillBourne.repelDis;
			base.skillHitInfo.deadRepelDistance = skillBourne.repelDis;
			m_skillTime = skillBourne.shootTime;
			m_skillDamageCount = skillBourne.damageCount;
			m_skillDamageRadius = skillBourne.damageRadius;
			m_skillReduceSpeed = skillBourne.debuffMoveSpeed;
			m_skillReduceSpeedTime = skillBourne.debuffTime;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_VOLLEY_FIRE);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_VOLLEY_HIT);
		}

		protected override void SetAnimationsMixing()
		{
			base.SetAnimationsMixing();
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
			weaponPistol.GetRightGun().SetBulletEmitOnTime(1);
			weaponPistol.handType = WeaponPistol.HandType.Single;
			weaponPistol.GetLeftGun().SetActive(false);
		}

		public void OnSkillVolley(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, true);
				m_weapon.StopFire();
				m_effectSkillVolley.gameObject.SetActive(true);
				m_effectSkillVolley.StartEmit();
				((WeaponPistol)m_weapon).GetLeftGun().SetActive(true);
				m_skillTimer = 0f;
				m_skillDamageCounter = 0;
				m_skillDamageFrenquency = m_skillTime / (float)(m_skillDamageCount + 1);
				m_skillTimerDamage = 0f;
				base.isRage = true;
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				SetGodTime(float.PositiveInfinity);
				break;
			case AIState.AIPhase.Update:
			{
				m_skillTimer += Time.deltaTime;
				if (m_skillTimer >= m_skillTime)
				{
					ChangeToDefaultAIState();
				}
				m_effectSkillVolleyFireTimer += Time.deltaTime;
				if (m_effectSkillVolleyFireTimer >= m_effectSkillVolleyFireFrequency)
				{
					m_effectSkillVolleyFireTimer = 0f;
					if (m_effectSkillVolleyFireMark == 0)
					{
						m_effectSkillVolleyFireMark = 1;
						m_effectSkillVolleyFireRight.StartEmit();
					}
					else
					{
						m_effectSkillVolleyFireMark = 0;
						m_effectSkillVolleyFireLeft.StartEmit();
					}
				}
				if (m_skillDamageCounter >= m_skillDamageCount)
				{
					break;
				}
				m_skillTimerDamage += Time.deltaTime;
				if (!(m_skillTimerDamage >= m_skillDamageFrenquency))
				{
					break;
				}
				m_skillTimerDamage = 0f;
				m_skillDamageCounter++;
				int layerMask = ((base.clique != 0) ? 1536 : 2048);
				m_skillTargets = Physics.OverlapSphere(GetTransform().position, m_skillDamageRadius, layerMask);
				if (m_skillTargets == null || m_skillTargets.Length <= 0)
				{
					break;
				}
				Collider[] skillTargets = m_skillTargets;
				foreach (Collider collider in skillTargets)
				{
					Character @object = DS2ObjectStub.GetObject<Character>(collider.gameObject);
					base.hitInfo.repelDirection = @object.GetTransform().position - GetTransform().position;
					base.hitInfo.source = this;
					if (m_skillDamageCounter == m_skillDamageCount)
					{
						base.skillHitInfo.repelDistance = ((DataConf.SkillBourne)base.skillInfo).repelDis;
						IBuffManager buffManager = @object.GetBuffManager();
						if (buffManager != null)
						{
							Buff buff = new Buff(Buff.AffectType.MoveSpeed, m_skillReduceSpeed, m_skillReduceSpeedTime, m_skillReduceSpeedTime, Buff.CalcType.General, 0f);
							buffManager.AddBuff(buff);
						}
					}
					else
					{
						base.skillHitInfo.repelDistance = new NumberSection<float>(0f, 0f);
						IBuffManager buffManager2 = @object.GetBuffManager();
						if (buffManager2 != null)
						{
							Buff buff2 = new Buff(Buff.AffectType.MoveSpeed, 0f, m_skillReduceSpeedTime, m_skillReduceSpeedTime, Buff.CalcType.General, 0f);
							buffManager2.AddBuff(buff2);
						}
					}
					@object.OnHit(base.skillHitInfo);
					BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_VOLLEY_HIT, @object.m_effectPointFoward.position);
				}
				break;
			}
			case AIState.AIPhase.Exit:
				((WeaponPistol)m_weapon).GetLeftGun().SetActive(false);
				m_effectSkillVolley.gameObject.SetActive(false);
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

		public override bool CheckSkillConditions()
		{
			if (SkillInCDTime())
			{
				return false;
			}
			bool result = false;
			int layerMask = ((base.clique != 0) ? 1536 : 2048);
			Collider[] array = Physics.OverlapSphere(GetTransform().position, m_skillDamageRadius, layerMask);
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
			else if (num > 4)
			{
				result = true;
			}
			return result;
		}
	}
}
