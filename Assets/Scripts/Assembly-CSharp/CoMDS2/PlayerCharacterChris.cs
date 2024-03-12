using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterChris : Player
	{
		private GameObject m_shield;

		private EffectParticleContinuous m_effectDash;

		private float m_fSkillDashDistance;

		private float m_fSkillDashTime;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			m_fSkillDashDistance = 12f;
			m_fSkillDashTime = 0.5f;
			AIStateSkillDash aIStateSkillDash = new AIStateSkillDash(this, "Skill", AIState.Controller.Player);
			aIStateSkillDash.SetDash(m_fSkillDashDistance, m_fSkillDashTime);
			aIStateSkillDash.SetCustomFunc(OnSkillDash);
			AIState aIState = new AIState(this, "SkillReady");
			aIState.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(4f);
			AddAIState(aIStateSkillDash.name, aIStateSkillDash);
			AddAIState(aIState.name, aIState);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			m_shield = GetTransform().Find("Chris_item").gameObject;
			if (m_shield != null)
			{
				m_shield.SetActive(false);
			}
			GameObject gameObject = GetTransform().Find("EffectSprint").gameObject;
			m_effectDash = gameObject.GetComponent<EffectParticleContinuous>();
			m_effectDash.gameObject.SetActive(false);
			DataConf.SkillChris skillChris = (DataConf.SkillChris)(base.skillInfo = (DataConf.SkillChris)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			m_fSkillDashDistance = skillChris.dashDistance;
			m_fSkillDashTime = skillChris.dashTime;
			NumberSection<float> aTK = skillChris.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillChris.repelDis;
			base.skillHitInfo.deadRepelDistance = skillChris.repelDis;
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

		public void OnSkillDash(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				if (m_shield != null)
				{
					m_shield.SetActive(true);
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(false);
				}
				m_weapon.StopFire();
				SetGodTime(float.PositiveInfinity);
				m_effectDash.gameObject.SetActive(true);
				m_effectDash.StartEmit();
				break;
			case AIState.AIPhase.Exit:
				if (m_shield != null)
				{
					m_shield.SetActive(false);
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(true);
				}
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
			RaycastHit[] array = Physics.SphereCastAll(ray, 1.5f, m_fSkillDashDistance, layerMask);
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
