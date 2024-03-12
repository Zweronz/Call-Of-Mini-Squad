using System;
using System.Collections.Generic;
using UnityEngine;

public class SolidMapCameraControl : MonoBehaviour
{
	public enum MapType
	{
		E_Stage1 = 0,
		E_Stage2 = 1,
		E_Stage3 = 2,
		E_Stage4 = 3,
		E_World = 4
	}

	[Serializable]
	public class LevelPointsInfo
	{
		public Transform stagePartStageCameraTrans;

		public Transform wavePartStageCameraTrans;

		public Transform stageTrans;

		public List<Transform>[] waveTrans;
	}

	public static SolidMapCameraControl mInstance;

	public List<LevelPointsInfo> levelPointsInfo;

	public Camera mainCamera;

	public Transform pointsInfoParent;

	public Transform globalCameraDefualtTrans;

	public Transform globalCameraTrans;

	public List<Transform> lsStagePartStageCameraTrans;

	public List<Transform> lsWavePartStageCameraTrans;

	public Transform globalTutorialCameraTrans;

	private Vector2 horizontalMinMax = new Vector2(15f, 415f);

	private Vector2 verticalMinMax = new Vector2(417f, 1117f);

	private bool cameraToLeft = true;

	private bool cameraToDown = true;

	private bool cameraToRight = true;

	private bool cameraToUp = true;

	private Rect m_worldRect = new Rect(0f, 0f, 0f, 0f);

	private Rect[] m_stageRect = new Rect[4];

	private Rect m_exploreWorldRect = new Rect(0f, 0f, 0f, 0f);

	private Rect[] m_exploreStageRect = new Rect[4];

	public GameObject[] m_maxRect;

	public GameObject[] m_stage1Rect;

	public GameObject[] m_stage2Rect;

	public GameObject[] m_stage3Rect;

	public GameObject[] m_stage4Rect;

	public GameObject[] m_exploreMaxRect;

	public GameObject[] m_exploreStage1Rect;

	public GameObject[] m_exploreStage2Rect;

	public GameObject[] m_exploreStage3Rect;

	public GameObject[] m_exploreStage4Rect;

	public MapType m_eMapType = MapType.E_World;

	private void Awake()
	{
		mInstance = this;
	}

