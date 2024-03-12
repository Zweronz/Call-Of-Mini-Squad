using UnityEngine;

public class JointPoint : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.gameObject.transform.position, 0.2f);
	}
}
