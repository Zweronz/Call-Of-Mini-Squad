using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Enhancement/CAIDTable")]
public class UICAIDTable : MonoBehaviour
{
	public enum Direction
	{
		Down = 0,
		Up = 1
	}

	public delegate void OnReposition();

	public int columns;

	public Direction direction;

	public Vector2 padding = Vector2.zero;

	public bool hideInactive = true;

	public bool repositionNow;

	public bool keepWithinPanel;

	public OnReposition onReposition;

	private UIPanel mPanel;

	private UIScrollView mDrag;

	private bool mStarted;

	private List<Transform> list = new List<Transform>();

	public List<Transform> children
	{
		get
		{
			return list;
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

	private void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = ((columns <= 0) ? 1 : (children.Count / columns + 1));
		int num4 = ((columns <= 0) ? children.Count : columns);
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		int j = 0;
		for (int count2 = children.Count; j < count2; j++)
		{
			Transform transform2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = transform2.localPosition;
			localPosition.x = num + bounds2.extents.x - bounds2.center.x;
			localPosition.x += bounds2.min.x - bounds3.min.x + padding.x;
			if (direction == Direction.Down)
			{
				localPosition.y = 0f - num2 - bounds2.extents.y - bounds2.center.y;
				localPosition.y += (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			else
			{
				localPosition.y = num2 + bounds2.extents.y - bounds2.center.y;
				localPosition.y += (bounds2.max.y - bounds2.min.y - bounds4.max.y + bounds4.min.y) * 0.5f - padding.y;
			}
			num += bounds3.max.x - bounds3.min.x + padding.x * 2f;
			transform2.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				num2 += bounds4.size.y + padding.y * 2f;
			}
		}
	}

	public void Reposition()
	{
		if (mStarted)
		{
			Transform target = base.transform;
			List<Transform> list = this.list;
			if (list.Count > 0)
			{
				RepositionVariableSize(list);
			}
			if (mDrag != null)
			{
				mDrag.UpdateScrollbars(true);
				mDrag.RestrictWithinBounds(true);
			}
			else if (mPanel != null)
			{
				mPanel.ConstrainTargetToBounds(target, true);
			}
			if (onReposition != null)
			{
				onReposition();
			}
		}
		else
		{
			repositionNow = true;
		}
	}

	private void Start()
	{
		mStarted = true;
		if (keepWithinPanel)
		{
			mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
			mDrag = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		}
		Reposition();
	}

	private void LateUpdate()
	{
		if (repositionNow)
		{
			repositionNow = false;
			Reposition();
		}
	}
}
