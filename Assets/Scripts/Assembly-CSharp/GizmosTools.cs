using UnityEngine;

public class GizmosTools : MonoBehaviour
{
	public enum DrawType
	{
		E_Selected = 0,
		E_Perpetual = 1,
		E_PerpetualAndSelected = 2
	}

	public DrawType enumDrawType = DrawType.E_Perpetual;

	public bool bCube;

	public bool bFrustum;

	public bool bLine;

	public bool bRay;

	public bool bSphere;

	public bool bWireCube;

	public bool bWireSphere;

	public bool bSectorRay;

	public Vector3 dataCenter = Vector3.zero;

	public Vector3 dataSize = Vector3.zero;

	public Vector3 dataDirection = Vector3.zero;

	public Vector3 dataTo = Vector3.zero;

	public float dataRadius;

	public float dataFov;

	public float dataMaxRange;

	public float dataMinRange;

	public float dataAspect;

	public float dataAngle;

	public Color color = Color.blue;

	public static GizmosTools GetGTools(GameObject go)
	{
		GizmosTools gizmosTools = go.GetComponent<GizmosTools>();
		if (!(gizmosTools != null))
		{
			gizmosTools = go.AddComponent<GizmosTools>();
		}
		return gizmosTools;
	}

	private void OnDrawGizmos()
	{
		if (enumDrawType != 0)
		{
			Gizmos.color = color;
			if (bCube)
			{
				DrawCube();
			}
			if (bFrustum)
			{
				DrawFrustum();
			}
			if (bLine)
			{
				DrawLine();
			}
			if (bRay)
			{
				DrawRay();
			}
			if (bSphere)
			{
				DrawSphere();
			}
			if (bWireCube)
			{
				DrawWireCube();
			}
			if (bWireSphere)
			{
				DrawWireSphere();
			}
			if (bSectorRay)
			{
				DrawSectorRay();
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (enumDrawType != DrawType.E_Perpetual)
		{
			Gizmos.color = color;
			if (bCube)
			{
				DrawCube();
			}
			if (bFrustum)
			{
				DrawFrustum();
			}
			if (bLine)
			{
				DrawLine();
			}
			if (bRay)
			{
				DrawRay();
			}
			if (bSphere)
			{
				DrawSphere();
			}
			if (bWireCube)
			{
				DrawWireCube();
			}
			if (bWireSphere)
			{
				DrawWireSphere();
			}
			if (bSectorRay)
			{
				DrawSectorRay();
			}
		}
	}

	private void DrawCube()
	{
		Gizmos.DrawCube(dataCenter, dataSize);
	}

	private void DrawFrustum()
	{
		Gizmos.DrawFrustum(dataCenter, dataFov, dataMaxRange, dataMinRange, dataAspect);
	}

	private void DrawLine()
	{
		Gizmos.DrawLine(dataCenter, dataTo);
	}

	private void DrawRay()
	{
		Gizmos.DrawRay(dataCenter, dataDirection);
	}

	private void DrawSphere()
	{
		Gizmos.DrawSphere(dataCenter, dataRadius);
	}

	private void DrawWireCube()
	{
		Gizmos.DrawWireCube(dataCenter, dataSize);
	}

	private void DrawWireSphere()
	{
		Gizmos.DrawWireSphere(dataCenter, dataRadius);
	}

	private void DrawSectorRay()
	{
		Gizmos.DrawRay(dataCenter, dataDirection * dataRadius);
		float num = Mathf.Cos(dataAngle) * dataRadius;
	}
}
