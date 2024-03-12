using UnityEngine;

namespace CoMDS2
{
	public class AutoLookAtCamera : MonoBehaviour
	{
		private void Update()
		{
			if (GameBattle.m_instance != null && GameBattle.m_instance.GetCameraWrap() != null)
			{
				base.transform.LookAt(GameBattle.m_instance.GetCameraWrap().transform);
			}
		}
	}
}
