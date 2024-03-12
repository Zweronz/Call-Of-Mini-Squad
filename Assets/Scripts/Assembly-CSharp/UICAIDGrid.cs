using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Enhancement/CAIDGrid")]
public class UICAIDGrid : MonoBehaviour
{
	public enum Arrangement
	{
		Horizontal = 0,
		Vertical = 1
	}

	public Arrangement arrangement;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool repositionNow;

	public bool hideInactive = true;

	private bool mStarted;

	private List<Transform> list = new List<Transform>();

	private void Start()
	{
		mStarted = true;
		Reposition();
	}

	private void Update()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}

	public void Add(Transform child, bool reposition = true)
	{
		Insert(list.Count, child);
	}

	public void AddRange(bool reposition = true, params Transform[] children)
	{
		InsertRange(list.Count, reposition, children);
	}

	public void Insert(int position, Transform child, bool reposition = true)
	{
		list.Insert(Mathf.Clamp(position, 0, list.Count), child);
		Vector3 localScale = child.localScale;
		Vector3 localPosition = child.localPosition;
		Quaternion localRotation = child.localRotation;
		child.parent = base.transform;
		child.localScale = localScale;
		child.localPosition = localPosition;
		child.localRotation = localRotation;
		if (reposition)
		{
			Reposition();
		}
	}

	public void InsertRange(int position, bool reposition = true, params Transform[] children)
	{
		if (children != null)
		{
			for (int num = children.Length - 1; num > -1; num--)
			{
				Insert(position, children[num], false);
			}
			if (reposition)
			{
				Reposition();
			}
		}
	}

	public void Remove(int position, bool deleteObj = true, bool reposition = true)
	{
		if (position >= 0 && position < list.Count)
		{
			GameObject obj = list[position].gameObject;
			list.RemoveAt(position);
			if (deleteObj)
			{
				Object.Destroy(obj);
			}
			if (reposition)
			{
				Reposition();
			}
		}
	}

	public void Remove(Transform child, bool deleteObj = true, bool reposition = true)
	{
		Remove(list.IndexOf(child), deleteObj, reposition);
	}

	public void Clear(bool deleteObj = true, bool reposition = true)
	{
		if (deleteObj)
		{
			foreach (Transform item in list)
			{
				Object.Destroy(item.gameObject);
			}
		}
		list.Clear();
		if (reposition)
		{
			Reposition();
		}
	}

	public void Reposition()
	{
		if (!mStarted)
		{
			repositionNow = true;
			return;
		}
		int num = 0;
		int num2 = 0;
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			Transform transform = list[i];
			if (NGUITools.GetActive(transform.gameObject) || !hideInactive)
			{
				float z = transform.localPosition.z;
				transform.localPosition = ((arrangement != 0) ? new Vector3(cellWidth * (float)num2, (0f - cellHeight) * (float)num, z) : new Vector3(cellWidth * (float)num, (0f - cellHeight) * (float)num2, z));
				if (++num >= maxPerLine && maxPerLine > 0)
				{
					num = 0;
					num2++;
				}
			}
		}
		UIScrollView uIScrollView = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		if (uIScrollView != null)
		{
			uIScrollView.UpdateScrollbars(true);
		}
	}
}
