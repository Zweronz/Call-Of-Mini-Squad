using System.Collections.Generic;
using UnityEngine;

public class UIEffectManager : MonoBehaviour
{
	public enum EffectType
	{
		E_Loading = 0,
		E_Team_LevelUp = 1,
		E_Team_Break = 2,
		E_Team_Unlock = 3
	}

	private static UIEffectManager mInstance;

	[SerializeField]
	private new Camera camera;

	[SerializeField]
	private List<GameObject> lsEffectGOS = new List<GameObject>();

	[SerializeField]
	private Dictionary<int, EffectType> ids = new Dictionary<int, EffectType>();

	private int usedMaxIDS = 50;

	public static UIEffectManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = GameObject.Find("Effect Dialog").GetComponent<UIEffectManager>();
			}
			return mInstance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < lsEffectGOS.Count; i++)
		{
			lsEffectGOS[i].SetActive(false);
		}
	}

	public void ShowEffect(EffectType type, int id)
	{
		if (!ids.ContainsKey(id))
		{
			lsEffectGOS[(int)type].SetActive(true);
			ids.Add(id, type);
			CheckCameraMode();
			UIPlaySound component = lsEffectGOS[(int)type].GetComponent<UIPlaySound>();
			if (component != null && !Application.loadedLevelName.StartsWith("UIEntry"))
			{
				component.Play();
			}
		}
		else
		{
			UIUtil.PDebug(string.Concat("Effect ", type, ",", id, "is already show!!!"), "1-4");
		}
	}

	public void ShowEffectParticle(EffectType type, Vector3 pos)
	{
		lsEffectGOS[(int)type].SetActive(true);
		lsEffectGOS[(int)type].transform.position = pos;
		lsEffectGOS[(int)type].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
	}

	public void ShowEffectParticle(Transform parentTrans, EffectType type)
	{
		Vector3 zero = Vector3.zero;
		zero = Instance.GetCameraObject().transform.InverseTransformPoint(parentTrans.position);
		ShowEffectParticle(type, zero);
	}

	public GameObject GetCameraObject()
	{
		return camera.gameObject;
	}

	public GameObject GetEffectObject(EffectType type)
	{
		return lsEffectGOS[(int)type];
	}

	public void HideEffect(EffectType type, int id)
	{
		if (ids.ContainsKey(id))
		{
			ids.Remove(id);
			bool flag = false;
			foreach (KeyValuePair<int, EffectType> id2 in ids)
			{
				if (id2.Value == type)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				lsEffectGOS[(int)type].SetActive(false);
			}
			CheckCameraMode();
		}
		else
		{
			UIUtil.PDebug(string.Concat("Effect ", type, "is already hide!!!"), "1-4");
		}
	}

	public void CheckCameraMode()
	{
	}

	public void ClearEffects()
	{
		foreach (KeyValuePair<int, EffectType> id in ids)
		{
		}
		foreach (GameObject lsEffectGO in lsEffectGOS)
		{
			lsEffectGO.SetActive(false);
		}
		ids.Clear();
		CheckCameraMode();
	}
}
