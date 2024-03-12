using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterEva : Player
	{
		private EffectParticleContinuous m_PolluteStartEffect;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillPollute);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			Transform transform = GetTransform().Find("Eva_Start");
			m_PolluteStartEffect = transform.GetComponentInChildren<EffectParticleContinuous>();
			m_PolluteStartEffect.gameObject.SetActive(false);
			base.skillInfo = DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar);
			m_skillTotalCDTime = base.skillInfo.CDTime;
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.Eva_Pollution, 15);
		}

		public void OnSkillPollute(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
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
				m_PolluteStartEffect.gameObject.SetActive(true);
				m_PolluteStartEffect.StartEmit();
				if (GameBattle.m_instance != null)
				{
					DataConf.SkillEva skillEva = (DataConf.SkillEva)base.skillInfo;
					DS2ActiveObject[] enemyList = GameBattle.m_instance.GetEnemyList();
					for (int i = 0; i < enemyList.Length; i++)
					{
						Character character = (Character)enemyList[i];
						IBuffManager buffManager = character.GetBuffManager();
						if (buffManager != null)
						{
							Buff buff = new Buff(Buff.AffectType.MoveSpeed, skillEva.GetTargetMoveSpeedDecrease(), skillEva.debuffTime, skillEva.debuffTime, Buff.CalcType.Percentage, 0f);
							buff.effect = Defined.EFFECT_TYPE.Eva_Pollution;
							buff.effect_bindTransform = character.m_effectPointUpHead;
							buffManager.AddBuff(buff);
							buff = new Buff(Buff.AffectType.HitRate, skillEva.GetTargetHitRateDecrease(), skillEva.debuffTime, skillEva.debuffTime, Buff.CalcType.Percentage, 0f);
							buffManager.AddBuff(buff);
							buff = new Buff(Buff.AffectType.AddATK, skillEva.GetTargetAtkDecrease(), skillEva.debuffTime, skillEva.debuffTime, Buff.CalcType.Percentage, 0f);
							buffManager.AddBuff(buff);
						}
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
			int layerMask = ((base.clique != 0) ? 1536 : 2048);
			Ray ray = new Ray(m_effectPoint.position, GetModelTransform().forward);
			RaycastHit[] array = Physics.SphereCastAll(ray, 2f, 4f, layerMask);
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

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
			weaponPistol.handType = WeaponPistol.HandType.Single;
			weaponPistol.GetLeftGun().SetActive(false);
			Buff item = new Buff(Buff.AffectType.AddDamage, m_weapon.attribute.extra[1], 0f, m_weapon.attribute.extra[0], Buff.CalcType.Percentage, 0f);
			base.hitInfo.buffs.Add(item);
		}
	}
}
