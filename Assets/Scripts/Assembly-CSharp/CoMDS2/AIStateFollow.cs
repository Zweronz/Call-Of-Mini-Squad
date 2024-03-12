using UnityEngine;

namespace CoMDS2
{
	public class AIStateFollow : AIState
	{
		private DS2ActiveObject m_target;

		private float m_fTargetRange;

		private Character m_character;

		private bool m_attack;

		private float m_originalSpeed;

		public AIStateFollow(Character npc, string name, Controller controller = Controller.System)
			: base(npc, name, controller)
		{
			m_character = npc;
		}

		public void SetFollow(DS2ActiveObject target, float targetRange)
		{
			m_target = target;
			m_fTargetRange = targetRange;
		}

		protected override void OnEnter()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				if (m_character.m_weapon != null && !m_character.m_weapon.NeedReload())
				{
					m_activeObject.AnimationStop(m_activeObject.animUpperBody);
				}
				base.animName = m_character.animLowerBody;
				if (!DataCenter.State().isPVPMode && m_character.clique == DS2ActiveObject.Clique.Player)
				{
					m_target = GameBattle.m_instance.GetPlayer();
					m_originalSpeed = m_character.baseAttribute.moveSpeed * 0.5f;
					m_character.MoveSpeed += m_originalSpeed;
				}
			}
			m_attack = false;
		}

		protected override void OnExit()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && m_character.clique == DS2ActiveObject.Clique.Player && !DataCenter.State().isPVPMode)
			{
				m_character.MoveSpeed -= m_originalSpeed;
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_character.MoveDirection = m_target.GetTransform().position - m_activeObject.GetTransform().position;
			if (DataCenter.Save().m_bTeamMemberAutoAttack && m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && !DataCenter.State().isPVPMode)
			{
				if (Util.s_allyMoveAttack)
				{
					if (!m_attack)
					{
						if (!m_character.m_weapon.NeedReload())
						{
							DS2ActiveObject dS2ActiveObject;
							if (DataCenter.State().isPVPMode)
							{
								dS2ActiveObject = m_character.lockedTarget;
								if (dS2ActiveObject == null)
								{
									dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
								}
							}
							else
							{
								dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_character);
							}
							if (dS2ActiveObject != null && dS2ActiveObject.Alive())
							{
								float num = m_character.shootRange * m_character.shootRange;
								float num2 = m_character.meleeRange * m_character.meleeRange;
								float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
								if (sqrMagnitude <= num - 10f)
								{
									if (!Util.s_allyMoveAttack)
									{
										IPathFinding pathFinding = m_character.GetPathFinding();
										if (pathFinding != null && pathFinding.HasNavigation())
										{
											pathFinding.StopNav();
										}
										m_activeObject.ChangeToDefaultAIState();
										return;
									}
									m_attack = true;
									Push(m_character.GetAIState("UpperFire"));
								}
							}
						}
					}
					else
					{
						DS2ActiveObject dS2ActiveObject2;
						if (DataCenter.State().isPVPMode)
						{
							dS2ActiveObject2 = m_character.lockedTarget;
							if (dS2ActiveObject2 == null)
							{
								dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
							}
						}
						else
						{
							dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_character);
						}
						if (dS2ActiveObject2 != null && dS2ActiveObject2.Alive())
						{
							float num3 = m_character.shootRange * m_character.shootRange;
							float num4 = m_character.meleeRange * m_character.meleeRange;
							float sqrMagnitude2 = (dS2ActiveObject2.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
							if (sqrMagnitude2 >= num3)
							{
								m_attack = false;
								Pop();
								if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
								{
									m_activeObject.AnimationStop(m_activeObject.animUpperBody);
								}
							}
						}
						else if (dS2ActiveObject2 == null || !dS2ActiveObject2.Alive())
						{
							m_attack = false;
							Pop();
							if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
							{
								m_activeObject.AnimationStop(m_activeObject.animUpperBody);
							}
						}
					}
				}
				if (!m_attack)
				{
					Character character = m_character;
					Vector3 forward = m_character.GetModelTransform().forward;
					m_character.MoveDirection = forward;
					character.FaceDirection = forward;
					base.animName = m_character.animLowerBody;
				}
			}
			IPathFinding pathFinding2 = m_character.GetPathFinding();
			if (pathFinding2 != null && pathFinding2.HasNavigation())
			{
				pathFinding2.SetNavDesination(m_target.GetTransform().position);
			}
			float sqrMagnitude3 = (m_target.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
			if (DataCenter.State().isPVPMode)
			{
				m_fTargetRange = m_character.shootRange;
			}
			else if (Util.s_allyMoveAttack && GameBattle.m_instance.IsInBattle)
			{
				m_fTargetRange = 11f;
			}
			else
			{
				m_fTargetRange = Player.ALLY_TOFOLLOW_DIS + 1f;
			}
			if ((double)sqrMagnitude3 < (double)(m_fTargetRange * m_fTargetRange) * 0.5)
			{
				if (DataCenter.State().isPVPMode)
				{
					m_activeObject.ChangeToDefaultAIState();
				}
				else
				{
					NextState(m_activeObject.GetAIState("FindSeat"));
				}
			}
		}
	}
}
