using UnityEngine;

namespace CoMDS2
{
	public class AIStateSmashSelf : AIState
	{
		private float m_timeToSmash = 2f;

		private float m_timer;

		private float m_radius;

		private Vector3 m_originalScale;

		private int m_targetLayerMask;

		private bool m_scale;

		public AIStateSmashSelf(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
		}

		public void SetSmash(float time, float radius, int targetLayerMask, bool scale)
		{
			m_timeToSmash = time;
			m_radius = radius;
			m_targetLayerMask = targetLayerMask;
			m_scale = scale;
		}

		protected override void OnEnter()
		{
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)m_activeObject).GetAnimationName("Explode");
			}
			m_activeObject.AnimationPlay(base.animName, true);
			m_originalScale = m_activeObject.GetTransform().localScale;
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_scale)
			{
				m_activeObject.GetTransform().localScale += m_activeObject.GetTransform().localScale * deltaTime * 0.15f;
			}
			m_timer += deltaTime;
			if (m_timer >= m_timeToSmash)
			{
				m_timer = 0f;
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_BOMB_1, new Vector3(m_activeObject.GetTransform().position.x, 1f, m_activeObject.GetTransform().position.z), 2f);
				Collider[] array = Physics.OverlapSphere(m_activeObject.GetTransform().position, m_radius, m_targetLayerMask);
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
					HitInfo hitInfo = m_activeObject.GetHitInfo();
					hitInfo.repelDirection = @object.GetTransform().position - m_activeObject.GetTransform().position;
					hitInfo.source = m_activeObject;
					@object.OnHit(hitInfo);
				}
				m_activeObject.SetSplash(false);
				m_activeObject.GetTransform().localScale = m_originalScale;
				m_activeObject.OnDeath();
				m_activeObject.Destroy();
				GameBattle.m_instance.SetCameraQuake(Defined.CameraQuakeType.Quake_C);
			}
			else
			{
				m_activeObject.SetSplash(true);
			}
		}
	}
}
