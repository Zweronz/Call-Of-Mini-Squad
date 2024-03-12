using UnityEngine;

namespace CoMDS2
{
	public class MathEx
	{
		public static float HorizontalAngle(Vector3 direction)
		{
			return Mathf.Atan2(direction.x, direction.z) * 57.29578f;
		}
	}
}
