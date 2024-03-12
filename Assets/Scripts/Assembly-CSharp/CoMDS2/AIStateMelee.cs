namespace CoMDS2
{
	internal class AIStateMelee : AIState
	{
		private Character m_character;

		private int m_animAttackId;

		public AIStateMelee(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			m_character.isRage = true;
		}

		protected override void OnExit()
		{
			base.OnExit();
			m_character.SetAttackCollider(false);
			m_character.isRage = false;
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (m_character.lockedTarget == null || !m_character.lockedTarget.Alive())
			{
				m_character.ChangeToDefaultAIState();
			}
		}
	}
}
