using UnityEngine;

namespace CoMDS2
{
	public class BuildingX1Tower : DS2Building
	{
		public override void Initialize(GameObject gameObject)
		{
			base.Initialize(gameObject);
			base.clique = Clique.Computer;
		}
	}
}
