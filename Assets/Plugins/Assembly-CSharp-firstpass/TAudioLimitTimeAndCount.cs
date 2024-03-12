using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AudioEffect/AudioLimit TimeAndCount")]
public class TAudioLimitTimeAndCount : ITAudioLimit
{
	public class CountLimit
	{
		private Queue<float> timeBound = new Queue<float>();

		private int m_max;

		public CountLimit(float t, int m)
		{
			timeBound.Enqueue(t);
			m_max = m;
		}

		public bool Add(float time)
		{
			while (timeBound.Count > 0 && timeBound.Peek() < Time.realtimeSinceStartup)
			{
				timeBound.Dequeue();
			}
			int count = timeBound.Count;
			if (count > m_max)
			{
				return false;
			}
			timeBound.Enqueue(time);
			return true;
		}

		public bool Check()
		{
			while (timeBound.Count > 0 && timeBound.Peek() < Time.realtimeSinceStartup)
			{
				timeBound.Dequeue();
			}
			int count = timeBound.Count;
			return count >= m_max;
		}
	}

	public int maxCount;

	public float deltaTime = 0.2f;

	private static Dictionary<string, CountLimit> s_records = new Dictionary<string, CountLimit>();

	public override bool isCanPlay
	{
		get
		{
			return !Limit(base.name);
		}
	}

	public override void OnAudioTrigger(AudioClip clip)
	{
		string key = "TimeAndCountLimit_" + base.name;
		if (s_records.ContainsKey(key))
		{
			if (s_records[key].Add(Time.realtimeSinceStartup + deltaTime))
			{
			}
		}
		else
		{
			CountLimit value = new CountLimit(Time.realtimeSinceStartup + deltaTime, maxCount);
			s_records.Add(key, value);
		}
	}

	public static bool Limit(string name)
	{
		string key = "TimeAndCountLimit_" + name;
		if (s_records.ContainsKey(key) && s_records[key].Check())
		{
			return true;
		}
		return false;
	}

	public static void ClearRecords()
	{
		s_records.Clear();
	}
}
