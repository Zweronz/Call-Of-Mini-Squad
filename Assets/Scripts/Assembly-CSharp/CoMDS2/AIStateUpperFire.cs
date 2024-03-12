using UnityEngine;

namespace CoMDS2
{
	public class AIStateUpperFire : AIState
	{
		private Character m_character;

		private float m_changeTime;

		private float m_emitTime;

		private float m_emitTimer;

		private float m_originalSpeed;

		private float m_reduceSpeed;

		private string animHurt = string.Empty;

		public AIStateUpperFire(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		protected override void OnEnter()
		{
			m_originalSpeed = m_character.MoveSpeed;
			m_reduceSpeed = m_character.baseAttribute.moveSpeed * 0f;
			m_character.MoveSpeed -= m_reduceSpeed;
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_activeObject;
				if (player.m_weapon != null)
				{
					m_emitTime = player.m_weapon.emitTimeInAnimation;
					player.animUpperBody = m_character.GetAnimationNameByWeapon("Shoting");
				}
				base.animName2 = player.animUpperBody;
			}
			m_character.AnimationCrossFade(base.animName, true);
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

		protected override void OnExit()
		{
			m_character.MoveSpeed += m_reduceSpeed;
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.SetNavSpeed(m_character.MoveSpeed);
			}
			if (m_character.m_weapon != null)
			{
				m_character.m_weapon.StopFire();
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (base.controller == Controller.Player)
			{
				Player player = (Player)m_character;
				DS2ActiveObject dS2ActiveObject;
				if (DataCenter.State().isPVPMode)
				{
					dS2ActiveObject = m_character.lockedTarget;
					if (dS2ActiveObject == null)
					{
						dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
					}
				}
				else
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
				}
				if (dS2ActiveObject != null && dS2ActiveObject.Alive() && dS2ActiveObject.GetTransform().position.y - m_character.GetTransform().position.y < 0.2f)
				{
					m_character.LookAt(dS2ActiveObject.GetTransform());
					m_character.FaceDirection = m_character.GetModelTransform().forward;
				}
			}
			if (m_character.m_weapon == null || m_character.m_weapon.NeedReload())
			{
				return;
			}
			if (m_character.m_weapon.m_bRunningFire)
			{
				if (!m_character.AnimationPlaying(base.animName2) && !m_character.AnimationPlaying(animHurt))
				{
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
			if (m_character.m_weapon.firePermit() && m_emitTimer == -1f)
			{
				m_emitTimer = 0f;
				if (!m_character.m_weapon.m_bRunningFire)
				{
					m_character.AnimationPlay(base.animName2, false, true);
				}
			}
		}
	}
}
