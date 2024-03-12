using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterLili : Player
	{
		private float m_skillExtraHeal;

		private ITAudioEvent m_audioSkill;

		private DS2ActiveObject[] m_healTargets;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillHeal);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			DataConf.SkillLili skillLili = (DataConf.SkillLili)(base.skillInfo = (DataConf.SkillLili)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			m_skillExtraHeal = skillLili.GetBR();
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_BEGIN, 5);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_COVER, 5);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_END, 5);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_ADD_HP, 10);
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.Bloodthirst, 5);
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public void OnSkillHeal(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_audioSkill.Trigger();
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(false);
				}
				if (GameBattle.m_instance != null)
				{
					m_healTargets = GameBattle.m_instance.GetTeammateList(base.clique);
					for (int j = 0; j < m_healTargets.Length; j++)
					{
						Character character2 = (Character)m_healTargets[j];
						if (character2.Alive())
						{
							IBuffManager buffManager3 = character2.GetBuffManager();
							if (buffManager3 != null)
							{
								Buff buff3 = new Buff(Buff.AffectType.SkillAddHp, ((DataConf.SkillLili)base.skillInfo).GetHealPercent(), ((DataConf.SkillLili)base.skillInfo).healInterval, ((DataConf.SkillLili)base.skillInfo).healTime, Buff.CalcType.Percentage, m_skillExtraHeal);
								buffManager3.AddBuff(buff3);
							}
						}
					}
				}
				else
				{
					IBuffManager buffManager4 = GetBuffManager();
					if (buffManager4 != null)
					{
						Buff buff4 = new Buff(Buff.AffectType.SkillAddHp, ((DataConf.SkillLili)base.skillInfo).GetHealPercent(), ((DataConf.SkillLili)base.skillInfo).healInterval, ((DataConf.SkillLili)base.skillInfo).healTime, Buff.CalcType.Percentage, m_skillExtraHeal);
						GetBuffManager().AddBuff(buff4);
					}
				}
				base.isRage = true;
				SetGodTime(float.PositiveInfinity);
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					ChangeToDefaultAIState();
				}
				break;
			case AIState.AIPhase.Exit:
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = false;
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(true);
				}
				if (GameBattle.m_instance != null)
				{
					for (int i = 0; i < m_healTargets.Length; i++)
					{
						Character character = (Character)m_healTargets[i];
						if (character.Alive())
						{
							IBuffManager buffManager = character.GetBuffManager();
							if (buffManager != null)
							{
								Buff buff = new Buff(Buff.AffectType.ReduceDamage, ((DataConf.SkillLili)base.skillInfo).GetDR(), 0f, ((DataConf.SkillLili)base.skillInfo).healTime, Buff.CalcType.General, 0f);
								buffManager.AddBuff(buff);
							}
						}
					}
				}
				else
				{
					IBuffManager buffManager2 = GetBuffManager();
					if (buffManager2 != null)
					{
						Buff buff2 = new Buff(Buff.AffectType.ReduceDamage, ((DataConf.SkillLili)base.skillInfo).GetDR(), 0f, ((DataConf.SkillLili)base.skillInfo).healTime, Buff.CalcType.General, 0f);
						buffManager2.AddBuff(buff2);
					}
				}
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
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
			DS2ActiveObject[] teammateList = GameBattle.m_instance.GetTeammateList(base.clique);
			int num = teammateList.Length;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				num2 += teammateList[i].hpPercent;
			}
			if (DataCenter.State().isPVPMode)
			{
				if (num2 <= 0.7f)
				{
					result = true;
				}
			}
			else if (num2 <= 0.5f)
			{
				result = true;
			}
			return result;
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			WeaponPistolDoctor weaponPistolDoctor = (WeaponPistolDoctor)m_weapon;
			weaponPistolDoctor.handType = WeaponPistol.HandType.Single;
			weaponPistolDoctor.GetLeftGun().SetActive(false);
		}
	}
}
