using UnityEngine;

public class BlazeFireShield : MonoBehaviour
{
	public Transform hostTransform;

	public Transform rotatePoint;

	private void Start()
	{
	}

	private void Update()
	{
		if (null != hostTransform)
		{
			base.transform.position = hostTransform.position;
		}
		rotatePoint.Rotate(Vector3.up * 100f * Time.deltaTime);
	}
}
