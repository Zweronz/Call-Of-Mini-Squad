using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterOppenheimer : Player
	{
		private GameObject m_frozenFistEffect;

		private float m_frozeNovaTime = 3f;

		private float m_novaPlus = 0.25f;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillFrozen);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			Transform transform = GetTransform().Find("Model/Bip01/Spine_00/Bip01 Spine/Spine_0/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand");
			m_frozenFistEffect = transform.gameObject;
			m_frozenFistEffect.SetActive(false);
			base.skillInfo = DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar);
			m_skillTotalCDTime = base.skillInfo.CDTime;
			DataConf.SkillOppenheimer skillOppenheimer = (DataConf.SkillOppenheimer)base.skillInfo;
			DataConf.SkillOppenheimerUpgrade skillOppenheimerUpgrade = skillOppenheimer.upgradePhaseList[skillOppenheimer.phase];
			NumberSection<float> aTK = skillOppenheimer.GetATK();
			m_frozeNovaTime = skillOppenheimer.frozeTime;
			m_novaPlus = skillOppenheimerUpgrade.novaPlus;
			base.skillHitInfo.damage = aTK;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public void OnSkillFrozen(AIState.AIPhase phase)
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
				m_frozenFistEffect.SetActive(true);
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

		private void OnFrozen()
		{
			effectPlayManager.PlayEffect("Frost nova");
			m_frozenFistEffect.SetActive(false);
			HitInfo hitInfo = new HitInfo();
			hitInfo.damage = base.skillHitInfo.damage;
			hitInfo.source = base.hitInfo.source;
			SpecialHitInfo specialHitInfo = new SpecialHitInfo();
			specialHitInfo.time = m_frozeNovaTime;
			specialHitInfo.specialHitParam = m_novaPlus;
			hitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.FROZEN_NOVA, specialHitInfo);
			int layerMask = 526336;
			Collider[] array = Physics.OverlapSphere(GetTransform().position, 10f, layerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
				if (@object == null)
				{
				}
				@object.OnHit(hitInfo);
			}
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			SpecialHitInfo specialHitInfo = new SpecialHitInfo();
			specialHitInfo.time = m_weapon.attribute.extra[1];
			specialHitInfo.specialHitProbability = (int)m_weapon.attribute.extra[0];
			specialHitInfo.specialHitParam = 0.15f;
			specialHitInfo.disposable = false;
			base.hitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.FROZEN_NOVA, specialHitInfo);
		}
	}
}