	private void Start()
	{
		levelPointsInfo = ResetInfo(pointsInfoParent);
		m_worldRect.xMin = m_maxRect[0].transform.position.x;
		m_worldRect.xMax = m_maxRect[1].transform.position.x;
		m_worldRect.yMin = m_maxRect[0].transform.position.y;
		m_worldRect.yMax = m_maxRect[1].transform.position.y;
		m_stageRect[0].xMin = m_stage1Rect[0].transform.position.x;
		m_stageRect[0].xMax = m_stage1Rect[1].transform.position.x;
		m_stageRect[0].yMin = m_stage1Rect[0].transform.position.y;
		m_stageRect[0].yMax = m_stage1Rect[1].transform.position.y;
		m_stageRect[1].xMin = m_stage2Rect[0].transform.position.x;
		m_stageRect[1].xMax = m_stage2Rect[1].transform.position.x;
		m_stageRect[1].yMin = m_stage2Rect[0].transform.position.y;
		m_stageRect[1].yMax = m_stage2Rect[1].transform.position.y;
		m_stageRect[2].xMin = m_stage3Rect[0].transform.position.x;
		m_stageRect[2].xMax = m_stage3Rect[1].transform.position.x;
		m_stageRect[2].yMin = m_stage3Rect[0].transform.position.y;
		m_stageRect[2].yMax = m_stage3Rect[1].transform.position.y;
		m_stageRect[3].xMin = m_stage4Rect[0].transform.position.x;
		m_stageRect[3].xMax = m_stage4Rect[1].transform.position.x;
		m_stageRect[3].yMin = m_stage4Rect[0].transform.position.y;
		m_stageRect[3].yMax = m_stage4Rect[1].transform.position.y;
		m_exploreWorldRect.xMin = m_exploreMaxRect[0].transform.position.x;
		m_exploreWorldRect.xMax = m_exploreMaxRect[1].transform.position.x;
		m_exploreWorldRect.yMin = m_exploreMaxRect[0].transform.position.y;
		m_exploreWorldRect.yMax = m_exploreMaxRect[1].transform.position.y;
		m_exploreStageRect[0].xMin = m_exploreStage1Rect[0].transform.position.x;
		m_exploreStageRect[0].xMax = m_exploreStage1Rect[1].transform.position.x;
		m_exploreStageRect[0].yMin = m_exploreStage1Rect[0].transform.position.y;
		m_exploreStageRect[0].yMax = m_exploreStage1Rect[1].transform.position.y;
		m_exploreStageRect[1].xMin = m_exploreStage2Rect[0].transform.position.x;
		m_exploreStageRect[1].xMax = m_exploreStage2Rect[1].transform.position.x;
		m_exploreStageRect[1].yMin = m_exploreStage2Rect[0].transform.position.y;
		m_exploreStageRect[1].yMax = m_exploreStage2Rect[1].transform.position.y;
		m_exploreStageRect[2].xMin = m_exploreStage3Rect[0].transform.position.x;
		m_exploreStageRect[2].xMax = m_exploreStage3Rect[1].transform.position.x;
		m_exploreStageRect[2].yMin = m_exploreStage3Rect[0].transform.position.y;
		m_exploreStageRect[2].yMax = m_exploreStage3Rect[1].transform.position.y;
		m_exploreStageRect[3].xMin = m_exploreStage4Rect[0].transform.position.x;
		m_exploreStageRect[3].xMax = m_exploreStage4Rect[1].transform.position.x;
		m_exploreStageRect[3].yMin = m_exploreStage4Rect[0].transform.position.y;
		m_exploreStageRect[3].yMax = m_exploreStage4Rect[1].transform.position.y;
	}

	private List<LevelPointsInfo> ResetInfo(Transform _poinsInfoParent)
	{
		List<LevelPointsInfo> list = new List<LevelPointsInfo>();
		for (int i = 0; i < _poinsInfoParent.childCount; i++)
		{
			LevelPointsInfo levelPointsInfo = new LevelPointsInfo();
			Transform child = pointsInfoParent.GetChild(i);
			levelPointsInfo.stagePartStageCameraTrans = lsStagePartStageCameraTrans[i];
			levelPointsInfo.wavePartStageCameraTrans = lsWavePartStageCameraTrans[i];
			levelPointsInfo.stageTrans = child;
			levelPointsInfo.waveTrans = new List<Transform>[child.childCount];
			for (int j = 0; j < child.childCount; j++)
			{
				Transform child2 = child.GetChild(j);
				List<Transform> list2 = new List<Transform>();
				for (int k = 0; k < child2.childCount; k++)
				{
					Transform child3 = child2.GetChild(k);
					list2.Add(child3);
				}
				levelPointsInfo.waveTrans[j] = list2;
			}
			list.Add(levelPointsInfo);
		}
		return list;
	}

	public void SetCameraTrans(Transform sameAsTrans)
	{
		globalCameraTrans.position = sameAsTrans.position;
		globalCameraTrans.eulerAngles = sameAsTrans.eulerAngles;
	}

