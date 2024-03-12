using UnityEngine;

namespace CoMDS2
{
	public class BulletAK47 : Bullet
	{
		public BulletAK47(DS2Object creator)
			: base(creator)
		{
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
		}
	}
}
