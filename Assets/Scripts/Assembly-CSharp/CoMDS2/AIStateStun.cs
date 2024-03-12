using UnityEngine;

namespace CoMDS2
{
	public class AIStateStun : AIState
	{
		private float m_stunTime;

		private float m_timer;

		private Character m_character;

		private CharacterController m_characterController;

		public AIStateStun(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_characterController = obj.GetGameObject().GetComponent<CharacterController>();
			m_character = (Character)obj;
		}

		public void SetStun(float time)
		{
			m_stunTime = time;
		}

		protected override void OnEnter()
		{
			m_timer = 0f;
			m_characterController = m_activeObject.GetGameObject().GetComponent<CharacterController>();
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				DS2ActiveObject activeObject = m_activeObject;
				string animLowerBody = (base.animName = ((Player)m_activeObject).GetAnimationName("SeriousInjury"));
				activeObject.animLowerBody = animLowerBody;
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
			else if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				DS2ActiveObject activeObject2 = m_activeObject;
				string animLowerBody = (base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle"));
				activeObject2.animLowerBody = animLowerBody;
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
			m_character.isStuck = true;
			m_character.SetMove(false, m_character.MoveDirection);
			if (m_character.GetPathFinding().HasNavigation())
			{
				m_character.GetPathFinding().StopNav();
			}
			m_character.effectPlayManager.PlayEffect("Stun");
		}

		protected override void OnExit()
		{
			m_character.isStuck = false;
			m_character.effectPlayManager.StopEffect("Stun");
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_timer += deltaTime;
			if (m_timer >= m_stunTime)
			{
				m_timer = 0f;
				m_activeObject.ChangeToDefaultAIState();
			}
		}
	}
}
