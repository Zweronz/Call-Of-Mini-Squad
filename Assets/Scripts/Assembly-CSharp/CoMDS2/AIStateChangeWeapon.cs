namespace CoMDS2
{
	public class AIStateChangeWeapon : AIState
	{
		private Character m_character;

		public AIStateChangeWeapon(Character obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_character = obj;
		}

		protected override void OnEnter()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				Player player = (Player)m_character;
				if (player.CurrentController)
				{
					Push(m_character.GetAIState("LowerMove"));
				}
				else if (m_character.m_move)
				{
					Push(m_character.GetAIState("AllyFollow"));
					base.animName = m_character.animLowerBody;
				}
				DS2ActiveObject activeObject = m_activeObject;
				string animUpperBody = (base.animName2 = m_character.GetAnimationNameByWeapon("Shift"));
				activeObject.animUpperBody = animUpperBody;
			}
			m_character.AnimationCrossFade(base.animName2, false, false, 0.1f);
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if ((m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY) && !m_character.AnimationPlaying(base.animName2))
			{
				m_character.ChangeToLastAIState();
				m_character.DoChangeWeapon();
			}
		}
	}
}
