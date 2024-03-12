using UnityEngine;

namespace CoMDS2
{
	public class BulletSpore : Bullet
	{
		public bool crossExplode;

		private SporeExplode m_explode;

		private DS2ActiveObject m_hitObj;

		public BulletSpore(DS2Object creator)
			: base(creator)
		{
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			m_explode = new SporeExplode();
			GameObject prefab2 = Resources.Load<GameObject>("Models/ActiveObjects/Spore/SporeGreenExplode");
			m_explode.Initialize(prefab2, "SporeExplode", Vector3.zero, Quaternion.identity, 0);
			m_explode.GetGameObject().SetActive(false);
			m_explode.GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
		}

		public override void Emit(float distanceLife = 0f)
		{
			m_hitObj = null;
			base.Emit(distanceLife);
		}

		public override void TriggerEnter(DS2Object obj)
		{
			m_hitObj = (DS2ActiveObject)obj;
			base.TriggerEnter(obj);
		}

		public override void Destroy(bool destroy = false)
		{
			m_explode.Explode(crossExplode, base.hitInfo, GetTransform().position, m_hitObj);
			base.Destroy(destroy);
		}
	}
}
