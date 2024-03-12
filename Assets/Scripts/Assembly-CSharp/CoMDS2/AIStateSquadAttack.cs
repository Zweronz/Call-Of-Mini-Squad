using UnityEngine;

namespace CoMDS2
{
	public class AIStateSquadAttack : AIState
	{
		private Player m_character;

		private CharacterController m_characterController;

		private SquadController m_squadController;

		private float m_time;

		private Vector3 m_targetPosition;

		private int m_squadSite;

		private float m_emitTime;

		private float m_emitTimer;

		private int m_animCount;

		private float m_originalSpeed;

		private float m_reduceSpeed;

		private string animHurt = string.Empty;

		public AIStateSquadAttack(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = m_character.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetSite(int squadSite)
		{
			m_squadSite = squadSite;
		}

		protected override void OnEnter()
		{
			m_squadController = GameBattle.m_instance.GetSquadController();
			stateToAttack();
			m_character.m_fire = true;
			m_originalSpeed = m_character.MoveSpeed;
			m_reduceSpeed = m_character.baseAttribute.moveSpeed * 0f;
			m_character.MoveSpeed -= m_reduceSpeed;
		}

		protected override void OnExit()
		{
			m_character.MoveSpeed += m_reduceSpeed;
			m_character.SetFire(false, m_character.m_fireDirection);
		}

		protected override void OnUpdate(float deltaTime)
		{
			updatePaseAttack(deltaTime);
		}

		private void stateToAttack()
		{
			if (m_character.m_weapon != null)
			{
				m_emitTime = m_character.m_weapon.emitTimeInAnimation;
				m_character.animUpperBody = m_character.GetAnimationNameByWeapon("Shoting");
				base.animName2 = m_character.animUpperBody;
				animHurt = m_character.GetAnimationNameByWeapon("Hurt");
				m_animCount = m_character.GetAnimationCountByName("Shoting");
			}
			if (m_character.m_weapon.m_bRunningFire)
			{
				m_character.AnimationPlay(base.animName2, true);
				m_emitTimer = m_emitTime;
			}
			else
			{
				m_emitTimer = -1f;
			}
		}

		private void updatePaseAttack(float deltaTime)
		{
			float distance = 0f;
			DS2ActiveObject dS2ActiveObject;
			if (DataCenter.State().isPVPMode)
			{
				dS2ActiveObject = m_character.lockedTarget;
				if (dS2ActiveObject == null)
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetListWithDistance(m_character, ref distance);
				}
			}
			else
			{
				dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetListWithDistance(m_character, ref distance);
			}
			float num = ((!m_squadController.m_fire) ? m_character.shootRange : m_squadController.TeammemberGuardDistance);
			if (m_squadController.m_fire)
			{
				m_character.FaceDirection = m_squadController.FaceDirection;
				m_character.GetTransform().forward = m_character.FaceDirection;
			}
			else if (distance != 0f && distance < num * num)
			{
				if (dS2ActiveObject == null || !dS2ActiveObject.Alive())
				{
					if (!m_squadController.m_fire)
					{
						return;
					}
				}
				else
				{
					Vector3 faceDirection = dS2ActiveObject.GetTransform().position - m_character.GetTransform().position;
					if (dS2ActiveObject.GetTransform().position.y - m_character.GetTransform().position.y < 0.2f)
					{
						m_character.FaceDirection = faceDirection;
					}
					m_character.GetTransform().forward = m_character.FaceDirection;
				}
			}
			else
			{
				DS2ActiveObject neareastEnemyInTargetArea = m_squadController.GetNeareastEnemyInTargetArea(m_character);
				if (neareastEnemyInTargetArea != null)
				{
					if (neareastEnemyInTargetArea.GetTransform().position.y - m_character.GetTransform().position.y < 0.2f)
					{
						m_character.FaceDirection = neareastEnemyInTargetArea.GetTransform().position - m_character.GetTransform().position;
					}
					m_character.GetTransform().forward = m_character.FaceDirection;
				}
				else
				{
					m_character.FaceDirection = m_squadController.FaceDirection;
					m_character.GetTransform().forward = m_character.FaceDirection;
					if (!m_squadController.m_fire)
					{
						return;
					}
				}
			}
			if (m_character.m_weapon == null || m_character.m_weapon.NeedReload())
			{
				return;
			}
			Player player = (Player)m_activeObject;
			if (m_character.m_weapon.m_bRunningFire)
			{
				if (!m_character.AnimationPlaying(base.animName2) && !m_character.AnimationPlaying(animHurt))
				{
					m_character.animUpperBody = base.animName2;
					m_character.AnimationPlay(base.animName2, true);
				}
			}
			else if (m_character.AnimationPlaying(animHurt))
			{
				m_emitTimer = -1f;
			}
			if (m_emitTimer != -1f && m_character.AnimationPlaying(base.animName2))
			{
				m_emitTimer += Time.deltaTime;
				if (m_emitTimer >= m_emitTime)
				{
					m_emitTimer = -1f;
					m_character.m_weapon.UpdateFire(deltaTime);
				}
			}
			if (!m_character.m_weapon.firePermit() || m_emitTimer != -1f)
			{
				return;
			}
			m_emitTimer = 0f;
			if (m_character.m_weapon.m_bRunningFire)
			{
				return;
			}
			if (m_animCount > 1)
			{
				m_character.animUpperBody = m_character.GetAnimationNameByWeapon("Shoting");
				base.animName2 = m_character.animUpperBody;
			}
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				if (player.characterType == Player.CharacterType.FireDragon || player.characterType == Player.CharacterType.Zero)
				{
					m_character.SetAnimationSpeed(base.animName2, m_character.AnimationLength(base.animName2) / m_character.m_weapon.attribute.fireFrequency);
					if (player.CurrentRandAnimIndex == 0)
					{
						player.m_weapon.EffectFireStart(0);
						player.m_weapon.SetEffectPlaySpeed(1, base.animSpeed);
					}
					else if (player.CurrentRandAnimIndex == 2)
					{
						player.m_weapon.EffectFireStart(1);
						player.m_weapon.SetEffectPlaySpeed(1, base.animSpeed);
					}
					m_character.AnimationCrossFade(base.animName2, false);
				}
				else if (player.CurrentController)
				{
					m_character.AnimationPlay(base.animName2, false, true);
				}
				else
				{
					m_character.AnimationCrossFade(base.animName2, false, true);
				}
			}
			else
			{
				m_character.AnimationCrossFade(base.animName2, false, true);
			}
		}
	}
}
