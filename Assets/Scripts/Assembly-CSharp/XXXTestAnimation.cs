using UnityEngine;

public class XXXTestAnimation : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Animation>()["relive_02_01"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().Play("relive_02_01");
	}

	private void Update()
	{
	}
}
