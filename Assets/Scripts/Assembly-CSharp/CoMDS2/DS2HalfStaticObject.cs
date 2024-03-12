using UnityEngine;

namespace CoMDS2
{
	public class DS2HalfStaticObject : DS2Object
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public new virtual void Initialize(GameObject gameObject)
		{
			base.Initialize(gameObject);
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}
	}
}
