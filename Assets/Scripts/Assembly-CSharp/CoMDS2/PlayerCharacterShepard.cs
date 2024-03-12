using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterShepard : Player
	{
		private Bullet m_blackholeBullet;

		private bool m_blackholeRepel;

		private float m_blackholeRepelTime = 0.3f;

		private float m_blackholeRepelDistance = 3f;

		private float m_blackholeRepelTimer;

		private ITAudioEvent m_audioSkill;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillBlackhole);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(8f);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
			DataConf.SkillShepard skillShepard = (DataConf.SkillShepard)(base.skillInfo = (DataConf.SkillShepard)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillShepard.GetATK();
			base.skillHitInfo.damage = aTK;
			SetBlackholeBullet(skillShepard);
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		private void SetBlackholeBullet(DataConf.SkillShepard skillShepard)
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(36);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			m_blackholeBullet = new Bullet(null);
			m_blackholeBullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 26);
			GameObject gameObject2 = m_blackholeBullet.GetGameObject();
			if (gameObject2.GetComponent<LinearMoveToDestroy>() == null)
			{
				gameObject2.AddComponent<LinearMoveToDestroy>();
			}
			BulletBlackholeLogic bulletBlackholeLogic = gameObject2.GetComponent<BulletBlackholeLogic>();
			if (bulletBlackholeLogic == null)
			{
				bulletBlackholeLogic = gameObject2.AddComponent<BulletBlackholeLogic>();
			}
			bulletBlackholeLogic.clique = base.clique;
			bulletBlackholeLogic.radius = skillShepard.radius;
			bulletBlackholeLogic.damageInterval = skillShepard.damageInterval;
			bulletBlackholeLogic.life = skillShepard.time;
			m_blackholeBullet.hitInfo.source = this;
			m_blackholeBullet.hitInfo.damage = base.skillHitInfo.damage;
			m_blackholeBullet.attribute = new Bullet.BulletAttribute();
			m_blackholeBullet.attribute.speed = skillShepard.speed;
			m_blackholeBullet.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
			gameObject2.SetActive(false);
			gameObject2.transform.parent = gameObject.transform;
		}

		private void EmitBlackholeBullet()
		{
			m_blackholeBullet.SetBullet(this, null, m_weapon.m_firePoint.position + Vector3.up, Quaternion.AngleAxis(m_weapon.m_firePoint.eulerAngles.y, Vector3.up));
			m_blackholeBullet.Emit();
		}

		public override void OnDeath()
		{
			base.OnDeath();
			m_blackholeRepel = false;
			if (m_blackholeBullet.GetGameObject().activeSelf)
			{
				m_blackholeBullet.Destroy();
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (m_blackholeRepel)
			{
				m_blackholeRepelTimer += deltaTime;
				if (m_blackholeRepelTimer >= m_blackholeRepelTime)
				{
					m_blackholeRepelTimer = 0f;
					m_blackholeRepel = false;
				}
				else if ((bool)m_characterController)
				{
					m_characterController.Move((GetTransform().forward * -1f).normalized * m_blackholeRepelDistance * deltaTime);
				}
				else
				{
					GetTransform().Translate((GetTransform().forward * -1f).normalized * m_blackholeRepelDistance * deltaTime, Space.World);
				}
			}
		}

		public void OnSkillBlackhole(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
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
					ChangeToDefaultAIState();
				}
				break;
			case AIState.AIPhase.Exit:
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

		private void ShootBlackhole()
		{
			if (GameBattle.m_instance != null)
			{
				AIStateRepel aIStateRepel = GetAIState("Repel") as AIStateRepel;
				if (aIStateRepel != null)
				{
					if (HasNavigation())
					{
						ResumeNav();
					}
					m_blackholeRepel = true;
					m_blackholeRepelTimer = 0f;
					m_blackholeRepelDistance = 9f;
					m_blackholeRepelTime = 0.3f;
				}
			}
			else
			{
				m_blackholeBullet.Destroy();
			}
			m_audioSkill.Trigger();
			EmitBlackholeBullet();
		}
	}
}
