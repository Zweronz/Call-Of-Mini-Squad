using UnityEngine;

namespace CoMDS2
{
	public class AIStateHurt : AIState
	{
		private Character m_character;

		private float m_hurtTime;

		private float m_timer;

		private float m_hurtFrequency = 2f;

		private float m_lastHurtTime;

		public GameObject target { get; set; }

		public float HurtTime
		{
			get
			{
				return m_hurtTime;
			}
			set
			{
				m_hurtTime = value;
			}
		}

		public bool ShouldHurt
		{
			get
			{
				return Time.realtimeSinceStartup - m_lastHurtTime > m_hurtFrequency;
			}
		}

		public AIStateHurt(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		protected override void OnEnter()
		{
			m_timer = 0f;
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_character;
				if (m_character.m_move)
				{
					if (player.CurrentController && !player.AutoControl)
					{
						Push(m_character.GetAIState("Move"));
					}
					else
					{
						Push(m_character.GetAIState("AllyFollow"));
					}
				}
				m_character.animUpperBody = m_character.GetAnimationNameByWeapon("Hurt");
				base.animName2 = m_character.animUpperBody;
			}
			else if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName2 = ((Enemy)m_activeObject).GetAnimationName("Hurt");
				if (!m_character.m_move)
				{
					base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle");
					m_activeObject.AnimationCrossFade(base.animName, true);
				}
				IPathFinding pathFinding = m_character.GetPathFinding();
				if (pathFinding != null && pathFinding.HasNavigation())
				{
					pathFinding.StopNav();
				}
			}
			if (m_hurtTime > 0f)
			{
				float speed = m_activeObject.AnimationLength(base.animName2) / m_hurtTime;
				m_activeObject.SetAnimationSpeed(base.animName2, speed);
			}
			m_activeObject.AnimationPlay(base.animName2, false);
			m_lastHurtTime = Time.realtimeSinceStartup;
		}

		protected override void OnExit()
		{
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				m_activeObject.ChangeToDefaultAIState();
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_timer += Time.deltaTime;
			if (m_timer >= m_activeObject.AnimationLength(base.animName2) && m_timer >= m_hurtTime)
			{
				if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
				{
					m_activeObject.ChangeAIState("Chase");
				}
				else
				{
					m_activeObject.ChangeToDefaultAIState();
				}
			}
		}
	}
}
