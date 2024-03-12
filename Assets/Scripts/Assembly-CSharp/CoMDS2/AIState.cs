namespace CoMDS2
{
	public class AIState
	{
		public enum Controller
		{
			Player = 0,
			System = 1
		}

		public enum AIPhase
		{
			Enter = 0,
			Update = 1,
			Exit = 2
		}

		public delegate void CustomFunc(AIPhase phase);

		protected CustomFunc m_customFunc;

		private AIState m_nextState;

		private bool m_continueUpdate = true;

		private AIState m_childState;

		private AIState m_root;

		public bool active = true;

		protected DS2ActiveObject m_activeObject;

		public string name { get; set; }

		public string animName { get; set; }

		public string animName2 { get; set; }

		public float animSpeed { get; set; }

		public Controller controller { get; set; }

		public AIState ChildState
		{
			get
			{
				return m_childState;
			}
		}

		public AIState(DS2ActiveObject obj, string name, Controller controller = Controller.System)
		{
			m_activeObject = obj;
			this.controller = controller;
			this.name = name;
			animSpeed = 1f;
		}

		public void Enter()
		{
			OnEnter();
		}

		public void Exit()
		{
			if (m_childState != null)
			{
				m_childState.Exit();
				m_childState.m_root = null;
				m_childState = null;
			}
			OnExit();
		}

		public void Update(float deltaTime)
		{
			m_nextState = this;
			m_continueUpdate = true;
			if (m_childState != null)
			{
				m_childState.Update(deltaTime);
				if (m_childState != null)
				{
					AIState aIState = m_childState.NextState();
					m_continueUpdate = m_childState.ContinueUpdate();
					if (aIState != m_childState)
					{
						m_childState.Exit();
						m_childState = aIState;
						if (m_childState != null)
						{
							m_childState.Enter();
						}
					}
				}
			}
			if (m_continueUpdate)
			{
				OnUpdate(deltaTime);
			}
		}

		public bool ContinueUpdate()
		{
			return m_continueUpdate;
		}

		public AIState NextState()
		{
			return m_nextState;
		}

		public void ContinueUpdate(bool continueUpdate)
		{
			m_continueUpdate = continueUpdate;
		}

		public void NextState(AIState nextState)
		{
			m_nextState = nextState;
		}

		public void Push(AIState state)
		{
			if (m_childState != null)
			{
				m_childState.Exit();
				m_childState.m_root = null;
				m_childState = null;
			}
			m_childState = state;
			m_childState.m_root = this;
			if (m_childState != null)
			{
				m_childState.Enter();
			}
		}

		public void Pop()
		{
			if (m_childState != null)
			{
				m_childState.Exit();
				m_childState.m_root = null;
				m_childState = null;
			}
		}

		public void Switch(AIState state)
		{
			if (state != null)
			{
				if (m_root != null)
				{
					m_root = null;
					NextState(null);
				}
				else
				{
					NextState(state);
				}
			}
		}

		public void SetCustomFunc(CustomFunc custom)
		{
			m_customFunc = custom;
		}

		protected virtual void OnEnter()
		{
			if (m_customFunc != null)
			{
				m_customFunc(AIPhase.Enter);
			}
		}

		protected virtual void OnExit()
		{
			if (m_customFunc != null)
			{
				m_customFunc(AIPhase.Exit);
			}
		}

		protected virtual void OnUpdate(float deltaTime)
		{
			if (m_customFunc != null)
			{
				m_customFunc(AIPhase.Update);
			}
		}
	}
}
