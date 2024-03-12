using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterVasily : Player
	{
		private VasilyEmplacement m_emplacement;

		private Transform m_skillEmplacementPointForward;

		private Transform m_skillEmplacementPointBack;

		private float m_emplacementAppearTimer;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillEmplacement);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			m_skillEmplacementPointForward = GetTransform().Find("SkillEmplacementPointForward").transform;
			m_skillEmplacementPointBack = GetTransform().Find("SkillEmplacementPointBack").transform;
			DataConf.SkillVasily skillVasily = (DataConf.SkillVasily)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar);
			base.skillInfo = skillVasily;
			m_skillTotalCDTime = base.skillInfo.CDTime;
			m_emplacement = new VasilyEmplacement(this);
			GameObject prefab2 = Resources.Load<GameObject>("Models/ActiveObjects/Emplacement02/Emplacement02");
			m_emplacement.Initialize(prefab2, "VasilyEmplacement", Vector3.zero, Quaternion.identity, 0);
			m_emplacement.SetActive(false);
			if (HasTalent(TeamSpecialAttribute.TeamAttributeType.ProlongedFirepower))
			{
				float num = DataCenter.Conf().m_teamAttributeProlongedFirepower.skillEmplacementTimePercent[base.teamData.talents[TeamSpecialAttribute.TeamAttributeType.ProlongedFirepower] - 1];
				m_emplacement.Duration += m_emplacement.Duration * num;
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public void OnSkillEmplacement(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				m_emplacementAppearTimer = 0f;
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				if (m_weapon != null)
				{
					m_weapon.SetActive(false);
				}
				base.isRage = true;
				SetGodTime(float.PositiveInfinity);
				break;
			case AIState.AIPhase.Update:
				m_emplacementAppearTimer += Time.deltaTime;
				if (m_emplacementAppearTimer >= 0.3f && m_emplacementAppearTimer != -1f)
				{
					m_emplacementAppearTimer = -1f;
					PutEmplacement();
				}
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
				m_emplacement.StateToGuard();
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				break;
			}
		}

		private void PutEmplacement()
		{
			if (m_emplacement == null)
			{
				return;
			}
			IPathFinding pathFinding = GetPathFinding();
			if (pathFinding != null)
			{
				if (pathFinding.HasNavigation())
				{
					UnityEngine.AI.NavMeshHit hit;
					pathFinding.GetNavMeshAgent().SamplePathPosition(-1, 2f, out hit);
					if (hit.distance != 0f)
					{
						m_emplacement.GetTransform().position = m_skillEmplacementPointBack.position;
					}
					else
					{
						m_emplacement.GetTransform().position = m_skillEmplacementPointForward.position;
					}
				}
				else
				{
					m_emplacement.GetTransform().position = new Vector3(GetTransform().position.x - 3f, GetTransform().position.y, GetTransform().position.z);
				}
			}
			m_emplacement.GetModelTransform().forward = GetModelTransform().forward;
			m_emplacement.SetActive(true);
			ChangeAIState("Skill");
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddObjToInteractObjectList(m_emplacement);
			}
			else
			{
				BattleBufferManager.Instance.AddObjToInteractObjectListForUIExhibition(m_emplacement);
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
			else if (num > 10)
			{
				result = true;
			}
			return result;
		}
	}
}
