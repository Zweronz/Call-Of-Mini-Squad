using UnityEngine;

public class GizmosPoint : MonoBehaviour
{
	public Color color = Color.white;

	public float radius = 0.1f;

	public void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(base.transform.position, radius);
	}
}
