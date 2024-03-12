using System.Collections.Generic;
using CoMDS2;
using UnityEngine;

public class Minimap : MonoBehaviour
{
	private List<GameObject> m_rooms;

	private Rect m_sceneSize;

	private Rect m_viewSize;

	private Vector2 m_viewPos;

	private Vector2 m_playerPos;

	private bool m_bOverall = true;

	private DS2ActiveObject[] m_allyMarks;

	private GameObject m_targetMark;

	private float m_scale = 1f;

	private Rect m_oringinalView;

	private Texture m_texturePack;

	private void Start()
	{
		m_texturePack = (Texture)Resources.Load("Minimap/minimapTex", typeof(Texture));
		m_rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("NodeGroup"));
		GameObject[] array = GameObject.FindGameObjectsWithTag("NodeGroup");
		if (array != null)
		{
			m_rooms.AddRange(array);
		}
		m_scale = 1f * (float)Mathf.Max(1, Screen.width / 320);
		float num = 9999.9f;
		float num2 = -9999.9f;
		float num3 = 9999.9f;
		float num4 = -9999.9f;
		foreach (GameObject room in m_rooms)
		{
			Renderer component = room.GetComponent<Renderer>();
			Bounds bounds = component.bounds;
			if (num > bounds.min.x)
			{
				num = bounds.min.x;
			}
			if (num2 < bounds.max.x)
			{
				num2 = bounds.max.x;
			}
			if (num3 > bounds.min.z)
			{
				num3 = bounds.min.z;
			}
			if (num4 < bounds.max.z)
			{
				num4 = bounds.max.z;
			}
		}
		m_oringinalView = new Rect(num, num3, Mathf.Abs(num) + Mathf.Abs(num2), Mathf.Abs(num3) + Mathf.Abs(num4));
		float left = m_oringinalView.xMin * m_scale;
		float top = m_oringinalView.yMin * m_scale;
		float width = m_oringinalView.width * m_scale;
		float height = m_oringinalView.height * m_scale;
		m_sceneSize = new Rect(left, top, width, height);
		m_viewSize = new Rect(Screen.width - 160, 20f, 120f, 120f);
		m_viewPos = default(Vector2);
		m_targetMark = GameObject.FindGameObjectWithTag("Target");
		if (m_bOverall)
		{
			StateToOverall();
		}
		else
		{
			StateToScroll();
		}
	}

	private void Update()
	{
		m_playerPos = new Vector2(GameBattle.m_instance.GetPlayer().GetTransform().position.x, GameBattle.m_instance.GetPlayer().GetTransform().position.z);
		m_allyMarks = GameBattle.m_instance.GetTeammateList();
		if (!m_bOverall)
		{
			float num = m_viewSize.width * 0.5f + m_sceneSize.x;
			float num2 = m_sceneSize.height - (m_viewSize.height * 0.5f - m_sceneSize.y);
			m_viewPos.x = num - m_playerPos.x * m_scale;
			m_viewPos.y = m_playerPos.y * m_scale - num2;
			if (m_viewPos.x > 0f)
			{
				m_viewPos.x = 0f;
			}
			else if (m_viewPos.x < m_viewSize.width - m_sceneSize.width)
			{
				m_viewPos.x = m_viewSize.width - m_sceneSize.width;
			}
			if (m_viewPos.y > 0f)
			{
				m_viewPos.y = 0f;
			}
			else if (m_viewPos.y < m_viewSize.height - m_sceneSize.height)
			{
				m_viewPos.y = m_viewSize.height - m_sceneSize.height;
			}
		}
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = new Color(1f, 1f, 0f, 0.8f);
		gUIStyle.alignment = TextAnchor.UpperCenter;
		if (m_bOverall)
		{
			GUI.BeginGroup(new Rect((float)Screen.width - m_sceneSize.width - 40f, 20f, m_sceneSize.width, m_sceneSize.height), string.Empty, gUIStyle);
		}
		else
		{
			GUI.BeginGroup(m_viewSize, string.Empty, gUIStyle);
		}
		DrawTexture(new Rect(0f, 0f, m_viewSize.width, m_viewSize.height), new Rect(2f, 2f, 43f, 43f), ref m_texturePack, new Color(0f, 0f, 0f, 0.2f));
		Rect position = new Rect(m_viewPos.x, m_viewPos.y, m_sceneSize.width, m_sceneSize.height);
		GUI.BeginGroup(position);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		foreach (GameObject room in m_rooms)
		{
			Renderer component = room.GetComponent<Renderer>();
			Bounds bounds = component.bounds;
			num = bounds.min.x * m_scale;
			num2 = bounds.min.z * m_scale;
			num3 = bounds.size.x * m_scale;
			num4 = bounds.size.z * m_scale;
			Rect destinationPixel = new Rect(num - m_sceneSize.x, m_sceneSize.height - (num2 - m_sceneSize.y) - num4, num3, num4);
			DrawTexture(destinationPixel, new Rect(48f, 3f, 6f, 6f), ref m_texturePack, new Color(1f, 1f, 1f, 0.05f));
		}
		num = m_playerPos.x * m_scale;
		num2 = m_playerPos.y * m_scale;
		num3 = 1.5f * m_scale;
		num4 = 1.5f * m_scale;
		Rect destinationPixel2 = new Rect(num - m_sceneSize.x - num3 * 0.5f, m_sceneSize.height - (num2 - m_sceneSize.y) - num4 * 0.5f, num3, num4);
		DrawTexture(destinationPixel2, new Rect(47f, 2f, 8f, 8f), ref m_texturePack, new Color(1f, 0f, 0f, 0.3f));
		if (m_allyMarks != null && m_allyMarks.Length > 0)
		{
			for (int num5 = m_allyMarks.Length - 1; num5 >= 0; num5--)
			{
				Vector2 vector = new Vector2(m_allyMarks[num5].GetTransform().position.x, m_allyMarks[num5].GetTransform().position.z);
				num = vector.x * m_scale;
				num2 = vector.y * m_scale;
				num3 = 1.5f * m_scale;
				num4 = 1.5f * m_scale;
				Rect destinationPixel3 = new Rect(num - m_sceneSize.x - num3 * 0.5f, m_sceneSize.height - (num2 - m_sceneSize.y) - num4 * 0.5f, num3, num4);
				DrawTexture(destinationPixel3, new Rect(47f, 2f, 8f, 8f), ref m_texturePack, new Color(1f, 0f, 1f, 0.3f));
			}
		}
		if (m_targetMark != null)
		{
			Vector2 vector2 = new Vector2(m_targetMark.transform.position.x, m_targetMark.transform.position.z);
			num = vector2.x * m_scale;
			num2 = vector2.y * m_scale;
			num3 = 1.5f * m_scale;
			num4 = 1.5f * m_scale;
			Rect destinationPixel4 = new Rect(num - m_sceneSize.x - num3 * 0.5f, m_sceneSize.height - (num2 - m_sceneSize.y) - num4 * 0.5f, num3, num4);
			DrawTexture(destinationPixel4, new Rect(47f, 2f, 8f, 8f), ref m_texturePack, new Color(0f, 1f, 0f, 0.3f));
		}
		GUI.EndGroup();
		GUI.EndGroup();
		GUI.color = new Color(1f, 1f, 0f, 0.3f);
		if (!m_bOverall)
		{
			if (GUI.Button(new Rect(m_viewSize.xMax, m_viewSize.y, 40f, 40f), "+"))
			{
				StateToOverall();
			}
		}
		else if (GUI.Button(new Rect(m_viewSize.xMax, m_viewSize.y, 40f, 40f), "-"))
		{
			StateToScroll();
		}
	}

	private void StateToOverall()
	{
		m_bOverall = true;
		m_viewPos.x = 0f;
		m_viewPos.y = 0f;
		float left = m_oringinalView.xMin * m_scale;
		float top = m_oringinalView.yMin * m_scale;
		float width = m_oringinalView.width * m_scale;
		float height = m_oringinalView.height * m_scale;
		m_sceneSize = new Rect(left, top, width, height);
		m_viewSize = new Rect((float)Screen.width - m_sceneSize.width - 40f, 20f, m_sceneSize.width, m_sceneSize.height);
	}

	private void StateToScroll()
	{
		m_bOverall = false;
		m_viewSize = new Rect(Screen.width - 160, 20f, 120f, 120f);
	}

	private void DrawTexture(Rect destinationPixel, Rect sourcePixel, ref Texture tex, Color c)
	{
		int width = tex.width;
		int height = tex.height;
		sourcePixel.x /= width;
		sourcePixel.y = 1f - (sourcePixel.y + sourcePixel.height) / (float)height;
		sourcePixel.width /= width;
		sourcePixel.height /= height;
		GUI.color = c;
		GUI.DrawTextureWithTexCoords(destinationPixel, tex, sourcePixel, false);
		GUI.color = Color.white;
	}
}
