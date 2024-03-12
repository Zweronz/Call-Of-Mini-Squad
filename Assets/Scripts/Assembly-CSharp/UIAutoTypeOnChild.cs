using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Auto CustomPosition On Child")]
public class UIAutoTypeOnChild : MonoBehaviour
{
	public enum CustomPositionType
	{
		FrontH = 0,
		FrontV = 1,
		BehindH = 2,
		BehindV = 3,
		Center = 4
	}

	public bool bWholePage;

	public float offect;

	public GameObject target;

	public string functionName;

	public bool includeChildren;

	private GameObject[] lastCurrentGOs = new GameObject[2];

	public CustomPositionType customPositionType = CustomPositionType.Center;

	public float springStrength = 8f;

	public SpringPanel.OnFinished onFinished;

	private UIScrollView mDrag;

	private GameObject mCenteredObject;

	public GameObject centeredObject
	{
		get
		{
			return mCenteredObject;
		}
	}

	private bool CheckBSameGO(GameObject go)
	{
		if (lastCurrentGOs[1] != null)
		{
			return (lastCurrentGOs[1] == go) ? true : false;
		}
		if (lastCurrentGOs[0] != null)
		{
			return (lastCurrentGOs[0] == go) ? true : false;
		}
		return false;
	}

	protected void SetCurrentGO(GameObject go)
	{
		if (!CheckBSameGO(go))
		{
			if (lastCurrentGOs[0] == null)
			{
				lastCurrentGOs[0] = go;
				SendMSG();
			}
			else if (lastCurrentGOs[1] == null)
			{
				lastCurrentGOs[1] = go;
				SendMSG();
			}
			else
			{
				lastCurrentGOs[0] = lastCurrentGOs[1];
				lastCurrentGOs[1] = go;
				SendMSG();
			}
		}
	}

	private void SendMSG()
	{
		if (string.IsNullOrEmpty(functionName))
		{
			return;
		}
		if (target == null)
		{
			target = base.gameObject;
		}
		if (includeChildren)
		{
			Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				Transform transform = componentsInChildren[i];
				transform.gameObject.SendMessage(functionName, GetCurrentGO(), SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			target.SendMessage(functionName, GetCurrentGO(), SendMessageOptions.DontRequireReceiver);
		}
	}

	public GameObject GetCurrentGO()
	{
		if (lastCurrentGOs[1] != null)
		{
			return lastCurrentGOs[1];
		}
		if (lastCurrentGOs[0] != null)
		{
			return lastCurrentGOs[0];
		}
		return null;
	}

	private void OnEnable()
	{
		Recenter();
	}

	private void OnDragFinished()
	{
		if (base.enabled)
		{
			Recenter();
		}
	}

	private void CenterOn(Transform target, Vector3 panelCenter)
	{
		if (target != null && mDrag != null && mDrag.panel != null)
		{
			SetCurrentGO(target.gameObject);
			Transform cachedTransform = mDrag.panel.cachedTransform;
			mCenteredObject = target.gameObject;
			Vector3 vector = cachedTransform.InverseTransformPoint(target.position);
			Vector3 vector2 = cachedTransform.InverseTransformPoint(panelCenter);
			Vector3 vector3 = vector - vector2;
			if (!mDrag.canMoveHorizontally)
			{
				vector3.x = 0f;
			}
			if (!mDrag.canMoveVertically)
			{
				vector3.y = 0f;
			}
			vector3.z = 0f;
			SpringPanel.Begin(mDrag.panel.cachedGameObject, cachedTransform.localPosition - vector3, springStrength).onFinished = onFinished;
		}
		else
		{
			mCenteredObject = null;
		}
	}

	public void CenterOn(Transform target)
	{
		if (mDrag != null && mDrag.panel != null)
		{
			Vector3[] worldCorners = mDrag.panel.worldCorners;
			Vector3 panelCenter = (worldCorners[2] + worldCorners[0]) * 0.5f;
			CenterOn(target, panelCenter);
		}
	}

	public void Recenter()
	{
		if (mDrag == null)
		{
			mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
			if (mDrag == null)
			{
				base.enabled = false;
				return;
			}
			mDrag.onDragFinished = OnDragFinished;
			if (mDrag.horizontalScrollBar != null)
			{
				mDrag.horizontalScrollBar.onDragFinished = OnDragFinished;
			}
			if (mDrag.verticalScrollBar != null)
			{
				mDrag.verticalScrollBar.onDragFinished = OnDragFinished;
			}
		}
		if (mDrag.panel == null)
		{
			return;
		}
		Vector3[] worldCorners = mDrag.panel.worldCorners;
		Vector3 vector = (worldCorners[2] + worldCorners[0]) * 0.5f;
		if (customPositionType == CustomPositionType.Center)
		{
			vector = (worldCorners[2] + worldCorners[0]) * 0.5f;
		}
		else
		{
			Vector3 vector2 = Vector3.zero;
			if (base.transform.childCount > 1 && !bWholePage)
			{
				vector2 = base.transform.GetChild(1).position - base.transform.GetChild(0).position;
			}
			if (customPositionType == CustomPositionType.FrontH)
			{
				vector = worldCorners[0] + new Vector3(vector2.x / 2f - offect, 0f, 0f);
			}
			else if (customPositionType == CustomPositionType.FrontV)
			{
				vector = worldCorners[1] + new Vector3(0f, vector2.y / 2f - offect, 0f);
			}
		}
		Vector3 vector3 = vector - mDrag.currentMomentum * (mDrag.momentumAmount * 0.1f);
		mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int i = 0;
		for (int childCount = transform2.childCount; i < childCount; i++)
		{
			Transform child = transform2.GetChild(i);
			float num2 = Vector3.SqrMagnitude(child.position - vector3);
			if (num2 < num)
			{
				num = num2;
				transform = child;
			}
		}
		CenterOn(transform, vector);
	}
}
