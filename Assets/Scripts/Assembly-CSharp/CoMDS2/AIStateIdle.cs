namespace CoMDS2
{
	public class AIStateIdle : AIState
	{
		public AIStateIdle(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
		}

		protected override void OnEnter()
		{
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Character character = (Character)m_activeObject;
				if (character.animLowerBody != string.Empty)
				{
					if (character.m_weapon != null)
					{
						character.animLowerBody = character.GetAnimationNameByWeapon("Idle");
					}
					base.animName = character.animLowerBody;
				}
				IPathFinding pathFinding = character.GetPathFinding();
				if (pathFinding != null && pathFinding.HasNavigation())
				{
					pathFinding.StopNav();
				}
			}
			else if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				Character character2 = (Character)m_activeObject;
				base.animName = character2.GetAnimationName("Idle");
				IPathFinding pathFinding2 = character2.GetPathFinding();
				if (pathFinding2 != null && pathFinding2.HasNavigation())
				{
					pathFinding2.StopNav();
				}
			}
			base.animName2 = base.animName;
			m_activeObject.AnimationCrossFade(base.animName, true);
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (base.controller != 0)
			{
				return;
			}
			Player player = (Player)m_activeObject;
			if (!player.AutoControl && !player.HalfAutoControl)
			{
				if (player.m_fire)
				{
					m_activeObject.ChangeAIState("Shoot");
				}
				if (player.m_move)
				{
					m_activeObject.ChangeAIState("Move");
				}
			}
		}
	}
}
