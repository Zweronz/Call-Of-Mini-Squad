using UnityEngine;

public class UITeamTeamInfoTweenPosReset : MonoBehaviour
{
	public Transform transA;

	public Transform transB;

	private int count = 100;

	private bool bHandleBtnToCheck;

	private void Awake()
	{
		Check();
	}

	public void Check()
	{
		Vector3 vector = base.transform.parent.InverseTransformPoint(transA.position);
		Vector3 vector2 = base.transform.parent.InverseTransformPoint(transB.position);
		float y = Mathf.Abs((vector - vector2).y);
		TweenPosition component = base.gameObject.GetComponent<TweenPosition>();
		if (component != null)
		{
			component.to = new Vector3(component.to.x, y, component.to.z);
		}
	}

	public void HandleBtnToCheck()
	{
		if (!bHandleBtnToCheck)
		{
			Check();
			bHandleBtnToCheck = true;
		}
	}
}
