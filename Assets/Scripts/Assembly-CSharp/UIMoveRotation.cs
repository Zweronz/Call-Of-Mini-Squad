using UnityEngine;

public class UIMoveRotation : MonoBehaviour
{
	public GameObject target;

	public float rotateFactor = 5f;

	public void SetTarget(GameObject go)
	{
		target = go;
	}

	public void SetRotateFactor(float factor)
	{
		rotateFactor = factor;
	}

	public void OnDrag(Vector2 delta)
	{
		if (target != null)
		{
			float num = rotateFactor * delta.x;
			target.transform.localEulerAngles = new Vector3(target.transform.localEulerAngles.x, target.transform.localEulerAngles.y - num, target.transform.localEulerAngles.z);
		}
	}
}
