using System.Collections.Generic;
using UnityEngine;

public class BaseMapPositionEdior : MonoBehaviour
{
	private string introMsg = "Key F1 is clean all the point\nKey F2 is create the State Point\nKey F3 is create the Wave Point\nKey F4 is create the Wave Boss Point\nKey F5 is refresh all the point if it can't move\n";

	private void Start()
	{
		DataCenter.Conf();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F1))
		{
			UINewBaseManager.Instance.UIBASEMAPINFO.SetStagePointsVisable(false);
			UINewBaseManager.Instance.UIBASEMAPINFO.ClearWavePoints();
			return;
		}
		if (Input.GetKeyUp(KeyCode.F2))
		{
			UINewBaseManager.Instance.UIBASEMAPINFO.CreateStagePoint(UINewBaseManager.Instance.UIBASEMAPINFO.dictStagePoints.Count + 1, true, Vector3.zero, null, false, false, string.Empty, string.Empty);
			return;
		}
		if (Input.GetKeyUp(KeyCode.F3))
		{
			UINewBaseManager.Instance.UIBASEMAPINFO.CreateWavePoint(UINewBaseManager.Instance.UIBASEMAPINFO.dictWavePoints.Count + 1, false, 1, Vector3.zero, null, false, false, -1, string.Empty);
			return;
		}
		if (Input.GetKeyUp(KeyCode.F4))
		{
			UINewBaseManager.Instance.UIBASEMAPINFO.CreateWavePoint(UINewBaseManager.Instance.UIBASEMAPINFO.dictWavePoints.Count + 1, true, 1, Vector3.zero, null, false, false, -1, string.Empty);
			return;
		}
		if (Input.GetKeyUp(KeyCode.F5))
		{
			foreach (KeyValuePair<GameObject, UtilUIBaseStagePointInfoData> dictStagePoint in UINewBaseManager.Instance.UIBASEMAPINFO.dictStagePoints)
			{
				UIDragObject component = dictStagePoint.Key.transform.Find("Button").GetComponent<UIDragObject>();
				if (component == null)
				{
					component = dictStagePoint.Key.transform.Find("Button").gameObject.AddComponent<UIDragObject>();
					component.target = dictStagePoint.Key.transform;
				}
			}
			{
				foreach (KeyValuePair<GameObject, UtilUIBaseWavePointInfoData> dictWavePoint in UINewBaseManager.Instance.UIBASEMAPINFO.dictWavePoints)
				{
					UIDragObject uIDragObject = null;
					if (dictWavePoint.Value.BELITE)
					{
						uIDragObject = dictWavePoint.Value.UI.m_Btn.GetComponent<UIDragObject>();
						if (uIDragObject == null)
						{
							uIDragObject = dictWavePoint.Value.UI.m_Btn.gameObject.AddComponent<UIDragObject>();
							uIDragObject.target = dictWavePoint.Key.transform.Find("Elite");
						}
					}
					else
					{
						uIDragObject = dictWavePoint.Value.UI.m_Btn.GetComponent<UIDragObject>();
						if (uIDragObject == null)
						{
							uIDragObject = dictWavePoint.Value.UI.m_Btn.gameObject.AddComponent<UIDragObject>();
							uIDragObject.target = dictWavePoint.Key.transform.Find("Normal");
						}
					}
				}
				return;
			}
		}
		if (!Input.GetKeyUp(KeyCode.F9) && !Input.GetKeyUp(KeyCode.F10))
		{
		}
	}

	private void OnGUI()
	{
		GUILayout.Label(introMsg);
		GUI.color = Color.red;
		GUILayout.Label("========Stages Info List========");
		GUI.color = Color.white;
		GUILayout.Space(3f);
		foreach (KeyValuePair<GameObject, UtilUIBaseStagePointInfoData> dictStagePoint in UINewBaseManager.Instance.UIBASEMAPINFO.dictStagePoints)
		{
			GUILayout.Label(string.Concat(dictStagePoint.Key.name, "'s position: ", dictStagePoint.Key.transform.position, ", local position: ", dictStagePoint.Key.transform.localPosition));
		}
		GUILayout.Space(10f);
		GUI.color = Color.red;
		GUILayout.Label("========Waves Info List========");
		GUI.color = Color.white;
		GUILayout.Space(3f);
		foreach (KeyValuePair<GameObject, UtilUIBaseWavePointInfoData> dictWavePoint in UINewBaseManager.Instance.UIBASEMAPINFO.dictWavePoints)
		{
			if (dictWavePoint.Value.BELITE)
			{
				GUILayout.Label(string.Concat(dictWavePoint.Key.name, "'s position: ", dictWavePoint.Key.transform.position, ", local position: ", dictWavePoint.Key.transform.Find("Elite").localPosition));
			}
			else
			{
				GUILayout.Label(string.Concat(dictWavePoint.Key.name, "'s position: ", dictWavePoint.Key.transform.position, ", local position: ", dictWavePoint.Key.transform.Find("Normal").localPosition));
			}
		}
	}
}
