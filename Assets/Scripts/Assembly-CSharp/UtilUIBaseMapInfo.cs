using System.Collections.Generic;
using UnityEngine;

public class UtilUIBaseMapInfo : MonoBehaviour
{
	[SerializeField]
	private UIPanel mapAreaPanel;

	[SerializeField]
	private UISprite mapAreaDifficultImage;

	[SerializeField]
	private UIPanel mapWorldPanel;

	[SerializeField]
	private UITexture[] mapTexture;

	[SerializeField]
	private Material worldMapMaterial;

	[SerializeField]
	private Material[] areaMapMaterials;

	[SerializeField]
	private Transform[] mapEventsParentTran;

	[SerializeField]
	private GameObject stagePointPrefab;

	[SerializeField]
	private GameObject wavePointPrefab;

	[SerializeField]
	private GameObject customsDifficultyParentGO;

	[SerializeField]
	private UIToggle[] arrCustomsDifficulty = new UIToggle[3];

	[SerializeField]
	private ZoomInfoScript worldZoomInfoScript;

	[SerializeField]
	private ZoomInfoScript areaZoomInfoScript;

	[SerializeField]
	private ZoomInfoScript parentZoomInfoScript;

	[SerializeField]
	private GameObject[] arrDragGO;

	public GameObject[] lsStagePointsPerfab;

	public Dictionary<GameObject, UtilUIBaseStagePointInfoData> dictStagePoints = new Dictionary<GameObject, UtilUIBaseStagePointInfoData>();

	public Dictionary<GameObject, UtilUIBaseWavePointInfoData> dictWavePoints = new Dictionary<GameObject, UtilUIBaseWavePointInfoData>();

	public List<GameObject> lsCustomPoints = new List<GameObject>();

	private UtilUIBaseMapInfo_CustomsDifficultyToggleStateChange_Delegate customsDifficultyToggleStateChangeEvent;

	private UtilUIBaseMapInfo_CustomsDifficultyDisableBtnClick_Delegate customsDifficultyDisableBtnClickEvent;

	protected void SetCustomsDifficultyVisable(bool bShow)
	{
		customsDifficultyParentGO.SetActive(bShow);
	}

