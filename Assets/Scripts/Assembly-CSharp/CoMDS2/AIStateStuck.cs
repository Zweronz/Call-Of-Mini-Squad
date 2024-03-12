using UnityEngine;

namespace CoMDS2
{
	public class AIStateStuck : AIState
	{
		private CharacterController m_characterController;

		private Character m_character;

		public AIStateStuck(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_characterController = obj.GetGameObject().GetComponent<CharacterController>();
			m_character = (Character)obj;
		}

		protected override void OnEnter()
		{
			m_characterController = m_activeObject.GetGameObject().GetComponent<CharacterController>();
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_activeObject;
				DS2ActiveObject activeObject = m_activeObject;
				string animLowerBody = (base.animName = ((Player)m_activeObject).GetAnimationName("Idle"));
				activeObject.animLowerBody = animLowerBody;
				m_activeObject.AnimationPlay(base.animName, false);
			}
			else if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				DS2ActiveObject activeObject2 = m_activeObject;
				string animLowerBody = (base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle"));
				activeObject2.animLowerBody = animLowerBody;
				m_activeObject.AnimationPlay(base.animName, false);
			}
			m_character.isStuck = true;
			m_character.SetMove(false, m_character.MoveDirection);
			if (m_character.GetPathFinding().HasNavigation())
			{
				m_character.GetPathFinding().StopNav();
			}
		}

		protected override void OnExit()
		{
			m_character.isStuck = false;
		}

		protected override void OnUpdate(float deltaTime)
		{
		}
	}
}
