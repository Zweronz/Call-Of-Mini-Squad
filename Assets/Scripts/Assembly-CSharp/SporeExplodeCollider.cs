using CoMDS2;
using UnityEngine;

public class SporeExplodeCollider : MonoBehaviour
{
	public enum ColliderType
	{
		X = 0,
		Z = 1
	}

	public SporeExplode sporeExplode;

	public ColliderType type;

	private void Start()
	{
		if (GameBattle.m_instance == null)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (sporeExplode != null)
		{
			sporeExplode.AddHitObject(DS2ObjectStub.GetObject<DS2ActiveObject>(other.gameObject));
		}
	}
}
