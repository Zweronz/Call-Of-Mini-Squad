using UnityEngine;

namespace CoMDS2
{
	public class AIStateFireReady : AIState
	{
		private Character m_character;

		public GameObject target { get; set; }

		public AIStateFireReady(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		protected override void OnEnter()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				m_character.animUpperBody = m_character.GetAnimationNameByWeapon("ReadyFire");
				base.animName2 = m_character.animUpperBody;
			}
			m_activeObject.AnimationCrossFade(base.animName2, false);
		}

		protected override void OnExit()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_character;
				if (player.CurrentController)
				{
					player.FaceToMoveDirection = true;
				}
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_character.m_move)
			{
				if (base.ChildState == null || base.ChildState.name != "LowerMove")
				{
					Push(m_character.GetAIState("LowerMove"));
				}
				if (!m_activeObject.AnimationPlaying(base.animName2))
				{
					Player player = (Player)m_character;
					if (player.AutoControl || player.HalfAutoControl || !player.CurrentController)
					{
						m_activeObject.ChangeToDefaultAIState();
					}
					else
					{
						m_activeObject.ChangeAIState("Move");
					}
				}
			}
			else if (m_character.m_fire)
			{
				if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
				{
					Player player2 = (Player)m_character;
					if (player2.AutoControl || player2.HalfAutoControl)
					{
						m_activeObject.ChangeToDefaultAIState();
					}
					else
					{
						m_activeObject.ChangeAIState("Shoot");
					}
				}
			}
			else if (!m_activeObject.AnimationPlaying(base.animName2) && !m_character.m_move)
			{
				m_activeObject.ChangeToDefaultAIState();
			}
		}
	}
}
