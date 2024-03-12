using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterRock : Player
	{
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
			aIStateSkillDash.SetDash(m_fSkillDashDistance, m_fSkillDashTime * 0.7f);
			aIStateSkillDash.SetCustomFunc(OnSkillDash);
			AIState aIState = new AIState(this, "SkillReady");
			aIState.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(4f);
			AddAIState(aIStateSkillDash.name, aIStateSkillDash);
			AddAIState(aIState.name, aIState);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			GameObject gameObject = GetTransform().Find("EffectSprint").gameObject;
			m_effectDash = gameObject.GetComponent<EffectParticleContinuous>();
			m_effectDash.gameObject.SetActive(false);
			DataConf.SkillRock skillRock = (DataConf.SkillRock)(base.skillInfo = (DataConf.SkillRock)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			m_fSkillDashDistance = skillRock.dashDistance;
			m_fSkillDashTime = skillRock.dashTime;
			NumberSection<float> aTK = skillRock.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillRock.repelDis;
			base.skillHitInfo.deadRepelDistance = skillRock.repelDis;
			base.skillHitInfo.bViolent = true;
		}

		protected override void SetAnimationsMixing()
		{
			base.SetAnimationsMixing();
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			m_weapon.GetTransform().Find("Model").GetComponent<MeshRenderer>()
				.enabled = false;
			m_weapon.GetTransform().localPosition += new Vector3(0.0787f, 0.021f, 0.37f);
		}

		public void OnSkillDash(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_weapon.StopFire();
				SetGodTime(float.PositiveInfinity);
				m_effectDash.gameObject.SetActive(true);
				m_effectDash.StartEmit();
				break;
			case AIState.AIPhase.Exit:
				AnimationPlay("Skill03", false);
				SkillEnd();
				SetGodTime(0f);
				m_effectDash.gameObject.SetActive(false);
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
