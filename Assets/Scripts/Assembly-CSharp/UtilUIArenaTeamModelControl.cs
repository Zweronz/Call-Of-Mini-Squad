using System.Collections.Generic;
using UnityEngine;

public class UtilUIArenaTeamModelControl : MonoBehaviour
{
	public class ITEMINFO
	{
		public GameObject go;

		public TweenPosition tweenPos;

		public int siteIndexInTeam;

		public int nowSiteIndex;

		public int nextSiteIndex;

		public ITEMINFO(GameObject _go, int _siteIndexInTeam, int _nowSiteIndex, int _nextSiteIndex)
		{
			go = _go;
			siteIndexInTeam = _siteIndexInTeam;
			nowSiteIndex = _nowSiteIndex;
			nextSiteIndex = _nextSiteIndex;
			tweenPos = go.GetComponent<TweenPosition>();
		}
	}

	public UITexture gReferenceTex;

	public Camera gCamera;

	public GameObject goParent;

	protected List<ITEMINFO> lsGOS = new List<ITEMINFO>();

	protected Dictionary<GameObject, ITEMINFO> dictItems = new Dictionary<GameObject, ITEMINFO>();

	protected Vector3[] vc3BasePos;

	public bool bTurning;

	protected UtilUIArenaTeamModelControl_TurningFinished_Delegate truningFinishedDelegate;

	private Vector3 vcM = new Vector3(0f, 0f, 0f);

	private Vector3 vcL = new Vector3(-1f, 0f, 0.5f);

	private Vector3 vcR = new Vector3(1f, 0f, 0.5f);

	private Vector3[,] vc3DefaultePos = new Vector3[5, 5]
	{
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		},
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(1.5f, 0f, 0.5f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		},
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(1.5f, 0f, 0.5f),
			new Vector3(-1.5f, 0f, 0.5f),
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0f, 0f)
		},
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(1.5f, 0f, 0.5f),
			new Vector3(0f, 0f, 2f),
			new Vector3(-1.5f, 0f, 0.5f),
			new Vector3(0f, 0f, 0f)
		},
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(1.5f, 0f, 0.5f),
			new Vector3(0.7f, 0f, 2f),
			new Vector3(-0.7f, 0f, 2f),
			new Vector3(-1.5f, 0f, 0.5f)
		}
	};

	private void Awake()
	{
		if (null == gCamera)
		{
			gCamera = base.gameObject.GetComponent<Camera>();
		}
		InitPosAndSize();
		SetVisable(false);
	}

	public void SetVisable(bool bShow)
	{
		gCamera.enabled = bShow;
	}

	private void InitPosAndSize()
	{
		Transform transform = NGUITools.GetRoot(gReferenceTex.gameObject).transform;
		Vector3 vector = transform.InverseTransformPoint(gReferenceTex.transform.position);
		float left = ((float)Screen.width / 2f - Mathf.Abs(vector.x)) / (float)Screen.width;
		float top = ((float)Screen.height / 2f - Mathf.Abs(vector.y)) / transform.Find("Content Root").Find("Arena Content").GetComponent<UIPanel>()
			.height;
		float width = (float)gReferenceTex.width / (float)Screen.width;
		float height = (float)gReferenceTex.height / (float)Screen.height;
		gCamera.rect = new Rect(left, top, width, height);
	}

	public ITEMINFO InitTargets(GameObject[] gos)
	{
		lsGOS.Clear();
		dictItems.Clear();
		vc3BasePos = new Vector3[gos.Length];
		for (int i = 0; i < gos.Length; i++)
		{
			vc3BasePos[i] = vc3DefaultePos[gos.Length - 1, i];
			gos[i].name = string.Empty + i;
			gos[i].transform.parent = goParent.transform;
			gos[i].transform.localScale = new Vector3(1f, 1f, 1f);
			gos[i].transform.localPosition = vc3BasePos[i];
			TweenPosition tweenPosition = gos[i].AddComponent<TweenPosition>();
			tweenPosition.duration = 0.7f;
			tweenPosition.eventReceiver = base.gameObject;
			tweenPosition.callWhenFinished = "TurnFinished";
			tweenPosition.enabled = false;
			ITEMINFO iTEMINFO = new ITEMINFO(gos[i], i, i, i);
			lsGOS.Add(iTEMINFO);
			dictItems.Add(iTEMINFO.go, iTEMINFO);
		}
		return lsGOS[0];
	}

	public void Turning(bool bTurningLeft)
	{
		if (bTurning)
		{
			return;
		}
		for (int i = 0; i < lsGOS.Count; i++)
		{
			if (bTurningLeft)
			{
				TurnLeft(lsGOS[i]);
			}
			else
			{
				TurnRight(lsGOS[i]);
			}
			UpdateTurning(lsGOS[i]);
		}
	}

	public void SetTurningFinishedDelegate(UtilUIArenaTeamModelControl_TurningFinished_Delegate dele)
	{
		truningFinishedDelegate = dele;
	}

	protected void TurnLeft(ITEMINFO ii)
	{
		if (ii.nowSiteIndex > 0)
		{
			ii.nextSiteIndex = ii.nowSiteIndex - 1;
		}
		else
		{
			ii.nextSiteIndex = vc3BasePos.Length - 1;
		}
	}

	protected void TurnRight(ITEMINFO ii)
	{
		if (ii.nowSiteIndex >= vc3BasePos.Length - 1)
		{
			ii.nextSiteIndex = 0;
		}
		else
		{
			ii.nextSiteIndex = ii.nowSiteIndex + 1;
		}
	}

	protected void UpdateTurning(ITEMINFO ii)
	{
		if (ii.nextSiteIndex != ii.nowSiteIndex)
		{
			ii.tweenPos.from = vc3BasePos[ii.nowSiteIndex];
			ii.tweenPos.to = vc3BasePos[ii.nextSiteIndex];
			ii.tweenPos.ResetToBeginning();
			ii.tweenPos.PlayForward();
			bTurning = true;
		}
	}

	public void TurnFinished(UITweener t)
	{
		ITEMINFO iTEMINFO = dictItems[t.gameObject];
		if (truningFinishedDelegate != null)
		{
			truningFinishedDelegate(iTEMINFO);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		iTEMINFO.nowSiteIndex = iTEMINFO.nextSiteIndex;
		bTurning = false;
	}
}
