using System;
using System.Collections.Generic;
using UnityEngine;

public class UtilUICourseInfo : MonoBehaviour
{
	public enum CoursePhaseState
	{
		None = 0,
		InProgress = 1,
		Done = 2
	}

	[Serializable]
	public class CourseInfo
	{
		private int id = -1;

		public GameObject targetGO;

		public CoursePhaseState phaseState;

		private UtilUICourseInfo manager;

		private int targetOriginalPanelDepth = -999;

		private UIPanel targetPanel;

		private Vector3 cursorPos = Vector3.zero;

		private Transform cursorTrans;

		private Transform cursorTransII;

		private GameObject explainGO;

		private bool bNeedTweenPos;

		private Transform moveToUnderTargetTrans;

		public CoursePhaseState STATE
		{
			get
			{
				return phaseState;
			}
			set
			{
				phaseState = value;
				if (phaseState == CoursePhaseState.InProgress)
				{
					if (bNeedTweenPos)
					{
						manager.tweenPos.enabled = true;
						manager.tweenPos.ResetToBeginning();
						manager.tweenPos.from = cursorTrans;
						manager.tweenPos.to = cursorTransII;
					}
					else
					{
						manager.m_panelCursor.transform.position = cursorPos;
						if (moveToUnderTargetTrans != null)
						{
							manager.m_panelCursor.transform.parent = moveToUnderTargetTrans;
							manager.m_panelCursor.transform.localPosition = Vector3.zero;
							manager.m_panelCursor.transform.localEulerAngles = Vector3.zero;
						}
					}
					UIPanel[] componentsInChildren = targetPanel.transform.GetComponentsInChildren<UIPanel>();
					if (componentsInChildren.Length > 1)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							if (componentsInChildren[i] == targetPanel)
							{
								targetPanel.depth = manager.m_panelCover.depth + 10;
							}
							else
							{
								componentsInChildren[i].depth = manager.m_panelCover.depth + componentsInChildren[i].depth + 1;
							}
						}
					}
					else
					{
						targetPanel.depth = manager.m_panelCover.depth + 10;
					}
					if (targetOriginalPanelDepth == -999)
					{
						targetPanel.enabled = true;
					}
					manager.gameObject.SetActive(true);
					if (explainGO != null)
					{
						explainGO.SetActive(true);
					}
					targetGO.SetActive(!targetGO.activeInHierarchy);
					targetGO.SetActive(!targetGO.activeInHierarchy);
					manager.TutorialInProgress = true;
				}
				else
				{
					if (phaseState != CoursePhaseState.Done)
					{
						return;
					}
					if (bNeedTweenPos)
					{
						manager.tweenPos.enabled = false;
					}
					if (moveToUnderTargetTrans != null)
					{
						manager.m_panelCursor.transform.parent = manager.m_transCursorParent;
						manager.m_panelCursor.transform.localPosition = Vector3.zero;
						manager.m_panelCursor.transform.localEulerAngles = Vector3.zero;
					}
					UIPanel[] componentsInChildren2 = targetPanel.transform.GetComponentsInChildren<UIPanel>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						if (componentsInChildren2[j] != targetPanel)
						{
							componentsInChildren2[j].depth = componentsInChildren2[j].depth - manager.m_panelCover.depth - 1;
						}
					}
					targetPanel.depth = targetOriginalPanelDepth;
					if (targetOriginalPanelDepth == -999)
					{
						UnityEngine.Object.DestroyImmediate(targetPanel);
					}
					manager.gameObject.SetActive(false);
					if (explainGO != null)
					{
						explainGO.SetActive(false);
					}
					manager.TutorialInProgress = false;
				}
			}
		}

		public CourseInfo(int _id, GameObject targetGO, Transform cursorPos, Transform cursorPosII, UtilUICourseInfo manager)
		{
			id = _id;
			phaseState = CoursePhaseState.None;
			this.targetGO = targetGO;
			this.manager = manager;
			cursorTrans = cursorPos;
			cursorTransII = cursorPosII;
			targetPanel = this.targetGO.GetComponent<UIPanel>();
			phaseState = CoursePhaseState.None;
			if (targetPanel != null)
			{
				targetOriginalPanelDepth = targetPanel.depth;
			}
			else
			{
				targetPanel = this.targetGO.AddComponent<UIPanel>();
				targetPanel.enabled = false;
			}
			Transform transform = this.targetGO.transform.Find("ExplainGO");
			if (transform != null)
			{
				explainGO = transform.gameObject;
			}
			bNeedTweenPos = true;
		}

		public CourseInfo(int _id, GameObject targetGO, Vector3 cursorPos, UtilUICourseInfo manager)
		{
			id = _id;
			phaseState = CoursePhaseState.None;
			this.targetGO = targetGO;
			this.manager = manager;
			this.cursorPos = cursorPos;
			targetPanel = this.targetGO.GetComponent<UIPanel>();
			phaseState = CoursePhaseState.None;
			if (targetPanel != null)
			{
				targetOriginalPanelDepth = targetPanel.depth;
			}
			else
			{
				targetPanel = this.targetGO.AddComponent<UIPanel>();
				targetPanel.enabled = false;
			}
			Transform transform = this.targetGO.transform.Find("ExplainGO");
			if (transform != null)
			{
				explainGO = transform.gameObject;
			}
		}

		public void SetExplainGO(GameObject go)
		{
			explainGO = go;
		}

		public void SetMoveToUnderTargetTrans(Transform t)
		{
			moveToUnderTargetTrans = t;
		}
	}

	public UIPanel m_panelCursor;

	public UIPanel m_panelCover;

	public TweenTransform tweenPos;

	public Dictionary<int, CourseInfo> dictCourseInfo = new Dictionary<int, CourseInfo>();

	public bool courseInProgress;

	public Transform m_transCursorParent;

	public bool TutorialInProgress
	{
		get
		{
			return courseInProgress;
		}
		set
		{
			courseInProgress = value;
		}
	}

	private void Awake()
	{
	}

	public void AddCursor(int id, GameObject targetGO, Vector3 cursorPos)
	{
		if (dictCourseInfo.ContainsKey(id))
		{
			dictCourseInfo.Remove(id);
		}
		dictCourseInfo.Add(id, new CourseInfo(id, targetGO, cursorPos, this));
	}

	public void AddCursor(int id, GameObject targetGO, Transform cursorTrans, Transform cursorTransII)
	{
		if (dictCourseInfo.ContainsKey(id))
		{
			dictCourseInfo.Remove(id);
		}
		dictCourseInfo.Add(id, new CourseInfo(id, targetGO, cursorTrans, cursorTransII, this));
	}

	public void AddCursor(int id, GameObject targetGO, Transform cursorTrans, Transform cursorTransII, GameObject explainGO)
	{
		AddCursor(id, targetGO, cursorTrans, cursorTransII);
		GetCourse(id).SetExplainGO(explainGO);
	}

	public CourseInfo GetCourse(int id)
	{
		if (dictCourseInfo.ContainsKey(id))
		{
			return dictCourseInfo[id];
		}
		return null;
	}
}
