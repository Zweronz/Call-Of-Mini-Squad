using UnityEngine;

namespace CoMDS2
{
	internal class AIStateLowerMove : AIState
	{
		protected CharacterController m_characterController;

		private Character m_character;

		public Vector3 direction { get; set; }

		public float speed { get; set; }

		public AIStateLowerMove(Character character, string name, Controller controller = Controller.System)
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
			base.animName = string.Empty;
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
				m_character.GetModelTransform().forward = m_character.FaceDirection;
				m_characterController.Move(m_activeObject.GetMoveTransform().TransformDirection(m_character.MoveDirection) * deltaTime * m_character.MoveSpeed);
			}
		}
	}
}
