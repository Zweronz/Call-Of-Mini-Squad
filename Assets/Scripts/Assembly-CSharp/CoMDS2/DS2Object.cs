using UnityEngine;

namespace CoMDS2
{
	public abstract class DS2Object
	{
		protected GameObject m_wrapGameObject;

		protected Transform m_wrapTransform;

		protected GameObject m_gameObject;

		protected Transform m_transform;

		protected int m_gameLayer;

		public Defined.OBJECT_TYPE objectType { get; set; }

		public string name { get; set; }

		public static int idCounter { get; set; }

		public int id { get; set; }

		public int Layer
		{
			get
			{
				return GetGameObject().layer;
			}
			set
			{
				m_gameLayer = value;
				GetGameObject().layer = value;
			}
		}

		public virtual void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			id = idCounter;
			idCounter++;
			this.name = name;
			m_gameObject = null;
			m_wrapGameObject = Object.Instantiate(prefab, position, Quaternion.identity) as GameObject;
			m_wrapGameObject.name = name + id;
			m_wrapTransform = m_wrapGameObject.transform;
			Transform transform = m_wrapTransform.Find("Model");
			if (transform != null)
			{
				m_gameObject = transform.gameObject;
			}
			else
			{
				m_gameObject = m_wrapGameObject;
			}
			m_transform = m_gameObject.transform;
			m_wrapTransform.position = position;
			m_wrapTransform.rotation = rotation;
			m_wrapGameObject.layer = (m_gameLayer = layer);
			DS2ObjectStub.BindObject(m_wrapGameObject, this);
			LoadResources();
		}

		public virtual void Initialize(GameObject gameObject)
		{
			id = idCounter;
			idCounter++;
			m_gameObject = null;
			m_wrapGameObject = gameObject;
			m_wrapTransform = m_wrapGameObject.transform;
			Transform transform = m_wrapTransform.Find("Model");
			if (transform != null)
			{
				m_gameObject = transform.gameObject;
			}
			else
			{
				m_gameObject = m_wrapGameObject;
			}
			m_transform = m_gameObject.transform;
			id++;
			DS2ObjectStub.BindObject(m_wrapGameObject, this);
		}

		protected virtual void LoadResources()
		{
		}

		public virtual void Update(float deltaTime)
		{
		}

		public GameObject GetGameObject()
		{
			return m_wrapGameObject;
		}

		public Transform GetTransform()
		{
			return m_wrapTransform;
		}

		public void LookAt(Transform target)
		{
			float f = GetTransform().position.y - target.position.y;
			if (Mathf.Abs(f) < 0.2f)
			{
				GetTransform().LookAt(target.position);
			}
		}

		public Transform GetModelTransform()
		{
			return m_wrapTransform;
		}

		public virtual ICollider GetCollider()
		{
			return null;
		}

		public virtual IFighter GetFighter()
		{
			return null;
		}

		public virtual void Destroy(bool destroy = false)
		{
			if (destroy)
			{
				Object.Destroy(m_wrapGameObject);
				m_gameObject = null;
				m_transform = null;
				m_wrapGameObject = null;
			}
			else
			{
				m_wrapGameObject.SetActive(false);
			}
		}
	}
}
