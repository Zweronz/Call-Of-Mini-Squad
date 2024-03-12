using UnityEngine;

namespace CoMDS2
{
	internal class AIStateMove : AIState
	{
		protected CharacterController m_characterController;

		private Character m_character;

		public Vector3 direction { get; set; }

		public float speed { get; set; }

		public AIStateMove(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = character.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetMove(Vector3 direction, float speed)
		{
			this.direction = direction;
			this.speed = m_character.MoveSpeed;
		}

		protected override void OnEnter()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				base.animName = m_character.animLowerBody;
			}
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_character.objectType != 0 && m_character.objectType != Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				return;
			}
			Player player = (Player)m_character;
			base.animName = m_character.animLowerBody;
			if (m_character.m_fire)
			{
				m_activeObject.ChangeAIState("MoveShoot");
			}
			if (!m_character.m_move)
			{
				m_character.ChangeToDefaultAIState();
			}
			else if (player.CurrentController)
			{
				if (player.m_objAutoShootAreaTarget == null)
				{
					m_character.GetModelTransform().forward = m_character.FaceDirection;
				}
				m_characterController.Move(m_activeObject.GetMoveTransform().TransformDirection(m_character.MoveDirection) * deltaTime * m_character.MoveSpeed);
			}
		}
	}
}
