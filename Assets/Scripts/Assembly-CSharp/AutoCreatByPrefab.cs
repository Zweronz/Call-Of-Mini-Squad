using System.Collections.Generic;
using UnityEngine;

public class AutoCreatByPrefab : MonoBehaviour
{
	public GameObject Prefab;

	public int MaxCount;

	public string GOEntryWord = string.Empty;

	public bool PlayOnAwake;

	public bool ResetCopyGOTrasform = true;

	private void Awake()
	{
		if (PlayOnAwake)
		{
			DoAutoCreate();
		}
	}

	private void Start()
	{
	}

	public List<GameObject> DoAutoCreate()
	{
		List<GameObject> list = new List<GameObject>();
		return AutoCreate(Prefab, MaxCount, GOEntryWord, ResetCopyGOTrasform);
	}

	public GameObject CreatePefab(int index)
	{
		return CreatePefab(index, Prefab, MaxCount, GOEntryWord, ResetCopyGOTrasform);
	}

	public GameObject CreatePefab(int index, GameObject prefab, int maxCount, string enterWord, bool bResetCopyGOTrasform)
	{
		GameObject gameObject = Object.Instantiate(prefab) as GameObject;
		if (maxCount >= 100000)
		{
			gameObject.name = enterWord + index.ToString("D0" + maxCount.ToString().Length);
		}
		else
		{
			gameObject.name = enterWord + index.ToString("D5");
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = Prefab.transform.localScale;
		if (bResetCopyGOTrasform)
		{
			gameObject.transform.localPosition = Vector3.zero;
		}
		return gameObject;
	}

	private List<GameObject> AutoCreate(GameObject prefab, int maxCount, string enterWord, bool bResetCopyGOTrasform)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < maxCount; i++)
		{
			GameObject item = CreatePefab(i, Prefab, maxCount, enterWord, bResetCopyGOTrasform);
			list.Add(item);
		}
		return list;
	}
}
