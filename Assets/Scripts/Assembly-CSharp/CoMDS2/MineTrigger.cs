using UnityEngine;

namespace CoMDS2
{
	public class MineTrigger : MonoBehaviour
	{
		private Mine m_mine;

		private void Start()
		{
			m_mine = DS2ObjectStub.GetObject<Mine>(base.gameObject);
		}

		private void OnTriggerEnter(Collider other)
		{
			m_mine.Detonate();
		}
	}
}
