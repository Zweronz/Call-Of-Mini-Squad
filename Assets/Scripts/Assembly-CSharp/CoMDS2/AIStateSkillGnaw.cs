using UnityEngine;

namespace CoMDS2
{
	public class AIStateSkillGnaw : AIState
	{
		private float m_timer;

		private float m_timerAttack;

		private DataConf.SkillInfo m_skillInfo;

		private Character m_character;

		private Character m_target;

		public AIStateSkillGnaw(Character obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_character = obj;
		}

		public void SetSkillInfo(DataConf.SkillInfo skillInfo, Character target)
		{
			m_skillInfo = skillInfo;
			m_target = target;
		}

		protected override void OnEnter()
		{
			m_character.AnimationStop(m_activeObject.animUpperBody);
			string animProcess = m_skillInfo.animProcess;
			m_character.animLowerBody = animProcess;
			base.animName = animProcess;
			m_character.AnimationPlay(base.animName, true);
			m_character.LookAt(m_target.GetTransform());
			m_character.isRage = true;
			m_target.isStuck = true;
			m_target.SwitchFSM(m_target.GetAIState("Stuck"));
			m_character.SkillStart();
			m_timer = 0f;
			m_timerAttack = 0f;
		}

		protected override void OnExit()
		{
			m_character.ClearGrabList();
			m_character.isRage = false;
			m_target.isStuck = false;
			if (m_target.Alive())
			{
				m_target.SwitchFSM(m_target.GetDefaultAIState());
			}
			m_character.SkillEnd();
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (!m_target.Alive())
			{
				m_character.ChangeToDefaultAIState();
				return;
			}
			m_timer += Time.deltaTime;
			if (m_timer >= m_skillInfo.time)
			{
				m_character.ChangeToDefaultAIState();
				return;
			}
			m_timerAttack += Time.deltaTime;
			if (m_timerAttack >= m_skillInfo.speed)
			{
				m_timerAttack = 0f;
				m_target.OnHit(m_character.skillHitInfo);
			}
		}
	}
}
