using UnityEngine;

namespace CoMDS2
{
	public class PlayerCharacterClint : Player
	{
		private string m_characterName = "Clint";

		private bool m_twoGunState;

		private float m_twoGunTime;

		private float m_twoGunKeepTime = 6f;

		private int m_bulletCountCache;

		private ITAudioEvent m_audioSkill;

		private float m_checkSkillTimer;

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIState aIState = new AIState(this, "Skill");
			aIState.SetCustomFunc(OnSkillTwoGun);
			AIState aIState2 = new AIState(this, "SkillReady");
			aIState2.SetCustomFunc(OnSkillReady);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			base.skillInfo = DataCenter.Conf().GetHeroSkillInfo(base.characterType, base.playerData.skillLevel, base.playerData.skillStar);
			m_skillTotalCDTime = base.skillInfo.CDTime;
			DataConf.SkillClint skillClint = (DataConf.SkillClint)base.skillInfo;
			m_twoGunKeepTime = skillClint.keepTime;
			NumberSection<float> aTK = skillClint.GetATK();
			base.skillHitInfo.damage = aTK;
			m_audioSkill = GetTransform().Find("AudioSkill").GetComponentInChildren<ITAudioEvent>();
		}

		public void OnSkillTwoGun(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				m_audioSkill.Trigger();
				m_weapon.StopFire();
				AnimationStop(base.animUpperBody);
				base.animLowerBody = GetAnimationName("Skill");
				AnimationPlay(base.animLowerBody, false);
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = true;
				}
				m_bulletCountCache = m_weapon.m_iBulletCount;
				m_weapon.m_iBulletCount = -999;
				WeaponPistol weaponPistol2 = (WeaponPistol)m_weapon;
				weaponPistol2.GetRightGun().SetBulletEmitOnTime(2);
				weaponPistol2.GetLeftGun().SetActive(true);
				base.isRage = true;
				SetGodTime(float.PositiveInfinity);
				break;
			}
			case AIState.AIPhase.Update:
				if (!AnimationPlaying(base.animLowerBody))
				{
					base.name = m_characterName + "2";
					m_twoGunState = true;
					m_twoGunTime = 0f;
					WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
					weaponPistol.handType = WeaponPistol.HandType.Dual;
					SwitchFSM(GetAIState("FireReady"));
				}
				break;
			case AIState.AIPhase.Exit:
				if (!DataCenter.Save().squadMode && base.CurrentController)
				{
					GameBattle.s_bInputLocked = false;
				}
				base.isRage = false;
				SkillEnd();
				SetGodTime(0f);
				break;
			}
		}

		protected override void SetAnimationsMixing()
		{
			Transform mix = m_transform.Find("Bip01/Spine_00/Bip01 Spine");
			if (base.name == null || base.name == string.Empty)
			{
				return;
			}
			DataConf.AnimData newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Hurt");
			if (newCharacterAnim.count > 1)
			{
				for (int i = 0; i < newCharacterAnim.count; i++)
				{
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (i + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (i + 1)].AddMixingTransform(mix);
				}
			}
			else
			{
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			}
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Reload");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shift");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shoting");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "ReadyFire");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			base.name = m_characterName + "2";
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Hurt");
			if (newCharacterAnim.count > 1)
			{
				for (int j = 0; j < newCharacterAnim.count; j++)
				{
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (j + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[newCharacterAnim.name + "0" + (j + 1)].AddMixingTransform(mix);
				}
			}
			else
			{
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
				m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			}
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Reload");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shift");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "Shoting");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			newCharacterAnim = DataCenter.Conf().GetNewCharacterAnim(base.name, "ReadyFire");
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].layer = 2;
			m_gameObject.GetComponent<Animation>()[newCharacterAnim.name].AddMixingTransform(mix);
			base.name = m_characterName;
		}

		public override void UpdateAnimationSpeed()
		{
			float num = m_moveSpeed / 6f;
			string animationName = GetAnimationName("Lower_Body_Run_F");
			SetAnimationSpeed(animationName, num);
			string animationName2 = GetAnimationName("Lower_Body_Run_B");
			SetAnimationSpeed(animationName2, num);
			string animationName3 = GetAnimationName("Lower_Body_Run_L");
			SetAnimationSpeed(animationName3, num);
			string animationName4 = GetAnimationName("Lower_Body_Run_R");
			SetAnimationSpeed(animationName4, num);
			base.name = m_characterName + "2";
			animationName = GetAnimationName("Lower_Body_Run_F");
			SetAnimationSpeed(animationName, num);
			animationName2 = GetAnimationName("Lower_Body_Run_B");
			SetAnimationSpeed(animationName2, num);
			animationName3 = GetAnimationName("Lower_Body_Run_L");
			SetAnimationSpeed(animationName3, num);
			animationName4 = GetAnimationName("Lower_Body_Run_R");
			SetAnimationSpeed(animationName4, num);
			if (!m_twoGunState)
			{
				base.name = m_characterName;
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (m_twoGunState)
			{
				m_twoGunTime += deltaTime;
				if (m_twoGunTime >= 6f)
				{
					m_twoGunState = false;
					WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
					weaponPistol.GetRightGun().SetBulletEmitOnTime(1);
					weaponPistol.handType = WeaponPistol.HandType.Single;
					weaponPistol.GetLeftGun().SetActive(false);
					m_weapon.m_iBulletCount = m_bulletCountCache;
					base.name = m_characterName;
					SwitchFSM(GetDefaultAIState());
				}
			}
		}

		public override void UseWeapon(int index)
		{
			base.UseWeapon(index);
			WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
			weaponPistol.GetRightGun().SetBulletEmitOnTime(1);
			weaponPistol.handType = WeaponPistol.HandType.Single;
			weaponPistol.GetLeftGun().SetActive(false);
		}

		public override void OnDeath()
		{
			m_twoGunState = false;
			WeaponPistol weaponPistol = (WeaponPistol)m_weapon;
			weaponPistol.GetRightGun().SetBulletEmitOnTime(1);
			weaponPistol.handType = WeaponPistol.HandType.Single;
			weaponPistol.GetLeftGun().SetActive(false);
			base.name = m_characterName;
			m_weapon.m_iBulletCount = m_bulletCountCache;
			base.OnDeath();
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
