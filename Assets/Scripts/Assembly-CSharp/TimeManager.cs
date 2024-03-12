using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	private static TimeManager mInstance;

	private List<TimeCalculagraph> m_lsTimeCalculagraphs = new List<TimeCalculagraph>();

	public static TimeManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new GameObject("TimeManager").AddComponent(typeof(TimeManager)) as TimeManager;
			}
			return mInstance;
		}
	}

	public void Init(int id, float MaxTime, Calculagraph_CallBack CallBack_End, Calculagraph_CallBackUp CallBack_Up, string GOName = "")
	{
		string text = "TimeCalculagraph_";
		text = ((!(GOName != string.Empty)) ? (text + (m_lsTimeCalculagraphs.Count + 1)) : (text + GOName));
		TimeCalculagraph timeCalculagraph = new GameObject(text).AddComponent(typeof(TimeCalculagraph)) as TimeCalculagraph;
		timeCalculagraph.Init(id, MaxTime, CallBack_End, CallBack_Up);
		m_lsTimeCalculagraphs.Add(timeCalculagraph);
	}

	public bool CheckTimeCalculaIsOwn(int id)
	{
		foreach (TimeCalculagraph lsTimeCalculagraph in m_lsTimeCalculagraphs)
		{
			if (lsTimeCalculagraph.ID == id)
			{
				return true;
			}
		}
		return false;
	}

	public void DestroyCalculagraph(TimeCalculagraph tc)
	{
		if (m_lsTimeCalculagraphs.Contains(tc))
		{
			m_lsTimeCalculagraphs.Remove(tc);
			if (tc != null && tc.gameObject != null)
			{
				Object.Destroy(tc.gameObject);
			}
		}
	}

	public void DestroyCalculagraph(int id)
	{
		foreach (TimeCalculagraph lsTimeCalculagraph in m_lsTimeCalculagraphs)
		{
			if (lsTimeCalculagraph.ID == id)
			{
				DestroyCalculagraph(lsTimeCalculagraph);
				break;
			}
		}
	}

	public void DestroyAllCalculagraph(int id)
	{
		foreach (TimeCalculagraph lsTimeCalculagraph in m_lsTimeCalculagraphs)
		{
			if (lsTimeCalculagraph.ID == id)
			{
				DestroyCalculagraph(lsTimeCalculagraph);
			}
		}
	}
}
