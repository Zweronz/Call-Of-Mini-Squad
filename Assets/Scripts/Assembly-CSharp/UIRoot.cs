using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Root")]
public class UIRoot : MonoBehaviour
{
	public enum Scaling
	{
		PixelPerfect = 0,
		FixedSize = 1,
		FixedSizeOnMobiles = 2
	}

	public static List<UIRoot> list = new List<UIRoot>();

	public Scaling scalingStyle;

	public int manualHeight = 720;

	public int minimumHeight = 320;

	public int maximumHeight = 1536;

	private Transform mTrans;

	public int activeHeight
	{
		get
		{
			int num = Mathf.Max(2, Screen.height);
			if (scalingStyle == Scaling.FixedSize)
			{
				return manualHeight;
			}
			if (scalingStyle == Scaling.FixedSizeOnMobiles)
			{
				return manualHeight;
			}
			if (num < minimumHeight)
			{
				return minimumHeight;
			}
			if (num > maximumHeight)
			{
				return maximumHeight;
			}
			return num;
		}
	}

	public float pixelSizeAdjustment
	{
		get
		{
			return GetPixelSizeAdjustment(Screen.height);
		}
	}

	public static float GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(go);
		return (!(uIRoot != null)) ? 1f : uIRoot.pixelSizeAdjustment;
	}

	public float GetPixelSizeAdjustment(int height)
	{
		height = Mathf.Max(2, height);
		if (scalingStyle == Scaling.FixedSize)
		{
			return (float)manualHeight / (float)height;
		}
		if (scalingStyle == Scaling.FixedSizeOnMobiles)
		{
			return (float)manualHeight / (float)height;
		}
		if (height < minimumHeight)
		{
			return (float)minimumHeight / (float)height;
		}
		if (height > maximumHeight)
		{
			return (float)maximumHeight / (float)height;
		}
		return 1f;
	}

	protected virtual void Awake()
	{
		mTrans = base.transform;
	}

	protected virtual void OnEnable()
	{
		list.Add(this);
	}

	protected virtual void OnDisable()
	{
		list.Remove(this);
	}

	protected virtual void Start()
	{
		UIOrthoCamera componentInChildren = GetComponentInChildren<UIOrthoCamera>();
		if (componentInChildren != null)
		{
			Camera component = componentInChildren.gameObject.GetComponent<Camera>();
			componentInChildren.enabled = false;
			if (component != null)
			{
				component.orthographicSize = 1f;
			}
		}
		else
		{
			Update();
		}
	}

	private void Update()
	{
		if (!(mTrans != null))
		{
			return;
		}
		float num = activeHeight;
		if (num > 0f)
		{
			float num2 = 2f / num;
			Vector3 localScale = mTrans.localScale;
			if (!(Mathf.Abs(localScale.x - num2) <= float.Epsilon) || !(Mathf.Abs(localScale.y - num2) <= float.Epsilon) || !(Mathf.Abs(localScale.z - num2) <= float.Epsilon))
			{
				mTrans.localScale = new Vector3(num2, num2, num2);
			}
		}
	}

	public static void Broadcast(string funcName)
	{
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			UIRoot uIRoot = list[i];
			if (uIRoot != null)
			{
				uIRoot.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static void Broadcast(string funcName, object param)
	{
		if (param == null)
		{
			return;
		}
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			UIRoot uIRoot = list[i];
			if (uIRoot != null)
			{
				uIRoot.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
