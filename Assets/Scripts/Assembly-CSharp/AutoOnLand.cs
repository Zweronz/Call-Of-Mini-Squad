using UnityEngine;

public class AutoOnLand : MonoBehaviour
{
	public void Update()
	{
		if (base.transform.position.y != 0.2f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0.2f, base.transform.position.z);
		}
	}
}
