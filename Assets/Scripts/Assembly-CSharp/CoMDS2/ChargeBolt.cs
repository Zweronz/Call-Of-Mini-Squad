using UnityEngine;

namespace CoMDS2
{
	public class ChargeBolt : DS2ActiveObject
	{
		public float burstInterval = 0.5f;

		public float burstLife = 3f;

		private float m_burstDelta;

		private float m_burstLifeDelta;

		public ChargeBolt()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
		}

		public void BoltBurst(Vector3 position)
		{
			GetTransform().position = position;
			GetGameObject().SetActive(true);
			m_burstDelta = 0f;
			m_burstLifeDelta = 0f;
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddObjToInteractObjectList(this);
			}
			else
			{
				BattleBufferManager.Instance.AddObjToInteractObjectListForUIExhibition(this);
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (!GetGameObject().activeSelf)
			{
				return;
			}
			m_burstDelta += deltaTime;
			m_burstLifeDelta += deltaTime;
			if (m_burstDelta >= burstInterval)
			{
				int layerMask = ((base.clique != 0) ? 1536 : 526336);
				Collider[] array = Physics.OverlapSphere(GetTransform().position, 3f, layerMask);
				for (int num = array.Length - 1; num >= 0; num--)
				{
					DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(array[num].gameObject);
					if (array[num].gameObject.activeInHierarchy && @object.Alive())
					{
						if (@object == null)
						{
						}
						base.hitInfo.hitPoint = GetTransform().position;
						@object.OnHit(base.hitInfo);
					}
				}
				m_burstDelta = 0f;
			}
			if (m_burstLifeDelta >= burstLife)
			{
				GetGameObject().SetActive(false);
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
				}
				else
				{
					BattleBufferManager.Instance.AddToInteractObjectNeedDeleteListForUIExhibition(this);
				}
			}
		}
	}
}
