using UnityEngine;

namespace CoMDS2
{
	public class BulletTriigerChargeBolt : MonoBehaviour
	{
		public ChargeBolt chargeBolt;

		public void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer != 26)
			{
				base.gameObject.SetActive(false);
				chargeBolt.GetTransform().position = base.transform.position;
				chargeBolt.GetGameObject().SetActive(true);
				chargeBolt.clique = ((base.gameObject.layer == 23) ? DS2ActiveObject.Clique.Computer : DS2ActiveObject.Clique.Player);
				chargeBolt.BoltBurst(base.transform.position);
			}
		}
	}
}
