using UnityEngine;

public class prefabDestroyerCS : MonoBehaviour
{
	private float overTime;

	private float countTime;

	public float OverTime
	{
		set
		{
			overTime = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		countTime += Time.deltaTime;
		if (countTime > 0f && countTime >= overTime)
		{
			countTime = -1f;
			Object.Destroy(base.gameObject);
		}
	}
}
