using UnityEngine;

namespace CoMDS2
{
	public class AIStateChase : AIState
	{
		private Character m_character;

		private CharacterController m_characterController;

		private float m_distance;

		private float m_speed;

		public string animStartMoveName { get; set; }

		public string animStopMoveName { get; set; }

		public DS2ActiveObject target { get; set; }

		public AIStateChase(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_character = (Character)obj;
			m_characterController = m_character.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetChase(float distance, float speed)
		{
			m_distance = distance;
			m_speed = m_character.MoveSpeed;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			if (!string.IsNullOrEmpty(animStartMoveName))
			{
				m_activeObject.AnimationCrossFade(animStartMoveName, false);
			}
			else
			{
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
		}

		protected override void OnExit()
		{
			base.OnExit();
		}

		protected override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (!string.IsNullOrEmpty(animStartMoveName) && !m_activeObject.AnimationPlaying(animStartMoveName) && !m_activeObject.AnimationPlaying(base.animName))
			{
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
			if (DataCenter.State().isPVPMode)
			{
				target = m_character.lockedTarget;
				if (target == null)
				{
					target = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
				}
			}
			else
			{
				target = GameBattle.m_instance.GetNearestObjFromTargetList(m_character);
				m_character.lockedTarget = target;
			}
			if (target != null && target.Alive())
			{
				bool flag = false;
				if (m_character != null)
				{
					float num = m_character.shootRange * m_character.shootRange;
					float num2 = m_character.meleeRange * m_character.meleeRange;
					float sqrMagnitude = (target.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
					if (sqrMagnitude <= num2 && m_character.meleeAble)
					{
						Ray ray = new Ray(m_activeObject.GetTransform().position, target.GetTransform().position - m_activeObject.GetTransform().position);
						if (!string.IsNullOrEmpty(animStopMoveName))
						{
							m_activeObject.AnimationCrossFade(animStopMoveName, false);
						}
						IPathFinding pathFinding = m_character.GetPathFinding();
						if (pathFinding != null && pathFinding.HasNavigation())
						{
							pathFinding.StopNav();
						}
						m_activeObject.ChangeAIState("Melee");
					}
					else if (sqrMagnitude <= num && m_character.shootAble)
					{
						Ray ray2 = new Ray(m_activeObject.GetTransform().position, target.GetTransform().position - m_activeObject.GetTransform().position);
						flag = m_activeObject.ChangeAIState("Shoot", false);
						if (flag)
						{
							if (!string.IsNullOrEmpty(animStopMoveName))
							{
								m_activeObject.AnimationCrossFade(animStopMoveName, false);
							}
							IPathFinding pathFinding2 = m_character.GetPathFinding();
							if (pathFinding2 != null && pathFinding2.HasNavigation())
							{
								pathFinding2.StopNav(false);
							}
						}
					}
					else if (sqrMagnitude > m_distance * m_distance)
					{
						IPathFinding pathFinding3 = m_character.GetPathFinding();
						if (pathFinding3 != null && pathFinding3.HasNavigation())
						{
							pathFinding3.StopNav();
						}
						if (!string.IsNullOrEmpty(animStopMoveName))
						{
							m_activeObject.AnimationCrossFade(animStopMoveName, false);
						}
						flag = m_activeObject.ChangeAIState("Idle", false);
					}
				}
				if (!flag)
				{
					IPathFinding pathFinding4 = m_character.GetPathFinding();
					if (pathFinding4 != null && pathFinding4.HasNavigation())
					{
						pathFinding4.SetNavDesination(target.GetTransform().position);
					}
					else if (m_characterController != null)
					{
						m_activeObject.LookAt(target.GetTransform());
						m_characterController.Move(m_activeObject.GetModelTransform().forward * deltaTime * m_speed);
					}
					else
					{
						m_activeObject.LookAt(target.GetTransform());
						m_activeObject.GetTransform().Translate(m_activeObject.GetModelTransform().forward * deltaTime * m_speed);
					}
				}
				return;
			}
			if (m_character != null)
			{
				IPathFinding pathFinding5 = m_character.GetPathFinding();
				if (pathFinding5 != null && pathFinding5.HasNavigation())
				{
					pathFinding5.StopNav();
				}
			}
			if (!string.IsNullOrEmpty(animStopMoveName))
			{
				m_activeObject.AnimationCrossFade(animStopMoveName, false);
			}
			m_activeObject.ChangeToDefaultAIState();
		}
	}
}
