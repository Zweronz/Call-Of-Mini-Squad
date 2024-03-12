using System;
using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterTanya : Player
	{
		private enum TalkPhase
		{
			Appear = 0,
			Continued = 1,
			Disappear = 2
		}

		private EffectPlayedByAnimation m_effectTalkAppear;

		private EffectParticleContinuous m_effectTalkContinued;

		private EffectPlayedByAnimation m_effectTalkDisappear;

		private Transform m_effectTalkPoint;

		private GameObject m_talkAppearGO;

		private GameObject m_talkContinuedGO;

		private GameObject m_talkDisappearGO;

		private float m_tiemr;

		private SupermanFirepower[] m_firepowers;

		private int m_iFirepowerCount = 6;

		private int m_iCurrentFirepower;

		private bool m_bLanchFirepower;

		private float m_fLanchFirepowerFrequency = 0.2f;

		private ITAudioEvent m_audioSkill;

		private TalkPhase m_talkPhase;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill", AIState.Controller.Player);
			aIState.SetCustomFunc(OnSkillFirepower);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			m_effectTalkPoint = GetTransform().Find("EffectTalk");
			m_talkAppearGO = GetTransform().Find("EffectTalk/EffectTalkAppear").gameObject;
			m_effectTalkAppear = m_talkAppearGO.GetComponentInChildren<EffectPlayedByAnimation>();
			m_talkContinuedGO = GetTransform().Find("EffectTalk/EffectTalkContinued").gameObject;
			m_effectTalkContinued = m_talkContinuedGO.GetComponentInChildren<EffectParticleContinuous>();
			m_talkDisappearGO = GetTransform().Find("EffectTalk/EffectTalkDisappear").gameObject;
			m_effectTalkDisappear = m_talkDisappearGO.GetComponentInChildren<EffectPlayedByAnimation>();
			SetEffectTalkPosition();
			m_effectTalkAppear.gameObject.SetActive(false);
			m_effectTalkContinued.gameObject.SetActive(false);
			m_effectTalkDisappear.gameObject.SetActive(false);
			DataConf.SkillTanya skillTanya = (DataConf.SkillTanya)(base.skillInfo = (DataConf.SkillTanya)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			DataConf.SkillTanyaUpgrade skillTanyaUpgrade = skillTanya.upgradePhaseList[skillTanya.phase];
			NumberSection<float> aTK = skillTanya.GetATK();
			m_iFirepowerCount = skillTanyaUpgrade.fireCount;
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = skillTanya.repelDis;
			base.skillHitInfo.deadRepelDistance = skillTanya.repelDis;
			GameObject gameObject = GetTransform().Find("SupermanFirepower").gameObject;
			gameObject.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			m_firepowers = new SupermanFirepower[m_iFirepowerCount];
			for (int i = 0; i < m_iFirepowerCount; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				gameObject2.name = "SupermanFirepower";
				gameObject2.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
				m_firepowers[i] = new SupermanFirepower(this);
				m_firepowers[i].Initialize(gameObject2);
				m_firepowers[i].damageRadius = skillTanya.damageRadius;
				m_firepowers[i].SetActive(false);
			}
			UnityEngine.Object.DestroyImmediate(gameObject);
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (m_bLanchFirepower)
			{
				UpdateLanchFirepower();
			}
		}

		public void OnSkillFirepower(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_audioSkill.Trigger();
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationCrossFade(base.animLowerBody, false);
				m_weapon.StopFire();
				SetEffectTalkPosition();
				TalkStateToAppear();
				m_tiemr = 0f;
				m_iCurrentFirepower = 0;
				m_bLanchFirepower = true;
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
				switch (m_talkPhase)
				{
				case TalkPhase.Appear:
					UpdateTalkStateAppear();
					break;
				case TalkPhase.Continued:
					UpdateTalkStateContinued();
					break;
				case TalkPhase.Disappear:
					UpdateTalkStateDisappear();
					break;
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

		private void TalkStateToAppear()
		{
			m_talkPhase = TalkPhase.Appear;
			m_tiemr = 0f;
			m_effectTalkAppear.gameObject.SetActive(true);
			m_effectTalkAppear.StartEmit();
		}

		private void UpdateTalkStateAppear()
		{
			m_tiemr += Time.deltaTime;
			if ((double)m_tiemr >= 0.15)
			{
				m_effectTalkAppear.gameObject.SetActive(false);
				TalkStateToContinued();
			}
		}

		private void TalkStateToContinued()
		{
			m_talkPhase = TalkPhase.Continued;
			m_tiemr = 0f;
			m_effectTalkContinued.gameObject.SetActive(true);
			m_effectTalkContinued.StartEmit();
		}

		private void UpdateTalkStateContinued()
		{
			m_tiemr += Time.deltaTime;
			if (m_tiemr >= 2f)
			{
				m_effectTalkContinued.gameObject.SetActive(false);
				TalkStateToDisappear();
			}
		}

		private void TalkStateToDisappear()
		{
			m_talkPhase = TalkPhase.Disappear;
			m_tiemr = 0f;
			m_effectTalkDisappear.gameObject.SetActive(true);
			m_effectTalkDisappear.StartEmit();
		}

		private void UpdateTalkStateDisappear()
		{
			m_tiemr += Time.deltaTime;
			if (m_tiemr >= 0.15f)
			{
				m_effectTalkDisappear.gameObject.SetActive(false);
				ChangeToDefaultAIState();
			}
		}

		private void UpdateLanchFirepower()
		{
			m_tiemr += Time.deltaTime;
			if (m_tiemr >= m_fLanchFirepowerFrequency)
			{
				m_tiemr = 0f;
				m_firepowers[m_iCurrentFirepower].GetTransform().position = GetTransform().position + GetFirepowerPosition();
				m_firepowers[m_iCurrentFirepower].SetActive(true);
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.AddObjToInteractObjectList(m_firepowers[m_iCurrentFirepower]);
				}
				else
				{
					BattleBufferManager.Instance.AddObjToInteractObjectListForUIExhibition(m_firepowers[m_iCurrentFirepower]);
				}
				m_iCurrentFirepower++;
				if (m_iCurrentFirepower >= m_iFirepowerCount)
				{
					m_bLanchFirepower = false;
				}
			}
		}

		private Vector3 GetFirepowerPosition()
		{
			float num = UnityEngine.Random.Range(0f, 360f);
			Vector3 vector = new Vector3(Mathf.Cos(num * ((float)Math.PI / 180f)), 0.02f, Mathf.Sin(num * ((float)Math.PI / 180f)));
			return vector * UnityEngine.Random.Range(1f, 10f);
		}

		private void SetEffectTalkPosition()
		{
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
					if (UnityEngine.Random.Range(0, 100) < 40)
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
