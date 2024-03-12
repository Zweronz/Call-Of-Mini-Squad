namespace CoMDS2
{
	public class AIStateBorn : AIState
	{
		private float m_timer;

		public AIStateBorn(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			if (obj.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)obj).GetAnimationName("Appear");
			}
		}

		protected override void OnEnter()
		{
			m_timer = 0f;
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)m_activeObject).GetAnimationName("Appear");
			}
			m_activeObject.AnimationPlay(base.animName, false);
		}

		protected override void OnExit()
		{
			m_activeObject.AnimationStop(base.animName);
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_timer += deltaTime;
			if (m_timer >= m_activeObject.AnimationLength(base.animName))
			{
				m_activeObject.ChangeToDefaultAIState();
			}
		}
	}
}
