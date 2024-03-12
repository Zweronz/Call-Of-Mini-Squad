using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterMike : Player
	{
		private GameObject m_bat;

		private float m_skillTimer;

		private EffectControl[] m_effectSwing;

		private ushort m_skillCounter;

		private ushort m_skillCount = 1;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill", AIState.Controller.Player);
			aIState.SetCustomFunc(OnSkillBat);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(4f);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			m_bat = GetTransform().Find("Baseball_bat").gameObject;
			if (m_bat != null)
			{
				m_bat.transform.parent = m_transform.Find("Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 Prop1");
				m_bat.transform.localPosition = Vector3.zero;
				m_bat.transform.localRotation = Quaternion.identity;
				m_bat.transform.localScale = Vector3.one;
				GameObject gameObject = m_bat.transform.Find("Avatar_Mike_L_Swing").gameObject;
				m_effectSwing = gameObject.GetComponentsInChildren<EffectControl>();
				for (int i = 0; i < m_effectSwing.Length; i++)
				{
					m_effectSwing[i].transform.parent = m_effectPoint;
					m_effectSwing[i].transform.localPosition = Vector3.zero;
					m_effectSwing[i].transform.localRotation = Quaternion.identity;
					m_effectSwing[i].gameObject.SetActive(false);
				}
				gameObject.SetActive(false);
				m_bat.SetActive(false);
			}
			DataConf.SkillMike skillMike = (DataConf.SkillMike)(base.skillInfo = (DataConf.SkillMike)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillMike.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillMike.repelDis;
			SpecialHitInfo specialHitInfo = new SpecialHitInfo();
			specialHitInfo.time = skillMike.stunTime;
			specialHitInfo.disposable = false;
			base.skillHitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.STUN, specialHitInfo);
			Buff item = new Buff(Buff.AffectType.MoveSpeed, skillMike.reduceSpeed, skillMike.reduceSpeedTime, skillMike.reduceSpeedTime, Buff.CalcType.Percentage, 0f);
			base.skillHitInfo.buffs.Add(item);
			base.skillHitInfo.bViolent = true;
		}

		protected override void SetAnimationsMixing()
		{
			base.SetAnimationsMixing();
			Transform mix = m_transform.Find("Bip01/Spine_00/Bip01 Spine");
			string animationName = GetAnimationName("Skill");
			m_gameObject.GetComponent<Animation>()[animationName].layer = 2;
			m_gameObject.GetComponent<Animation>()[animationName].AddMixingTransform(mix);
		}

		public void OnSkillBat(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				AIState aIState = GetAIState("Skill");
				aIState.Push(GetAIState("LowerMove"));
				if (m_bat != null)
				{
					m_bat.SetActive(true);
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(false);
				}
				m_weapon.StopFire();
				PlayerEffectSwint();
				base.animUpperBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animUpperBody, false);
				base.isRage = true;
				SetGodTime(float.PositiveInfinity);
				break;
			}
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animUpperBody))
				{
					m_skillCounter++;
					if (m_skillCounter >= m_skillCount)
					{
						m_skillCounter = 0;
						ChangeToDefaultAIState();
					}
					else
					{
						AnimationCrossFade(base.animUpperBody, false);
						PlayerEffectSwint();
					}
				}
				break;
			case AIState.AIPhase.Exit:
				if (m_bat != null)
				{
					m_bat.SetActive(false);
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(true);
				}
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				SetAttackCollider(false, AttackCollider.AttackColliderType.Bat);
				break;
			}
		}

		public void PlayerEffectSwint()
		{
			for (int i = 0; i < m_effectSwing.Length; i++)
			{
				m_effectSwing[i].StartEmit();
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
	}
}
