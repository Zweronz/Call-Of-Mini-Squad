using UnityEngine;

public class UtilUIBase3DMapStateControlManager : MonoBehaviour
{
	public static UtilUIBase3DMapStateControlManager mInstance;

	public UtilUIBase3DMapStateControlInfo[] arr3DMapStateControl;

	private void Awake()
	{
		mInstance = this;
	}

	public void SetState(int worldIndex, bool bCurrent, bool bUnlock, bool bClear)
	{
		arr3DMapStateControl[worldIndex].SetState(bCurrent, bUnlock, bClear);
	}
}
