using System.Collections.Generic;
using CoMDS2;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
	public enum TriggerTarget
	{
		Player = 9,
		Ally = 10,
		Enemy = 11,
		All = 12
	}

	public enum EventType
	{
		SliderDoor = 0,
		SpawnEnemy = 1,
		Trap = 2,
		EnableObject = 3,
		DisableObject = 4,
		ElectricDoor = 5
	}

	public bool enable = true;

	public TriggerTarget triggerTarget;

	public List<GameObject> eventTarget;

	public EventType eventType;

	public int triggerTimes = -1;

	public GameObject[] implicateGameObjects;

	private List<DS2ActiveObject> m_objects;

	private GameObject m_unlockedDoor;

	private GameObject m_lockedDoor;

	private int m_triggerTimes;

	private bool m_checkObjects;

	public bool Enable
	{
		get
		{
			return enable;
		}
		set
		{
			enable = value;
			if (eventType == EventType.SliderDoor)
			{
				if (enable)
				{
					m_lockedDoor.SetActive(false);
					m_unlockedDoor.SetActive(true);
					if (eventTarget != null)
					{
						foreach (GameObject item in eventTarget)
						{
							if (item != null)
							{
								item.SetActive(true);
								item.SendMessage("ExecuteEvent", "Enter");
							}
						}
					}
					foreach (DS2ActiveObject @object in m_objects)
					{
						UnityEngine.AI.NavMeshAgent component = @object.GetGameObject().GetComponent<UnityEngine.AI.NavMeshAgent>();
						if (component != null)
						{
							component.areaMask |= 8;
						}
					}
				}
				else if (eventTarget != null)
				{
					foreach (GameObject item2 in eventTarget)
					{
						item2.SendMessage("ExecuteEvent", "CloseOff");
					}
				}
				if (implicateGameObjects != null)
				{
					for (int num = implicateGameObjects.Length - 1; num >= 0; num--)
					{
						if (GameBattle.m_instance != null)
						{
							int instanceID = implicateGameObjects[num].GetInstanceID();
							GameBattle.m_instance.SetNodeGroupEnable(instanceID, enable);
						}
					}
				}
			}
			if (eventType != EventType.ElectricDoor)
			{
				return;
			}
			if (enable)
			{
				eventTarget[0].SetActive(false);
				foreach (DS2ActiveObject object2 in m_objects)
				{
					UnityEngine.AI.NavMeshAgent component2 = object2.GetGameObject().GetComponent<UnityEngine.AI.NavMeshAgent>();
					if (component2 != null)
					{
						component2.areaMask |= 8;
					}
				}
			}
			else
			{
				eventTarget[0].SetActive(true);
			}
			if (implicateGameObjects == null)
			{
				return;
			}
			for (int num2 = implicateGameObjects.Length - 1; num2 >= 0; num2--)
			{
				if (GameBattle.m_instance != null)
				{
					int instanceID2 = implicateGameObjects[num2].GetInstanceID();
					GameBattle.m_instance.SetNodeGroupEnable(instanceID2, enable);
				}
			}
		}
	}

	private void Awake()
	{
		m_objects = new List<DS2ActiveObject>();
	}

	private void Start()
	{
		if (eventType == EventType.SliderDoor)
		{
			m_unlockedDoor = base.transform.Find("UnlockedDoor").gameObject;
			m_lockedDoor = base.transform.Find("LockedDoor").gameObject;
			if (enable)
			{
				m_lockedDoor.SetActive(false);
				m_unlockedDoor.SetActive(true);
				if (implicateGameObjects == null)
				{
					return;
				}
				for (int num = implicateGameObjects.Length - 1; num >= 0; num--)
				{
					GameObject gameObject = implicateGameObjects[num].transform.Find("SpawnPointInNodeGroup").gameObject;
					gameObject.SetActive(true);
					int instanceID = implicateGameObjects[num].GetInstanceID();
					if (GameBattle.m_instance != null)
					{
						GameBattle.m_instance.SetNodeGroupEnable(instanceID, true);
					}
				}
				return;
			}
			m_lockedDoor.SetActive(true);
			m_unlockedDoor.SetActive(false);
			if (implicateGameObjects == null)
			{
				return;
			}
			for (int num2 = implicateGameObjects.Length - 1; num2 >= 0; num2--)
			{
				GameObject gameObject2 = implicateGameObjects[num2].transform.Find("SpawnPointInNodeGroup").gameObject;
				gameObject2.SetActive(false);
				int instanceID2 = implicateGameObjects[num2].GetInstanceID();
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.SetNodeGroupEnable(instanceID2, false);
				}
			}
		}
		else
		{
			if (eventType != EventType.ElectricDoor)
			{
				return;
			}
			if (enable)
			{
				eventTarget[0].SetActive(false);
				if (implicateGameObjects == null)
				{
					return;
				}
				for (int num3 = implicateGameObjects.Length - 1; num3 >= 0; num3--)
				{
					GameObject gameObject3 = implicateGameObjects[num3].transform.Find("SpawnPointInNodeGroup").gameObject;
					gameObject3.SetActive(true);
					int instanceID3 = implicateGameObjects[num3].GetInstanceID();
					if (GameBattle.m_instance != null)
					{
						GameBattle.m_instance.SetNodeGroupEnable(instanceID3, true);
					}
				}
				return;
			}
			eventTarget[0].SetActive(true);
			if (implicateGameObjects == null)
			{
				return;
			}
			for (int num4 = implicateGameObjects.Length - 1; num4 >= 0; num4--)
			{
				GameObject gameObject4 = implicateGameObjects[num4].transform.Find("SpawnPointInNodeGroup").gameObject;
				gameObject4.SetActive(false);
				int instanceID4 = implicateGameObjects[num4].GetInstanceID();
				if (GameBattle.m_instance != null)
				{
					GameBattle.m_instance.SetNodeGroupEnable(instanceID4, false);
				}
			}
		}
	}

	public void Update()
	{
	}

	public void OnTriggerEnter(Collider other)
	{
		if (eventType == EventType.SliderDoor || eventType == EventType.ElectricDoor)
		{
			if (enable)
			{
				UnityEngine.AI.NavMeshAgent component = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
				if (component != null)
				{
					component.areaMask |= 8;
				}
			}
			else
			{
				UnityEngine.AI.NavMeshAgent component2 = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
				if (component2 != null)
				{
					component2.areaMask = -9;
				}
			}
		}
		if (triggerTarget != TriggerTarget.All && triggerTarget != (TriggerTarget)other.gameObject.layer)
		{
			return;
		}
		DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject);
		@object.AddImplicate(base.gameObject);
		m_objects.Add(@object);
		if (!enable)
		{
			return;
		}
		m_triggerTimes++;
		switch (eventType)
		{
		case EventType.EnableObject:
			if (eventTarget == null)
			{
				return;
			}
			{
				foreach (GameObject item in eventTarget)
				{
					TriggerEvent component3 = item.GetComponent<TriggerEvent>();
					if (component3 != null)
					{
						component3.Enable = true;
					}
					else
					{
						item.SetActive(true);
					}
				}
				return;
			}
		case EventType.DisableObject:
			if (eventTarget == null)
			{
				return;
			}
			{
				foreach (GameObject item2 in eventTarget)
				{
					TriggerEvent component4 = item2.GetComponent<TriggerEvent>();
					if (component4 != null)
					{
						if (component4.Enable)
						{
							component4.Enable = false;
						}
					}
					else
					{
						item2.SetActive(false);
					}
				}
				return;
			}
		case EventType.SliderDoor:
		case EventType.ElectricDoor:
			if (@object.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				GameBattle.m_instance.GetSquadController().SquadRadius = 1f;
			}
			break;
		}
		if (eventType == EventType.ElectricDoor || eventTarget == null)
		{
			return;
		}
		foreach (GameObject item3 in eventTarget)
		{
			if (item3 != null)
			{
				item3.SendMessage("ExecuteEvent", "Enter");
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (this.eventType == EventType.SliderDoor || this.eventType == EventType.ElectricDoor)
		{
			UnityEngine.AI.NavMeshAgent component = other.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (component != null)
			{
				component.areaMask |= 8;
			}
		}
		if (triggerTarget != TriggerTarget.All && triggerTarget != (TriggerTarget)other.gameObject.layer)
		{
			return;
		}
		DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject);
		if (m_objects.Contains(@object))
		{
			m_objects.Remove(@object);
			@object.RemoveImplicate(base.gameObject);
		}
		if (!enable)
		{
			return;
		}
		EventType eventType = this.eventType;
		if ((eventType == EventType.SliderDoor || eventType == EventType.ElectricDoor) && @object.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
		{
			GameBattle.m_instance.GetSquadController().SquadRadius = 1.8f;
		}
		if (this.eventType == EventType.ElectricDoor || m_objects.Count != 0)
		{
			return;
		}
		if (eventTarget != null)
		{
			foreach (GameObject item in eventTarget)
			{
				if (item != null)
				{
					item.SendMessage("ExecuteEvent", "Exit");
				}
			}
		}
		if (triggerTimes != -1 && m_triggerTimes >= triggerTimes)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void OnTriggerStay(Collider other)
	{
	}

	public void TargetDead(DS2ActiveObject obj)
	{
		if (!m_objects.Contains(obj))
		{
			return;
		}
		m_objects.Remove(obj);
		if (eventType == EventType.ElectricDoor || !enable || m_objects.Count != 0 || eventTarget == null)
		{
			return;
		}
		foreach (GameObject item in eventTarget)
		{
			item.SendMessage("ExecuteEvent", "Exit");
		}
	}

	public void ExecuteEvent(string eventName)
	{
		if (!(eventName == "Enter") && !(eventName == "Exit"))
		{
		}
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 0f);
		if (eventTarget == null)
		{
			return;
		}
		foreach (GameObject item in eventTarget)
		{
			if (item != null)
			{
				Gizmos.DrawLine(base.transform.position, item.transform.position);
			}
		}
	}

	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0.8f, 0f, 0.8f);
		Gizmos.DrawCube(base.gameObject.transform.position, new Vector3(1.5f, 3f, 1.5f));
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		if (eventTarget == null)
		{
			return;
		}
		foreach (GameObject item in eventTarget)
		{
			if (item != null)
			{
				Gizmos.DrawLine(base.transform.position, item.transform.position);
			}
		}
	}
}
