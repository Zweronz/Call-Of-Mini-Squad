using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("AudioEffect/AudioRule MaxTime")]
public class TAudioRuleMaxTime : ITAudioRule
{
	public enum Mode
	{
		Default = 0,
		Cutoff = 1,
		Upon = 2
	}

	public class Orbit
	{
		public GameObject evt;

		public float time = float.MaxValue;

		public int prior = int.MaxValue;
	}

	public GameObject[] evtRefs = new GameObject[0];

	public Mode mode;

	public int max = 1;

	public float judgeTime;

	public float delayTime;

	public bool usePrior;

	private Orbit[] m_orbits = new Orbit[0];

	private bool m_dirty;

	private Dictionary<string, int> m_prior = new Dictionary<string, int>();

	private int m_count;

	private void Awake()
	{
		switch (mode)
		{
		case Mode.Cutoff:
			max = 1;
			break;
		case Mode.Upon:
			max = 2;
			break;
		}
		m_orbits = new Orbit[max];
		for (int i = 0; i < max; i++)
		{
			m_orbits[i] = new Orbit();
		}
		judgeTime /= 1000f;
		delayTime /= 1000f;
	}

	private void Start()
	{
		RegistRule(this);
	}

	public void RegistRule(ITAudioRule regist)
	{
		GameObject[] array = evtRefs;
		foreach (GameObject gameObject in array)
		{
			TAudioRuleMaxTime component = gameObject.GetComponent<TAudioRuleMaxTime>();
			if ((bool)component)
			{
				component.RegistRule(regist);
				continue;
			}
			TAudioRuleMaxTime tAudioRuleMaxTime = regist as TAudioRuleMaxTime;
			if (tAudioRuleMaxTime.mode == Mode.Cutoff && tAudioRuleMaxTime.usePrior)
			{
				tAudioRuleMaxTime.AddPrior(gameObject.name);
			}
			TAudioManager.instance.RegistRule(gameObject.name, regist);
		}
	}

	public void AddPrior(string sample_name)
	{
		m_prior.Add(sample_name, m_count++);
	}

	public int QueryPrior(string sample_name)
	{
		return m_prior[sample_name];
	}

	private void Update()
	{
		switch (mode)
		{
		case Mode.Default:
			UpdateDefault();
			break;
		case Mode.Cutoff:
			UpdateCutoff();
			break;
		case Mode.Upon:
			UpdateUpon();
			break;
		}
	}

	public override bool Try(string sample_name, GameObject go, float over_time)
	{
		switch (mode)
		{
		case Mode.Default:
			return TryDefault(go, over_time);
		case Mode.Cutoff:
			return TryCutoff(sample_name, go, over_time);
		case Mode.Upon:
			return TryUpon(go, over_time);
		default:
			return true;
		}
	}

	public override void Stop(GameObject go)
	{
		switch (mode)
		{
		case Mode.Default:
			StopDefault(go);
			break;
		case Mode.Cutoff:
			StopCutoff(go);
			break;
		case Mode.Upon:
			StopUpon(go);
			break;
		}
	}

	private bool TryDefault(GameObject go, float over_time)
	{
		bool result = false;
		for (int num = m_orbits.Length - 1; num >= 0; num--)
		{
			Orbit orbit = m_orbits[num];
			if (!orbit.evt)
			{
				orbit.evt = go;
				orbit.time = over_time;
				m_dirty = true;
				result = true;
				break;
			}
		}
		return result;
	}

	private void StopDefault(GameObject go)
	{
		for (int i = 0; i < m_orbits.Length; i++)
		{
			Orbit orbit = m_orbits[i];
			if (go == orbit.evt)
			{
				orbit.evt = null;
				orbit.time = float.MaxValue;
				m_dirty = true;
				break;
			}
		}
	}

	private void UpdateDefault()
	{
		if (m_dirty)
		{
			Array.Sort(m_orbits, Compare);
			m_dirty = false;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int i = 0; i < m_orbits.Length; i++)
		{
			Orbit orbit = m_orbits[i];
			if (!(orbit.evt == null))
			{
				if (realtimeSinceStartup < orbit.time + delayTime)
				{
					break;
				}
				orbit.evt = null;
				orbit.time = float.MaxValue;
				m_dirty = true;
			}
		}
	}

	private static int Compare(Orbit l, Orbit r)
	{
		if (l.time < r.time)
		{
			return -1;
		}
		if (l.time > r.time)
		{
			return 1;
		}
		return 0;
	}

	private bool TryCutoff(string sample_name, GameObject go, float over_time)
	{
		Orbit orbit = m_orbits[0];
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int num = 0;
		if (usePrior)
		{
			num = QueryPrior(sample_name);
		}
		if ((bool)orbit.evt)
		{
			if (usePrior && orbit.prior < num && realtimeSinceStartup < orbit.time + delayTime)
			{
				return false;
			}
			if (!(realtimeSinceStartup > orbit.time - judgeTime))
			{
				return false;
			}
			ITAudioEvent component = orbit.evt.GetComponent<ITAudioEvent>();
			if ((bool)component)
			{
				component.Stop();
			}
		}
		orbit.evt = go;
		orbit.time = over_time;
		orbit.prior = num;
		return true;
	}

	private void StopCutoff(GameObject go)
	{
		Orbit orbit = m_orbits[0];
		if (orbit.evt == go)
		{
			orbit.evt = null;
			orbit.time = float.MaxValue;
		}
	}

	private void UpdateCutoff()
	{
	}

	private bool TryUpon(GameObject go, float over_time)
	{
		if (m_orbits[0].evt == null)
		{
			m_orbits[0].evt = go;
			m_orbits[0].time = over_time;
			return true;
		}
		if (m_orbits[1].evt == null)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup < m_orbits[0].time - judgeTime)
			{
				return false;
			}
			m_orbits[1].evt = go;
			m_orbits[1].time = over_time;
		}
		else if (m_orbits[1].evt == go)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			if (realtimeSinceStartup2 > m_orbits[0].time + delayTime)
			{
				m_orbits[0].evt = go;
				m_orbits[0].time = over_time;
				m_orbits[1].evt = null;
				m_orbits[1].time = float.MaxValue;
				return true;
			}
		}
		return false;
	}

	private void StopUpon(GameObject go)
	{
		if (m_orbits[0].evt == go)
		{
			m_orbits[0].evt = null;
			m_orbits[0].time = float.MaxValue;
			m_orbits[1].evt = null;
			m_orbits[1].time = float.MaxValue;
		}
	}

	private void UpdateUpon()
	{
		if (m_orbits[0].evt == null)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(realtimeSinceStartup > m_orbits[0].time))
		{
			return;
		}
		if (m_orbits[1].evt == null)
		{
			m_orbits[0].evt = null;
			m_orbits[0].time = float.MaxValue;
			return;
		}
		ITAudioEvent component = m_orbits[1].evt.GetComponent<ITAudioEvent>();
		if ((bool)component)
		{
			component.Stop();
			component.Trigger();
		}
	}
}
