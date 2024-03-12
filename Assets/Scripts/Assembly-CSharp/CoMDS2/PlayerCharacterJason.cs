using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterJason : Player
	{
		private int m_mineCount = 4;

		private float m_setMineRadius = 3f;

		private float m_mineExplodeRadius = 2.5f;

		private List<Mine> m_mineBuffer = new List<Mine>();

		private ITAudioEvent m_audioSkill;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillSetMine);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
			DataConf.SkillJason skillJason = (DataConf.SkillJason)(base.skillInfo = (DataConf.SkillJason)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillJason.GetATK();
			m_mineCount = skillJason.GetMINECOUNT();
			m_setMineRadius = skillJason.setRadius;
			m_mineExplodeRadius = skillJason.explodeRadius;
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = new NumberSection<float>(0f, 0f);
			base.skillHitInfo.deadRepelDistance = base.hitInfo.deadRepelDistance;
			CreateMine();
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		private void CreateMine()
		{
			m_mineBuffer = new List<Mine>();
			GameObject prefab = Resources.Load<GameObject>("Models/ActiveObjects/Mine/Mine");
			for (int i = 0; i < m_mineCount; i++)
			{
				Mine mine = new Mine();
				mine.Initialize(prefab, "Mine", Vector3.zero, Quaternion.identity, 14);
				mine.GetGameObject().SetActive(false);
				mine.GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
				HitInfo hitInfo = new HitInfo();
				hitInfo.damage = base.skillHitInfo.damage;
				mine.SetInfo(hitInfo, m_mineExplodeRadius);
				m_mineBuffer.Add(mine);
			}
		}

		public override void OnDeath()
		{
			base.OnDeath();
			for (int i = 0; i < m_mineBuffer.Count; i++)
			{
				Mine mine = m_mineBuffer[i];
				if (mine.GetGameObject().activeSelf)
				{
					mine.Detonate();
				}
			}
		}

		public void OnSkillSetMine(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_audioSkill.Trigger();
				m_weapon.SetActive(false);
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				base.isRage = true;
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				SetGodTime(float.PositiveInfinity);
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					SwitchFSM(GetAIState("FireReady"));
				}
				break;
			case AIState.AIPhase.Exit:
				m_weapon.SetActive(true);
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
			Ray ray = new Ray(m_effectPoint.position, GetModelTransform().forward);
			RaycastHit[] array = Physics.SphereCastAll(ray, 2f, 4f, layerMask);
			int num = array.Length;
			if (DataCenter.State().isPVPMode)
			{
				if (num >= 1)
				{
					m_checkSkillTimer += Time.deltaTime;
					m_checkSkillTimer = 0f;
					if (UnityEngine.Random.Range(0, 100) < 40)
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

		private void SetMine()
		{
			for (int i = 0; i < m_mineBuffer.Count; i++)
			{
				Mine mine = m_mineBuffer[i];
				if (mine.GetGameObject().activeSelf)
				{
					mine.Detonate();
				}
			}
			Transform transform = GetTransform();
			float num = 360f / (float)m_mineBuffer.Count;
			for (int j = 0; j < m_mineBuffer.Count; j++)
			{
				float num2 = m_setMineRadius;
				Quaternion quaternion = Quaternion.AngleAxis(num * (float)j, Vector3.up);
				Debug.DrawRay(transform.position, quaternion * transform.forward, Color.red, num2);
				RaycastHit raycastHit;
				if (Physics.Raycast(transform.position, quaternion * transform.forward, out raycastHit, num2, 327680))
				{
					num2 = Vector3.Distance(transform.position, raycastHit.point) - 0.5f;
					num2 = ((!(num2 < 0f)) ? num2 : 0f);
				}
				Mine mine2 = m_mineBuffer[j];
				mine2.GetTransform().rotation = transform.rotation;
				mine2.GetGameObject().SetActive(true);
				Vector3 vector = new Vector3(num2 * Mathf.Sin((float)Math.PI / 180f * num * (float)j), 0f, num2 * Mathf.Cos((float)Math.PI / 180f * num * (float)j));
				mine2.GetTransform().position = transform.position + transform.rotation * vector;
				mine2.SetMine();
			}
		}
	}
}
