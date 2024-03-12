using UnityEngine;

namespace CoMDS2
{
	public class AIStateSeriousInjury : AIState
	{
		private float m_repelDis;

		private float m_repelTime;

		private float m_repelSpeed;

		private Vector3 m_direction;

		private float m_timer;

		private bool m_bRepel;

		private CharacterController m_characterController;

		private bool m_bCheckWin;

		private bool m_bToResurgence;

		public AIStateSeriousInjury(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_characterController = obj.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetRepel(float dis, float time, Vector3 dir)
		{
			m_repelDis = dis;
			m_repelTime = time;
			m_repelSpeed = dis / time;
			m_direction = dir;
			m_timer = 0f;
			m_bRepel = true;
		}

		protected override void OnEnter()
		{
			m_bCheckWin = false;
			m_bToResurgence = false;
			m_characterController = m_activeObject.GetGameObject().GetComponent<CharacterController>();
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				DS2ActiveObject activeObject = m_activeObject;
				string animLowerBody = (base.animName = ((Character)m_activeObject).GetAnimationNameByWeapon("Death"));
				activeObject.animLowerBody = animLowerBody;
			}
			else if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)m_activeObject).GetAnimationName("Death");
			}
			m_activeObject.AnimationCrossFade(base.animName, false);
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_bRepel)
			{
				m_timer += deltaTime;
				if (m_timer >= m_repelTime)
				{
					m_timer = 0f;
					m_bRepel = false;
				}
				else if ((bool)m_characterController)
				{
					m_characterController.Move(m_direction.normalized * m_repelSpeed * deltaTime);
				}
				else
				{
					m_activeObject.GetTransform().Translate(m_direction.normalized * m_repelSpeed * deltaTime, Space.World);
				}
			}
			if (!m_bCheckWin && !m_activeObject.AnimationPlaying(base.animName))
			{
				m_bRepel = false;
				if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
				{
					Player player = (Player)m_activeObject;
					if (player.willResurgence)
					{
						m_timer = 0f;
						m_bToResurgence = true;
						player.audioTalkManager.PlayLife();
						BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.heores_Prayer, player.m_effectPointUpHead.position, 0f, player.m_effectPointUpHead);
						m_bCheckWin = true;
						return;
					}
					DS2ActiveObject[] playerList = GameBattle.m_instance.GetPlayerList();
					bool flag = true;
					for (int i = 0; i < playerList.Length; i++)
					{
						if (playerList[i].Alive())
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						GameBattle.m_instance.GameState = GameBattle.State.Failed;
						return;
					}
					DS2ActiveObject[] playerList2 = GameBattle.m_instance.GetPlayerList();
					for (int j = 0; j < playerList2.Length; j++)
					{
						Player player2 = (Player)playerList2[j];
						if (player2 != player && player2.SquadSite > player.SquadSite)
						{
							player2.SquadSite--;
							if (player2.SquadSite == 0)
							{
								GameBattle.m_instance.ChangeCurrentContorlPlayer(player2);
							}
						}
					}
					player.SquadSite = playerList2.Length - 1;
					if (DataCenter.Save().squadMode)
					{
						GameBattle.m_instance.GetSquadController().UpdatePointsPosition();
					}
				}
				m_bCheckWin = true;
			}
			if (m_bToResurgence)
			{
				m_timer += Time.deltaTime;
				if (m_timer >= 2f)
				{
					Player player3 = (Player)m_activeObject;
					float resHpPercent = DataCenter.Conf().m_teamAttributeResurrection.resHpPercent;
					player3.hp = (int)((float)player3.hpMax * resHpPercent);
					player3.Reset();
					player3.SwitchFSM(player3.GetDefaultAIState());
				}
			}
		}
	}
}