	public void SetCameraPosition(Vector3 worldPos)
	{
		Vector3 vector = WorldToScreenViewPort(worldPos);
		Vector3 vector2 = new Vector3((float)Screen.width * vector.x, (float)Screen.height * vector.y, vector.z);
		globalCameraTrans.position = worldPos;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F1))
		{
			Start();
		}
	}

	public void LimitCheck(Rect maxRect)
	{
		cameraToLeft = true;
		cameraToDown = true;
		cameraToRight = true;
		cameraToUp = true;
		Rect rect = default(Rect);
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(0f, 0f, 0f));
		Debug.DrawRay(ray.origin, ray.direction * 5000f, Color.yellow);
		RaycastHit[] array = Physics.RaycastAll(ray, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.xMin = array[0].point.x;
			rect.yMax = array[0].point.y;
		}
		Ray ray2 = mainCamera.ScreenPointToRay(new Vector3(Screen.width, 0f, 0f));
		Debug.DrawRay(ray2.origin, ray2.direction * 5000f, Color.yellow);
		array = Physics.RaycastAll(ray2, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.xMax = array[0].point.x;
			rect.yMax = array[0].point.y;
		}
		Ray ray3 = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height, 0f));
		Debug.DrawRay(ray3.origin, ray3.direction * 5000f, Color.yellow);
		array = Physics.RaycastAll(ray3, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.yMin = array[0].point.y;
		}
		if (rect.xMin <= maxRect.xMin + 4f)
		{
			cameraToLeft = false;
		}
		if (rect.yMax >= maxRect.yMax - 2f)
		{
			cameraToDown = false;
		}
		if (rect.xMax >= maxRect.xMax - 4f)
		{
			cameraToRight = false;
		}
		if (rect.yMin <= maxRect.yMin + 2f)
		{
			cameraToUp = false;
		}
	}

	public Rect CameraViewBoundToRectDelta(Rect maxRect)
	{
		Rect result = default(Rect);
		Rect rect = default(Rect);
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(0f, 0f, 0f));
		Debug.DrawRay(ray.origin, ray.direction * 5000f, Color.yellow);
		RaycastHit[] array = Physics.RaycastAll(ray, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.xMin = array[0].point.x;
			rect.yMax = array[0].point.y;
		}
		Ray ray2 = mainCamera.ScreenPointToRay(new Vector3(Screen.width, 0f, 0f));
		Debug.DrawRay(ray2.origin, ray2.direction * 5000f, Color.yellow);
		array = Physics.RaycastAll(ray2, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.xMax = array[0].point.x;
			rect.yMax = array[0].point.y;
		}
		Ray ray3 = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height, 0f));
		Debug.DrawRay(ray3.origin, ray3.direction * 5000f, Color.yellow);
		array = Physics.RaycastAll(ray3, 5000f, 134217728);
		if (array.Length > 0)
		{
			rect.yMin = array[0].point.y;
		}
		result.xMin = maxRect.xMin + 4f - rect.xMin;
		result.yMax = rect.yMax - maxRect.yMax - 2f;
		result.xMax = rect.xMax - maxRect.xMax - 4f;
		result.yMin = maxRect.yMin + 2f - rect.yMin;
		return result;
	}

	public void MoveCamera(Vector3 delta)
	{
		if (mainCamera.transform.parent != globalCameraTrans)
		{
			SetCameraTrans(globalCameraTrans);
		}
		if (m_eMapType == MapType.E_Stage1)
		{
			LimitCheck(m_exploreStageRect[0]);
		}
		else if (m_eMapType == MapType.E_Stage2)
		{
			LimitCheck(m_exploreStageRect[1]);
		}
		else if (m_eMapType == MapType.E_Stage3)
		{
			LimitCheck(m_exploreStageRect[2]);
		}
		else if (m_eMapType == MapType.E_Stage4)
		{
			LimitCheck(m_exploreStageRect[3]);
		}
		else
		{
			LimitCheck(m_exploreWorldRect);
		}
		float num = 0f;
		float num2 = 0f;
		if (delta.x > 0f)
		{
			num = ((!cameraToLeft) ? 0f : delta.x);
		}
		else if (delta.x < 0f)
		{
			num = ((!cameraToRight) ? 0f : delta.x);
		}
		if (delta.y > 0f)
		{
			num2 = ((!cameraToDown) ? 0f : delta.y);
		}
		else if (delta.y < 0f)
		{
			num2 = ((!cameraToUp) ? 0f : delta.y);
		}
		if (m_eMapType == MapType.E_World)
		{
			num *= 0.3f;
			num2 *= 0.3f;
		}
		else
		{
			num *= 0.1f;
			num2 *= 0.1f;
		}
		float num3 = num / Time.deltaTime;
		if (num3 > 400f)
		{
			num = 400f * Time.deltaTime;
		}
		float num4 = num2 / Time.deltaTime;
		if (num4 > 400f)
		{
			num2 = 400f * Time.deltaTime;
		}
		Vector3 localPosition = globalCameraTrans.localPosition + new Vector3(0f - num, num2, delta.z);
		globalCameraTrans.localPosition = localPosition;
	}

	public bool ExploreCamera(GameObject eventTarget, string eventFunctionName)
	{
		bool flag = false;
		Rect rect = default(Rect);
		rect = ((m_eMapType == MapType.E_Stage1) ? CameraViewBoundToRectDelta(m_stageRect[0]) : ((m_eMapType == MapType.E_Stage2) ? CameraViewBoundToRectDelta(m_stageRect[1]) : ((m_eMapType == MapType.E_Stage3) ? CameraViewBoundToRectDelta(m_stageRect[2]) : ((m_eMapType != MapType.E_Stage4) ? CameraViewBoundToRectDelta(m_worldRect) : CameraViewBoundToRectDelta(m_stageRect[3])))));
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (rect.xMin > 0f)
		{
			num = rect.xMin;
		}
		if (rect.yMax > 0f)
		{
			num2 = 0f - rect.yMax;
		}
		if (rect.xMax > 0f)
		{
			num = 0f - rect.xMax;
		}
		if (rect.yMin > 0f)
		{
			num2 = rect.yMin;
		}
		Vector3 localTo = globalCameraTrans.localPosition + new Vector3(num, num2, num3);
		TweenCameraLocalPos(localTo, 0.3f, eventTarget, eventFunctionName, AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1f));
		if (num != 0f || num2 != 0f || num3 != 0f)
		{
			return true;
		}
		return false;
	}

	protected void TweenCameraWorldPos(Vector3 worldTo, float duration, GameObject eventTarget, string eventFunctionName)
	{
		Vector3 vector = globalCameraTrans.InverseTransformPoint(worldTo);
		TweenCameraLocalPos(globalCameraTrans.localPosition + vector, duration, eventTarget, eventFunctionName);
	}

	public void TweenCameraLocalPos(Vector3 localTo, float duration, GameObject eventTarget, string eventFunctionName, AnimationCurve ac)
	{
		TweenPosition component = globalCameraTrans.GetComponent<TweenPosition>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		component = globalCameraTrans.gameObject.AddComponent<TweenPosition>();
		component.from = globalCameraTrans.localPosition;
		component.to = localTo;
		component.duration = duration;
		component.eventReceiver = eventTarget;
		component.callWhenFinished = eventFunctionName;
		if (ac != null)
		{
			component.animationCurve = ac;
		}
	}

	public void TweenCameraLocalPos(Vector3 localTo, float duration, GameObject eventTarget, string eventFunctionName)
	{
		TweenCameraLocalPos(localTo, duration, eventTarget, eventFunctionName, null);
	}

	public Vector3 ScreenToWorldPosition(Vector3 pos)
	{
		Vector3 zero = Vector3.zero;
		return mainCamera.ScreenToWorldPoint(pos);
	}

	public Vector3 WorldToScreenPosition(Vector3 pos)
	{
		Vector3 zero = Vector3.zero;
		return mainCamera.WorldToScreenPoint(pos);
	}

	public Vector3 WorldToScreenViewPort(Vector3 pos)
	{
		Vector3 zero = Vector3.zero;
		return mainCamera.WorldToViewportPoint(pos);
	}
}
