using UnityEngine;

public class AnimCallBack : MonoBehaviour
{
	public GameObject[] targetObj;

	private void Awake()
	{
		if (targetObj != null)
		{
			return;
		}
		for (int i = 0; i < targetObj.Length; i++)
		{
			int childCount = targetObj[i].transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				targetObj[i].transform.GetChild(j).gameObject.active = false;
			}
			targetObj[i].active = false;
		}
	}

	private void Start()
	{
	}

	private void PlayEffect(int index)
	{
		if (targetObj[index] == null)
		{
		}
		int childCount = targetObj[index].transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			targetObj[index].transform.GetChild(i).gameObject.active = true;
		}
		targetObj[index].active = true;
	}
}