	public void HandleCustomsDifficultyToggle1StateChangedDelegate()
	{
		if (customsDifficultyToggleStateChangeEvent != null)
		{
			customsDifficultyToggleStateChangeEvent(0, arrCustomsDifficulty[0].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleCustomsDifficultyToggle2StateChangedDelegate()
	{
		if (customsDifficultyToggleStateChangeEvent != null)
		{
			customsDifficultyToggleStateChangeEvent(1, arrCustomsDifficulty[1].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleCustomsDifficultyToggle3StateChangedDelegate()
	{
		if (customsDifficultyToggleStateChangeEvent != null)
		{
			customsDifficultyToggleStateChangeEvent(2, arrCustomsDifficulty[2].value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleCustomsDifficultyDisableBtn1ClickDelegate()
	{
		if (customsDifficultyDisableBtnClickEvent != null)
		{
			customsDifficultyDisableBtnClickEvent(Defined.LevelMode.Normal);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleCustomsDifficultyDisableBtn2ClickDelegate()
	{
		if (customsDifficultyDisableBtnClickEvent != null)
		{
			customsDifficultyDisableBtnClickEvent(Defined.LevelMode.Hard);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleCustomsDifficultyDisableBtn3ClickDelegate()
	{
		if (customsDifficultyDisableBtnClickEvent != null)
		{
			customsDifficultyDisableBtnClickEvent(Defined.LevelMode.Hell);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void ClearStagePoints()
	{
		foreach (KeyValuePair<GameObject, UtilUIBaseStagePointInfoData> dictStagePoint in dictStagePoints)
		{
			Object.Destroy(dictStagePoint.Key);
		}
		dictStagePoints.Clear();
	}

	public GameObject CreateStagePoint(int id, bool bUnlock, Transform trans, Vector3 position, UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate dele, bool bClear, bool bCurrent, string icon, string name)
	{
		GameObject gameObject = Object.Instantiate(stagePointPrefab) as GameObject;
		UtilUIBaseStagePointInfoData component = gameObject.GetComponent<UtilUIBaseStagePointInfoData>();
		gameObject.transform.parent = mapEventsParentTran[0];
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.name = "Stage" + id;
		component.Init(id, bUnlock, trans, position, dele, bClear, bCurrent, icon, name);
		dictStagePoints.Add(gameObject, component);
		return gameObject;
	}

	public GameObject CreateStagePoint(int id, bool bUnlock, Vector3 position, UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate dele, bool bClear, bool bCurrent, string icon, string name)
	{
		return CreateStagePoint(id, bUnlock, null, position, dele, bClear, bCurrent, icon, name);
	}

	public void ClearWavePoints()
	{
		foreach (KeyValuePair<GameObject, UtilUIBaseWavePointInfoData> dictWavePoint in dictWavePoints)
		{
			Object.Destroy(dictWavePoint.Key);
		}
		dictWavePoints.Clear();
	}

	public GameObject CreateWavePoint(int id, bool bElite, int starsCount, Transform trans, Vector3 position, UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate dele, bool bDrift, bool bCurrent, int depth, string name)
	{
		GameObject gameObject = Object.Instantiate(wavePointPrefab) as GameObject;
		UtilUIBaseWavePointInfoData component = gameObject.GetComponent<UtilUIBaseWavePointInfoData>();
		gameObject.transform.parent = mapEventsParentTran[1];
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.name = "Wave" + id;
		component.Init(id, bElite, starsCount, trans, position, dele, bDrift, bCurrent, mapTexture[1].localSize, depth, name);
		dictWavePoints.Add(gameObject, component);
		return gameObject;
	}

	public GameObject CreateWavePoint(int id, bool bElite, int starsCount, Vector3 position, UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate dele, bool bDrift, bool bCurrent, int depth, string name)
	{
		return CreateWavePoint(id, bElite, starsCount, null, position, dele, bDrift, bCurrent, depth, name);
	}

	public void SetCustomPointsVisable(bool bShow)
	{
		foreach (GameObject lsCustomPoint in lsCustomPoints)
		{
			lsCustomPoint.SetActive(bShow);
		}
	}

	public void SetCustomPointVisable(int index, bool bShow)
	{
		lsCustomPoints[index].SetActive(bShow);
	}

	public void SetStagePointsVisable(bool bShow)
	{
		GameObject[] array = lsStagePointsPerfab;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(bShow);
		}
	}

	public void ShowWorldMap()
	{
		SetCustomsDifficultyVisable(false);
		ClearStagePoints();
		ClearWavePoints();
		SetCustomPointVisable(1, true);
		SolidMapCameraControl.mInstance.m_eMapType = SolidMapCameraControl.MapType.E_World;
	}

	public void ShowAreaMap(int worldIndex)
	{
		SetCustomsDifficultyVisable(true);
		ClearStagePoints();
		ClearWavePoints();
		SetCustomPointVisable(1, false);
		SolidMapCameraControl.mInstance.m_eMapType = (SolidMapCameraControl.MapType)worldIndex;
	}

	public void SetWorldPositionInCenter(Vector3 pos)
	{
	}

	public void MoveWorldPositionInCenter(Vector3 pos, GameObject targetGO, string functionName)
	{
	}

	public void SetCustomsDifficultyChecked(int index, bool bChecked)
	{
		arrCustomsDifficulty[index].value = bChecked;
	}

	public void SetCustomDifficultyTUnlockState(Defined.LevelMode levelMode)
	{
		for (int i = 0; i < arrCustomsDifficulty.Length; i++)
		{
			if (i <= (int)levelMode)
			{
				arrCustomsDifficulty[i].gameObject.GetComponent<UIImageButton>().isEnabled = true;
				arrCustomsDifficulty[i].gameObject.transform.Find("DisableClickMsg").gameObject.SetActive(false);
			}
			else
			{
				arrCustomsDifficulty[i].gameObject.GetComponent<UIImageButton>().isEnabled = false;
				arrCustomsDifficulty[i].gameObject.transform.Find("DisableClickMsg").gameObject.SetActive(true);
				arrCustomsDifficulty[i].GetComponentInChildren<UILabel>().color = UIUtil._UIGrayColor;
			}
		}
	}

	public void SetMapUIBackgroundByCustomsDifficulty(Defined.LevelMode levelMode)
	{
		switch (levelMode)
		{
		}
	}

	public void SetCustomsDifficultyTogglesStateChangedEvent(UtilUIBaseMapInfo_CustomsDifficultyToggleStateChange_Delegate dele)
	{
		customsDifficultyToggleStateChangeEvent = dele;
	}

	public void SetCustomsDifficultyDisableBtnClickEvent(UtilUIBaseMapInfo_CustomsDifficultyDisableBtnClick_Delegate dele)
	{
		customsDifficultyDisableBtnClickEvent = dele;
	}

	public void ZoomIn(bool bWorld, float durationTime)
	{
		parentZoomInfoScript.ZoomIn(durationTime);
	}

	public void ZoomOut(bool bWorld, float durationTime)
	{
		parentZoomInfoScript.ZoomOut(durationTime);
	}
}
