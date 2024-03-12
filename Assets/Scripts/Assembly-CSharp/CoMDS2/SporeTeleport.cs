using UnityEngine;

namespace CoMDS2
{
	public class SporeTeleport : DS2ActiveObject
	{
		private Transform m_attackIndicate;

		public SporeTeleport()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			base.clique = Clique.Computer;
			Transform transform = GetTransform();
			m_attackIndicate = transform.Find("AttackIndicate");
		}

		public void TeleportStart()
		{
			GetGameObject().SetActive(true);
			m_attackIndicate.gameObject.SetActive(false);
			effectPlayManager.PlayEffect("Attack");
		}

		public void TeleportEnd()
		{
			m_attackIndicate.gameObject.SetActive(false);
			effectPlayManager.PlayEffect("Attack_02");
		}

		public void SetAttackIndicate(Vector3 position)
		{
			m_attackIndicate.gameObject.SetActive(true);
			GetTransform().position = position;
		}

		public override void Update(float deltaTime)
		{
		}
	}
}
