using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public float DestroyTime = 0.2f;

	private void Awake()
	{
		Invoke("Destroy", DestroyTime);
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}
