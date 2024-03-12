using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterWesker : Player
	{
		private DS2ObjectBuffer m_bombBuff;

		private ArmerBomb[] m_bombs;

		private Transform[] m_skillBombPoints;

		private int m_iSearchPointIndex;

		private float m_fPutBombFrequency;

		private float m_timer;

		private int m_iCurrentBomb;

		private int m_iBombCount = 6;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill", AIState.Controller.Player);
			aIState.SetCustomFunc(OnSkillBomb);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			DataConf.SkillWesker skillWesker = (DataConf.SkillWesker)(base.skillInfo = (DataConf.SkillWesker)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			m_iBombCount = skillWesker.count;
			NumberSection<float> aTK = skillWesker.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillWesker.repelDis;
			base.skillHitInfo.deadRepelDistance = skillWesker.repelDis;
			m_bombs = new ArmerBomb[m_iBombCount];
			for (int i = 0; i < m_iBombCount; i++)
			{
				m_bombs[i] = new ArmerBomb(this);
				GameObject prefab2 = Resources.Load<GameObject>("Models/ActiveObjects/Bomb/Bomb");
				m_bombs[i].Initialize(prefab2, "ArmerBomb", Vector3.zero, Quaternion.identity, 0);
				m_bombs[i].SetActive(false);
			}
			m_fPutBombFrequency = 1f / (float)m_iBombCount;
			Transform transform = GetTransform().Find("SkillBombPoint");
			int childCount = transform.childCount;
			m_skillBombPoints = new Transform[childCount];
			for (int j = 0; j < childCount; j++)
			{
				m_skillBombPoints[j] = transform.GetChild(j);
			}
		}

		protected override void SetAnimationsMixing()
		{
			base.SetAnimationsMixing();
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public void OnSkillBomb(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				m_iCurrentBomb = 0;
				m_iSearchPointIndex = 0;
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
				m_timer += Time.deltaTime;
				if (m_timer >= m_fPutBombFrequency)
				{
					m_timer = 0f;
					PutBomb();
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
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				break;
			}
		}

		private void PutBomb()
		{
			if (m_iCurrentBomb < m_iBombCount)
			{
				m_bombs[m_iCurrentBomb].GetTransform().position = GetBombPoint();
				m_bombs[m_iCurrentBomb].GetModelTransform().forward = GetModelTransform().forward;
				m_bombs[m_iCurrentBomb].SetActive(true);
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.AddObjToInteractObjectList(m_bombs[m_iCurrentBomb]);
				}
				else
				{
					BattleBufferManager.Instance.AddObjToInteractObjectListForUIExhibition(m_bombs[m_iCurrentBomb]);
				}
				m_iCurrentBomb++;
			}
		}

		private Vector3 GetBombPoint()
		{
			return m_skillBombPoints[m_iSearchPointIndex++].position;
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
