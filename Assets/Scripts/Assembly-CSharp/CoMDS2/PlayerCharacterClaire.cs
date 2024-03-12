using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterClaire : Player
	{
		private bool m_fireBtnDown;

		private DS2ObjectBuffer m_missileBuffer;

		private int m_missileCount = 5;

		private float m_missileAngle = 20f;

		private ITAudioEvent m_audioSkill;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillVolley);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AIStateSkillFindTarget aIStateSkillFindTarget = new AIStateSkillFindTarget(this, "SkillFindTarget");
			aIStateSkillFindTarget.SetStoppingDistance(8f);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIStateSkillFindTarget.name, aIStateSkillFindTarget);
			m_skillNeedFindTarget = true;
			base.hitInfo.deadRepelDistance = new NumberSection<float>(5f, 7f);
			DataConf.SkillClaire skillClaire = (DataConf.SkillClaire)(base.skillInfo = (DataConf.SkillClaire)DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar));
			m_missileCount = skillClaire.missileCount;
			m_missileAngle = skillClaire.missileAngle;
			m_skillTotalCDTime = base.skillInfo.CDTime;
			NumberSection<float> aTK = skillClaire.GetATK();
			base.skillHitInfo.damage = aTK;
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		private void SetMissile()
		{
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(35);
			m_missileBuffer = new DS2ObjectBuffer(m_missileCount);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			for (int i = 0; i < m_missileCount; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 21);
				GameObject gameObject2 = bullet.GetGameObject();
				if (gameObject2.GetComponent<LinearMoveToDestroy>() == null)
				{
					gameObject2.AddComponent<LinearMoveToDestroy>();
				}
				if (gameObject2.GetComponent<BulletTriggerScript>() == null)
				{
					gameObject2.AddComponent<BulletTriggerScript>();
				}
				bullet.hitInfo.damage = new NumberSection<float>(base.skillHitInfo.damage.left, base.skillHitInfo.damage.right);
				bullet.attribute = new Bullet.BulletAttribute();
				bullet.attribute.speed = m_weapon.attribute.bulletSpeed;
				bullet.attribute.effectHit = (Defined.EFFECT_TYPE)bulletDataByIndex.hitType;
				bullet.attribute.damageType = Bullet.BULLET_DAMAGE_TYPE.MULTI;
				bullet.attribute.damageRange = 2.5f;
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_missileBuffer.AddObj(bullet);
			}
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			SetMissile();
		}

		private void BigBangAttack()
		{
			m_weapon.EffectFireStart(false);
			m_weapon.EffectCartridgeEmit();
			m_weapon.PlayEffectLight();
			m_audioSkill.Trigger();
			for (int i = 0; i < m_missileCount; i++)
			{
				Bullet bullet = (Bullet)m_missileBuffer.GetObject();
				if (bullet != null)
				{
					bullet.hitInfo.source = this;
					Quaternion rotation = Quaternion.AngleAxis(m_weapon.m_firePoint.eulerAngles.y - (float)(m_missileCount - 1) * m_missileAngle * 0.5f + m_missileAngle * (float)i, Vector3.up);
					bullet.SetBullet(this, null, m_weapon.m_firePoint.position, rotation);
					bullet.Emit(m_weapon.attribute.attackRange);
				}
			}
		}

		public void OnSkillVolley(AIState.AIPhase phase)
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
	}
}
