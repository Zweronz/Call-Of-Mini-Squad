using UnityEngine;

public class TestGuter : MonoBehaviour
{
	private Animation ani;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			RaycastHit[] array = Physics.RaycastAll(base.transform.position + Vector3.up * 0.1f, base.transform.forward);
			for (int i = 0; i < array.Length; i++)
			{
			}
		}
	}
}
