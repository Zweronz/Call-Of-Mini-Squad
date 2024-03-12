using UnityEngine;

[ExecuteInEditMode]
public class GameScene : MonoBehaviour
{
	public const int CellCountX = 40;

	public const int CellCountY = 40;

	public const float CellSizeX = 1f;

	public const float CellSizeY = 1f;

	private static GameScene m_instance;

	public Vector3 basePosition;

	public static GameScene Instance()
	{
		return m_instance;
	}

	public void Awake()
	{
		m_instance = this;
		if (base.transform.position != new Vector3(0f, -0.0001f, 0f))
		{
			base.transform.position = new Vector3(0f, -0.0001f, 0f);
		}
	}

	public Vector3 GetCellPosition(int cellX, int cellY)
	{
		return basePosition + new Vector3((float)cellX * 1f, -0.29f, (float)cellY * 1f);
	}

	public int GetPositionCellX(Vector3 position)
	{
		return (int)Mathf.Floor((position.x - basePosition.x) / 1f);
	}

	public int GetPositionCellY(Vector3 position)
	{
		return (int)Mathf.Floor((position.z - basePosition.z) / 1f);
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		float z = 40f;
		for (int i = 0; i <= 40; i++)
		{
			Vector3 from = base.transform.position + basePosition + new Vector3((float)i * 1f, 0f, 0f);
			Vector3 to = base.transform.position + basePosition + new Vector3((float)i * 1f, 0f, z);
			Gizmos.DrawLine(from, to);
		}
		float x = 40f;
		for (int j = 0; j <= 40; j++)
		{
			Vector3 from2 = base.transform.position + basePosition + new Vector3(0f, 0f, (float)j * 1f);
			Vector3 to2 = base.transform.position + basePosition + new Vector3(x, 0f, (float)j * 1f);
			Gizmos.DrawLine(from2, to2);
		}
	}
}
