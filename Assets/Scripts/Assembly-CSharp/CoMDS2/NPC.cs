using UnityEngine;

namespace CoMDS2
{
	public class NPC : Character
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		public override void Destroy(bool destroy = false)
		{
			base.Destroy(destroy);
		}

		public override IPathFinding GetPathFinding()
		{
			return this;
		}
	}
}
