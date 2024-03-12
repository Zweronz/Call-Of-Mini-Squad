using UnityEngine;

namespace CoMDS2
{
	public class AIStateAutoControl : AIState
	{
		private enum State
		{
			Idle = 0,
			Fire = 1
		}

		protected CharacterController m_characterController;

		private Character m_character;

		private State m_state;

		public AIStateAutoControl(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = character.GetGameObject().GetComponent<CharacterController>();
		}

		protected override void OnEnter()
		{
			Player player = (Player)m_character;
			if (m_character.m_weapon != null)
			{
				m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
			}
			base.animName = m_character.animLowerBody;
			m_activeObject.AnimationCrossFade(base.animName, true);
			m_state = State.Idle;
			m_character.SetFire(false, m_character.FaceDirection);
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			Player player = (Player)m_character;
			if (!m_character.m_move)
			{
				if (m_character.m_weapon != null)
				{
					m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
				}
				if (base.animName != m_character.animLowerBody)
				{
					base.animName = m_character.animLowerBody;
					m_activeObject.AnimationCrossFade(base.animName, true);
				}
			}
			else if (player.CurrentController)
			{
				base.animName = m_character.animLowerBody;
				m_characterController.Move(m_activeObject.GetMoveTransform().TransformDirection(m_character.MoveDirection) * deltaTime * m_character.MoveSpeed);
			}
			switch (m_state)
			{
			case State.Idle:
				updatePaseIdle();
				break;
			case State.Fire:
				updatePaseAttack();
				break;
			}
		}

		private void stateToIdle()
		{
			m_state = State.Idle;
			m_character.SetFire(false, m_character.FaceDirection);
			base.animName2 = m_character.animUpperBody;
			m_activeObject.AnimationStop(base.animName2);
		}

		private void updatePaseIdle()
		{
			DS2ActiveObject dS2ActiveObject;
			if (DataCenter.State().isPVPMode)
			{
				dS2ActiveObject = m_character.lockedTarget;
				if (dS2ActiveObject == null)
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
				}
			}
			else
			{
				dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
			}
			if (dS2ActiveObject != null && dS2ActiveObject.Alive())
			{
				float num = m_character.shootRange * m_character.shootRange;
				float num2 = m_character.meleeRange * m_character.meleeRange;
				float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude <= m_character.shootRange * m_character.shootRange - 10f)
				{
					stateToAttack();
				}
			}
		}

		private void stateToAttack()
		{
			m_character.SetFire(true, m_character.FaceDirection);
			Push(m_character.GetAIState("UpperFire"));
			m_state = State.Fire;
		}

		private void updatePaseAttack()
		{
			DS2ActiveObject dS2ActiveObject;
			if (DataCenter.State().isPVPMode)
			{
				dS2ActiveObject = m_character.lockedTarget;
				if (dS2ActiveObject == null)
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
				}
			}
			else
			{
				dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
			}
			if (dS2ActiveObject == null || !dS2ActiveObject.Alive())
			{
				Pop();
				m_character.ChangeAIState("FireReady");
				return;
			}
			float num = m_character.shootRange * m_character.shootRange;
			float num2 = m_character.meleeRange * m_character.meleeRange;
			float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
			if (sqrMagnitude > m_character.shootRange * m_character.shootRange)
			{
				Pop();
				m_character.ChangeAIState("FireReady");
			}
		}
	}
}
