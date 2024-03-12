using UnityEngine;

namespace CoMDS2
{
	public class Tutorial : MonoBehaviour
	{
		public enum TutorialPhaseState
		{
			None = 0,
			InProgress = 1,
			Done = 2
		}

		public enum TutorialPhase
		{

		}

		private static Tutorial m_instance;

		private TutorialPhaseState m_tutorialPahseMove;

		private TutorialPhaseState m_tutorialPahseFire;

		private TutorialPhaseState m_tutorialPahseSkill;

		private TutorialPhaseState m_tutorialPahseChangeMode;

		private TutorialPhaseState m_tutorialPahseChangePlayer;

		public UIPanel covePanel;

		public UIPanel highLightPanel;

		public UIPanel indicatePanel;

		public UIPanel movePanel;

		public UIPanel firePanel;

		public UIPanel skillPanel;

		public UIPanel changeModePanel;

		public UIPanel minimapPanel;

		public UIPanel changePlayerPanel;

		private GameObject moveExplain;

		private GameObject fireExplain;

		private GameObject skillExplain;

		private GameObject changeModeExplain;

		private GameObject minimapExplain;

		private GameObject changePlayerExplain;

		private bool m_tutorialInProgress;

		private float m_timer;

		public static Tutorial Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new Tutorial();
				}
				return m_instance;
			}
		}

		public TutorialPhaseState TutorialPahseMove
		{
			get
			{
				if (DataCenter.Save().BattleTutorialFinished)
				{
					return TutorialPhaseState.Done;
				}
				return m_tutorialPahseMove;
			}
			set
			{
				m_tutorialPahseMove = value;
				if (m_tutorialPahseMove == TutorialPhaseState.InProgress)
				{
					indicatePanel.transform.position = movePanel.transform.position;
					if (GameBattle.m_instance.GameState == GameBattle.State.Game)
					{
						indicatePanel.gameObject.SetActive(true);
					}
					movePanel.depth = covePanel.depth + 1;
					base.gameObject.SetActive(true);
					moveExplain.SetActive(true);
					TutorialInProgress = true;
				}
				else if (m_tutorialPahseMove == TutorialPhaseState.Done)
				{
					movePanel.depth = covePanel.depth - 1;
					moveExplain.SetActive(false);
					base.gameObject.SetActive(false);
					TutorialInProgress = false;
					indicatePanel.gameObject.SetActive(false);
					GameBattle.m_instance.IsPause = false;
				}
			}
		}

		public TutorialPhaseState TutorialPahseFire
		{
			get
			{
				if (DataCenter.Save().BattleTutorialFinished)
				{
					return TutorialPhaseState.Done;
				}
				return m_tutorialPahseFire;
			}
			set
			{
				m_tutorialPahseFire = value;
				if (m_tutorialPahseFire == TutorialPhaseState.InProgress)
				{
					indicatePanel.transform.position = firePanel.transform.position;
					indicatePanel.gameObject.SetActive(true);
					firePanel.depth = covePanel.depth + 1;
					base.gameObject.SetActive(true);
					fireExplain.SetActive(true);
					TutorialInProgress = true;
					TUIButtonJoystickEx component = movePanel.GetComponent<TUIButtonJoystickEx>();
					TUIInput input = default(TUIInput);
					input.inputType = TUIInputType.Ended;
					component.HandleInput(input);
					GameBattle.m_instance.IsPause = true;
				}
				else if (m_tutorialPahseFire == TutorialPhaseState.Done)
				{
					firePanel.depth = covePanel.depth - 1;
					fireExplain.SetActive(false);
					TutorialInProgress = false;
					indicatePanel.gameObject.SetActive(false);
					covePanel.gameObject.SetActive(false);
					m_timer = 5f;
					GameBattle.m_instance.IsPause = false;
				}
			}
		}

		public TutorialPhaseState TutorialPahseSkill
		{
			get
			{
				if (DataCenter.Save().BattleTutorialFinished)
				{
					return TutorialPhaseState.Done;
				}
				return m_tutorialPahseSkill;
			}
			set
			{
				m_tutorialPahseSkill = value;
				if (m_tutorialPahseSkill == TutorialPhaseState.InProgress)
				{
					indicatePanel.transform.position = skillPanel.transform.position;
					indicatePanel.gameObject.SetActive(true);
					skillPanel.depth = covePanel.depth + 1;
					base.gameObject.SetActive(true);
					skillExplain.SetActive(true);
					TutorialInProgress = true;
					covePanel.gameObject.SetActive(true);
					TUIButtonJoystickEx component = movePanel.GetComponent<TUIButtonJoystickEx>();
					TUIInput input = default(TUIInput);
					input.inputType = TUIInputType.Ended;
					component.HandleInput(input);
					TUIButtonJoystick component2 = firePanel.GetComponent<TUIButtonJoystick>();
					component2.HandleInput(input);
					GameBattle.m_instance.IsPause = true;
				}
				else if (m_tutorialPahseSkill == TutorialPhaseState.Done)
				{
					skillPanel.depth = covePanel.depth - 1;
					skillExplain.SetActive(false);
					base.gameObject.SetActive(false);
					TutorialInProgress = false;
					indicatePanel.gameObject.SetActive(false);
					GameBattle.m_instance.IsPause = false;
					HttpRequestHandle.instance.SendRequest(HttpRequestHandle.RequestType.Lesson, null);
				}
			}
		}

		public TutorialPhaseState TutorialPahseChangeMode
		{
			get
			{
				if (DataCenter.Save().tutorialChangeMode)
				{
					return TutorialPhaseState.Done;
				}
				return m_tutorialPahseChangeMode;
			}
			set
			{
				m_tutorialPahseChangeMode = value;
				if (m_tutorialPahseChangeMode == TutorialPhaseState.InProgress)
				{
					indicatePanel.transform.position = changeModePanel.transform.position;
					indicatePanel.gameObject.SetActive(true);
					changeModePanel.depth = covePanel.depth + 1;
					base.gameObject.SetActive(true);
					changeModeExplain.SetActive(true);
					TutorialInProgress = true;
					TUIButtonJoystickEx component = movePanel.GetComponent<TUIButtonJoystickEx>();
					TUIInput input = default(TUIInput);
					input.inputType = TUIInputType.Ended;
					component.HandleInput(input);
					TUIButtonJoystick component2 = firePanel.GetComponent<TUIButtonJoystick>();
					component2.HandleInput(input);
					GameBattle.m_instance.m_UIChangeMode.gameObject.SetActive(true);
					GameBattle.m_instance.IsPause = true;
				}
				else if (m_tutorialPahseChangeMode == TutorialPhaseState.Done)
				{
					changeModePanel.depth = covePanel.depth - 1;
					changeModeExplain.SetActive(false);
					TutorialInProgress = false;
					indicatePanel.gameObject.SetActive(false);
					covePanel.gameObject.SetActive(false);
					m_timer = 5f;
					GameBattle.m_instance.IsPause = false;
				}
			}
		}

		public TutorialPhaseState TutorialPahseChangePlayer
		{
			get
			{
				if (DataCenter.Save().tutorialChangeMode)
				{
					return TutorialPhaseState.Done;
				}
				return m_tutorialPahseChangePlayer;
			}
			set
			{
				m_tutorialPahseChangePlayer = value;
				if (m_tutorialPahseChangePlayer == TutorialPhaseState.InProgress)
				{
					indicatePanel.transform.position = changePlayerPanel.transform.position;
					indicatePanel.gameObject.SetActive(true);
					changePlayerPanel.depth = covePanel.depth + 1;
					base.gameObject.SetActive(true);
					changePlayerExplain.SetActive(true);
					TutorialInProgress = true;
					covePanel.gameObject.SetActive(true);
					TUIButtonJoystickEx component = movePanel.GetComponent<TUIButtonJoystickEx>();
					TUIInput input = default(TUIInput);
					input.inputType = TUIInputType.Ended;
					component.HandleInput(input);
					TUIButtonJoystick component2 = firePanel.GetComponent<TUIButtonJoystick>();
					component2.HandleInput(input);
					GameBattle.m_instance.IsPause = true;
				}
				else if (m_tutorialPahseChangePlayer == TutorialPhaseState.Done)
				{
					changePlayerPanel.depth = covePanel.depth - 1;
					changePlayerExplain.SetActive(false);
					base.gameObject.SetActive(false);
					TutorialInProgress = false;
					indicatePanel.gameObject.SetActive(false);
					GameBattle.m_instance.IsPause = false;
					DataCenter.Save().tutorialChangeMode = true;
					DataCenter.Save().SaveGameData();
				}
			}
		}

		public bool TutorialInProgress
		{
			get
			{
				return m_tutorialInProgress;
			}
			set
			{
				m_tutorialInProgress = value;
			}
		}

		public bool NextStep
		{
			get
			{
				return m_timer <= 0f;
			}
		}

		private void Awake()
		{
			if (m_instance == null)
			{
				m_instance = this;
			}
			if (movePanel != null)
			{
				moveExplain = movePanel.transform.Find("tutorialExplain").gameObject;
				if (TutorialPahseMove != TutorialPhaseState.InProgress)
				{
					moveExplain.SetActive(false);
				}
			}
			if (firePanel != null)
			{
				fireExplain = firePanel.transform.Find("tutorialExplain").gameObject;
				if (TutorialPahseFire != TutorialPhaseState.InProgress)
				{
					fireExplain.SetActive(false);
				}
			}
			if (skillPanel != null)
			{
				skillExplain = skillPanel.transform.Find("tutorialExplain").gameObject;
				if (TutorialPahseSkill != TutorialPhaseState.InProgress)
				{
					skillExplain.SetActive(false);
				}
			}
			if (changeModePanel != null)
			{
				changeModeExplain = changeModePanel.transform.Find("tutorialExplain").gameObject;
				if (TutorialPahseChangeMode != TutorialPhaseState.InProgress)
				{
					changeModeExplain.SetActive(false);
				}
			}
			if (minimapPanel != null)
			{
				minimapExplain = minimapPanel.transform.Find("tutorialExplain").gameObject;
				minimapExplain.SetActive(false);
			}
			if (changePlayerPanel != null)
			{
				changePlayerExplain = changePlayerPanel.transform.Find("tutorialExplain").gameObject;
				if (TutorialPahseChangePlayer != TutorialPhaseState.InProgress)
				{
					changePlayerExplain.SetActive(false);
				}
			}
			base.gameObject.SetActive(false);
		}

		private void Start()
		{
			if (m_tutorialPahseMove == TutorialPhaseState.InProgress && GameBattle.m_instance.GameState == GameBattle.State.Game)
			{
				indicatePanel.gameObject.SetActive(true);
			}
		}

		private void Update()
		{
			if (m_timer > 0f)
			{
				m_timer -= Time.deltaTime;
			}
		}
	}
}
