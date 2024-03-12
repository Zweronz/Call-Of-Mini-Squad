using UnityEngine;

namespace CoMDS2
{
	public class AIStateFire : AIState
	{
		private float m_range = 1f;

		public AIStateFire(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
		}

		public void SetFire(float range)
		{
			m_range = range;
		}

		protected override void OnEnter()
		{
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (base.controller == Controller.Player)
			{
				return;
			}
			DS2ActiveObject nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
			if (nearestObjFromTargetList != null && nearestObjFromTargetList.Alive())
			{
				m_activeObject.LookAt(nearestObjFromTargetList.GetTransform());
				float sqrMagnitude = (nearestObjFromTargetList.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
				float num = m_range * m_range;
				if (sqrMagnitude > num)
				{
					m_activeObject.ChangeToDefaultAIState();
				}
				else
				{
					m_activeObject.UpdateFire(Time.deltaTime);
				}
			}
			else
			{
				m_activeObject.ChangeToDefaultAIState();
			}
		}
	}
}
