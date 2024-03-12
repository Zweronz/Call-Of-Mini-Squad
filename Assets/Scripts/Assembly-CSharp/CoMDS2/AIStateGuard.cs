using UnityEngine;

namespace CoMDS2
{
	public class AIStateGuard : AIState
	{
		private enum GuardState
		{
			Idle = 0,
			TurnRound = 1
		}

		private GuardState m_guardState;

		private CharacterController m_characterController;

		private float m_range;

		private float m_time;

		private float m_changeStateTime;

		private int m_turnRoundAnimId = 1;

		private new string animName2;

		private Character m_character;

		public AIStateGuard(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle");
			}
		}

		public void SetGuard(float range)
		{
			m_range = range;
		}

		protected override void OnEnter()
		{
			m_guardState = GuardState.Idle;
			m_changeStateTime = 5f;
			if (!m_activeObject.isBuilding)
			{
				if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
				{
					DS2ActiveObject activeObject = m_activeObject;
					string animLowerBody = (base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle"));
					activeObject.animLowerBody = animLowerBody;
				}
				m_activeObject.AnimationCrossFade(base.animName, true);
				m_activeObject.AnimationStop(m_activeObject.animUpperBody);
			}
		}

		protected override void OnExit()
		{
			if (!m_activeObject.isBuilding)
			{
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			switch (m_guardState)
			{
			case GuardState.Idle:
				updateIdle(deltaTime);
				break;
			case GuardState.TurnRound:
				updateTurnRound();
				break;
			}
			if (base.controller != Controller.System)
			{
				return;
			}
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
			if (dS2ActiveObject.Alive())
			{
				float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude <= m_range * m_range && !m_activeObject.ChangeAIState("Chase", false))
				{
					m_activeObject.ChangeAIState("Shoot");
				}
			}
			else
			{
				m_activeObject.ChangeToDefaultAIState();
			}
		}

		private void stateToTurnRound()
		{
			m_guardState = GuardState.TurnRound;
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				animName2 = ((Enemy)m_activeObject).GetAnimationName("Guard");
			}
			m_activeObject.AnimationCrossFade(animName2, false);
		}

		private void updateTurnRound()
		{
			if (!m_activeObject.AnimationPlaying(animName2))
			{
				stateToIdle();
			}
		}

		private void stateToIdle()
		{
			m_guardState = GuardState.Idle;
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				m_activeObject.AnimationStop(animName2);
				m_activeObject.AnimationCrossFade(base.animName, true);
				m_changeStateTime = Random.Range(4, 6);
			}
		}

		private void updateIdle(float deltaTime)
		{
			m_time += deltaTime;
			if (m_time >= m_changeStateTime)
			{
				m_time = 0f;
				stateToTurnRound();
			}
		}
	}
}
