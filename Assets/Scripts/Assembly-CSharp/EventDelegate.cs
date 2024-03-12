using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventDelegate
{
	public delegate void Callback();

	[SerializeField]
	private MonoBehaviour mTarget;

	[SerializeField]
	private string mMethodName;

	public bool oneShot;

	private Callback mCachedCallback;

	private bool mRawDelegate;

	private static int s_Hash = "EventDelegate".GetHashCode();

	public MonoBehaviour target
	{
		get
		{
			return mTarget;
		}
		set
		{
			mTarget = value;
			mCachedCallback = null;
			mRawDelegate = false;
		}
	}

	public string methodName
	{
		get
		{
			return mMethodName;
		}
		set
		{
			mMethodName = value;
			mCachedCallback = null;
			mRawDelegate = false;
		}
	}

	public bool isValid
	{
		get
		{
			return (mRawDelegate && mCachedCallback != null) || (mTarget != null && !string.IsNullOrEmpty(mMethodName));
		}
	}

	public bool isEnabled
	{
		get
		{
			return (mRawDelegate && mCachedCallback != null) || (mTarget != null && mTarget.enabled);
		}
	}

	public EventDelegate()
	{
	}

	public EventDelegate(Callback call)
	{
		Set(call);
	}

	public EventDelegate(MonoBehaviour target, string methodName)
	{
		Set(target, methodName);
	}

	private static string GetMethodName(Callback callback)
	{
		return callback.Method.Name;
	}

	private static bool IsValid(Callback callback)
	{
		return callback != null && callback.Method != null;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return !isValid;
		}
		if (obj is Callback)
		{
			Callback callback = obj as Callback;
			if (callback.Equals(mCachedCallback))
			{
				return true;
			}
			MonoBehaviour monoBehaviour = callback.Target as MonoBehaviour;
			return mTarget == monoBehaviour && string.Equals(mMethodName, GetMethodName(callback));
		}
		if (obj is EventDelegate)
		{
			EventDelegate eventDelegate = obj as EventDelegate;
			return mTarget == eventDelegate.mTarget && string.Equals(mMethodName, eventDelegate.mMethodName);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_Hash;
	}

	private Callback Get()
	{
		if (!mRawDelegate && (mCachedCallback == null || mCachedCallback.Target as MonoBehaviour != mTarget || GetMethodName(mCachedCallback) != mMethodName))
		{
			if (!(mTarget != null) || string.IsNullOrEmpty(mMethodName))
			{
				return null;
			}
			mCachedCallback = (Callback)Delegate.CreateDelegate(typeof(Callback), mTarget, mMethodName);
		}
		return mCachedCallback;
	}

	private void Set(Callback call)
	{
		if (call == null || !IsValid(call))
		{
			mTarget = null;
			mMethodName = null;
			mCachedCallback = null;
			mRawDelegate = false;
			return;
		}
		mTarget = call.Target as MonoBehaviour;
		if (mTarget == null)
		{
			mRawDelegate = true;
			mCachedCallback = call;
			mMethodName = null;
		}
		else
		{
			mMethodName = GetMethodName(call);
			mRawDelegate = false;
		}
	}

	public void Set(MonoBehaviour target, string methodName)
	{
		mTarget = target;
		mMethodName = methodName;
		mCachedCallback = null;
		mRawDelegate = false;
	}

	public bool Execute()
	{
		Callback callback = Get();
		if (callback != null)
		{
			callback();
			return true;
		}
		return false;
	}

	public void Clear()
	{
		mTarget = null;
		mMethodName = null;
		mRawDelegate = false;
		mCachedCallback = null;
	}

	public override string ToString()
	{
		if (mTarget != null)
		{
			string text = mTarget.GetType().ToString();
			int num = text.LastIndexOf('.');
			if (num > 0)
			{
				text = text.Substring(num + 1);
			}
			if (!string.IsNullOrEmpty(methodName))
			{
				return text + "." + methodName;
			}
			return text + ".[delegate]";
		}
		return (!mRawDelegate) ? null : "[delegate]";
	}

	public static void Execute(List<EventDelegate> list)
	{
		if (list == null)
		{
			return;
		}
		int num = 0;
		while (num < list.Count)
		{
			EventDelegate eventDelegate = list[num];
			if (eventDelegate != null)
			{
				eventDelegate.Execute();
				if (num >= list.Count)
				{
					break;
				}
				if (list[num] != eventDelegate)
				{
					continue;
				}
				if (eventDelegate.oneShot)
				{
					list.RemoveAt(num);
					continue;
				}
			}
			num++;
		}
	}

	public static bool IsValid(List<EventDelegate> list)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.isValid)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void Set(List<EventDelegate> list, Callback callback)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(new EventDelegate(callback));
		}
	}

	public static void Add(List<EventDelegate> list, Callback callback)
	{
		Add(list, callback, false);
	}

	public static void Add(List<EventDelegate> list, Callback callback, bool oneShot)
	{
		if (list == null)
		{
			return;
		}
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			EventDelegate eventDelegate = list[i];
			if (eventDelegate != null && eventDelegate.Equals(callback))
			{
				return;
			}
		}
		EventDelegate eventDelegate2 = new EventDelegate(callback);
		eventDelegate2.oneShot = oneShot;
		list.Add(eventDelegate2);
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev)
	{
		Add(list, ev, false);
	}

	public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
	{
		if (list == null)
		{
			return;
		}
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			EventDelegate eventDelegate = list[i];
			if (eventDelegate != null && eventDelegate.Equals(ev))
			{
				return;
			}
		}
		EventDelegate eventDelegate2 = new EventDelegate(ev.target, ev.methodName);
		eventDelegate2.oneShot = oneShot;
		list.Add(eventDelegate2);
	}

	public static bool Remove(List<EventDelegate> list, Callback callback)
	{
		if (list != null)
		{
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					list.RemoveAt(i);
					return true;
				}
			}
		}
		return false;
	}
}
