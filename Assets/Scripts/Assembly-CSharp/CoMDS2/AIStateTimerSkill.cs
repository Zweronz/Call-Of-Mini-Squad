namespace CoMDS2
{
	internal class AIStateTimerSkill : AIState
	{
		private Character m_character;

		public AIStateTimerSkill(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
		}

		protected override void OnExit()
		{
			base.OnExit();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
		}
	}
}
