using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterXJohnX : Player
	{
		private EffectControl m_chestLaserEffect;

		private EffectControl m_chestLaserPointEffect;

		private EffectControl m_chestLaserAttackEffect;

		private Transform m_chestLaserPoint;

		private float m_laserAttackDeltaTime;

		private float m_laserMaxDistance = 20f;

		private float m_laserInterval = 0.2f;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill", AIState.Controller.Player);
			aIState.SetCustomFunc(OnSkillChestLaser);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(8f);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			SetAttackCollider(false);
			Transform transform = GetTransform();
			m_chestLaserEffect = transform.Find("High-speed particle flow").Find("High-speed particle flow_01").GetComponent<EffectControl>();
			m_chestLaserPointEffect = transform.Find("High-speed particle flow_ju").GetComponentInChildren<EffectControl>();
			m_chestLaserAttackEffect = transform.Find("High-speed particle flow_Attack").GetComponentInChildren<EffectControl>();
			m_chestLaserPoint = transform.Find("ChestLaserPoint");
			m_chestLaserEffect.gameObject.SetActive(false);
			m_chestLaserAttackEffect.gameObject.SetActive(false);
			m_chestLaserPointEffect.gameObject.SetActive(false);
			DataConf.SkillXJohnX skillXJohnX = (DataConf.SkillXJohnX)(base.skillInfo = (DataConf.SkillXJohnX)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillXJohnX.GetATK();
			base.skillHitInfo.damage = aTK;
			base.skillHitInfo.repelDistance = new NumberSection<float>(0f, 0f);
			base.skillHitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
			m_laserMaxDistance = skillXJohnX.range;
			m_laserInterval = skillXJohnX.laserInterval;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (m_chestLaserEffect.gameObject.activeSelf)
			{
				m_laserAttackDeltaTime += deltaTime;
				if (m_laserAttackDeltaTime >= m_laserInterval)
				{
					m_laserAttackDeltaTime = 0f;
					SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
					SetAttackCollider(true, AttackCollider.AttackColliderType.Dash);
				}
			}
		}

		public void OnSkillChestLaser(AIState.AIPhase phase)
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
				m_weapon.SetActive(false);
				break;
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					m_weapon.SetActive(true);
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

		private void ChestLaserAttack()
		{
			float num = m_laserMaxDistance;
			RaycastHit raycastHit;
			if (Physics.Raycast(m_chestLaserPoint.position, m_chestLaserPoint.forward, out raycastHit, num, 327680))
			{
				num = Vector3.Distance(m_chestLaserPoint.position, raycastHit.point);
				m_chestLaserAttackEffect.transform.position = raycastHit.point;
			}
			else
			{
				m_chestLaserAttackEffect.transform.position = m_chestLaserPoint.position + m_chestLaserPoint.forward * 20f;
			}
			m_chestLaserEffect.transform.parent.localScale = new Vector3(1f, 1f, num / 20f);
			m_chestLaserEffect.gameObject.SetActive(true);
			m_chestLaserAttackEffect.gameObject.SetActive(true);
			m_chestLaserPointEffect.gameObject.SetActive(true);
			SetAttackCollider(true, AttackCollider.AttackColliderType.Dash);
			m_laserAttackDeltaTime = 0f;
			m_chestLaserEffect.StartEmit();
			m_chestLaserAttackEffect.StartEmit();
			m_chestLaserPointEffect.StartEmit();
		}

		private void ChestLaserOver()
		{
			m_chestLaserEffect.StopEmit();
			m_chestLaserAttackEffect.StopEmit();
			m_chestLaserPointEffect.StopEmit();
			m_chestLaserEffect.gameObject.SetActive(false);
			m_chestLaserAttackEffect.gameObject.SetActive(false);
			m_chestLaserPointEffect.gameObject.SetActive(false);
			SetAttackCollider(false, AttackCollider.AttackColliderType.Dash);
		}
	}
}
