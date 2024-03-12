using CoMDS2;
using UnityEngine;

public class EffectNumber : MonoBehaviour
{
	public enum EffectNumType
	{
		Damge = 0,
		Heal = 1,
		Crit = 2,
		Miss = 3,
		Ignore = 4,
		Shield = 5
	}

	public enum Direction
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	public EffectNumType type;

	private float speed = 3f;

	private float acceleration = 3f;

	private float m_offsetMax = 3f;

	private float m_oeffest;

	private float m_timer;

	private float m_time = 1f;

	public Direction direction;

	private UILabel label;

	private float m_deltaAlpha;

	private bool effectActive;

	private Vector3 originalScale;

	public Vector3 worldPosition;

	public void SetNum(int num)
	{
		if (label == null)
		{
			label = GetComponent<UILabel>();
		}
		label.text = string.Empty + num;
	}

	public void Start()
	{
		if (label == null)
		{
			label = GetComponent<UILabel>();
		}
		effectActive = false;
		switch (type)
		{
		case EffectNumType.Damge:
		case EffectNumType.Heal:
		case EffectNumType.Ignore:
		case EffectNumType.Shield:
			speed = 1f;
			m_time = 0.5f;
			m_timer = 0f;
			m_deltaAlpha = m_time;
			originalScale = base.transform.localScale;
			break;
		case EffectNumType.Crit:
			speed = 1f;
			m_time = 1f;
			m_timer = 0f;
			m_deltaAlpha = m_time;
			originalScale = base.transform.localScale;
			base.transform.localScale *= 3f;
			break;
		case EffectNumType.Miss:
			speed = 30f;
			m_time = 1f;
			m_timer = 0f;
			acceleration = 6f;
			m_deltaAlpha = m_time;
			break;
		}
	}

	public void Update()
	{
		switch (type)
		{
		case EffectNumType.Damge:
		case EffectNumType.Heal:
		case EffectNumType.Crit:
		case EffectNumType.Ignore:
		case EffectNumType.Shield:
			if (!effectActive)
			{
				if (type == EffectNumType.Crit)
				{
					float num3 = Mathf.Lerp(base.transform.localScale.x, originalScale.x, m_timer / 0.2f);
					base.transform.localScale = new Vector3(num3, num3, num3);
				}
				m_timer += Time.deltaTime;
				if (m_timer >= 0.3f)
				{
					m_timer = 0f;
					effectActive = true;
					if (type == EffectNumType.Crit)
					{
						base.transform.localScale = originalScale;
					}
				}
			}
			else if (m_timer < m_time)
			{
				m_timer += Time.deltaTime;
				float num4 = speed * Time.deltaTime;
				m_oeffest += num4;
				float num5 = label.color.a / m_deltaAlpha * Time.deltaTime;
				label.color = new Color(label.color.r, label.color.g, label.color.b, label.color.a - num5);
				worldPosition = new Vector3(worldPosition.x, worldPosition.y + num4, worldPosition.z);
				Vector3 p3 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(worldPosition);
				p3 = Util.ScreenPointToNGUI(p3);
				base.transform.localPosition = p3;
			}
			else
			{
				Object.DestroyImmediate(base.gameObject);
			}
			break;
		case EffectNumType.Miss:
			if (m_timer < m_time)
			{
				m_timer += Time.deltaTime;
				float num = speed * Time.deltaTime;
				speed -= speed * acceleration * Time.deltaTime;
				float num2 = label.color.a / m_deltaAlpha * Time.deltaTime;
				label.color = new Color(label.color.r, label.color.g, label.color.b, label.color.a - num2);
				if (direction == Direction.Left)
				{
					worldPosition = new Vector3(worldPosition.x - num, worldPosition.y, worldPosition.z);
					Vector3 p = GameBattle.m_instance.GetCamera().WorldToScreenPoint(worldPosition);
					p = Util.ScreenPointToNGUI(p);
					base.transform.localPosition = p;
				}
				else
				{
					worldPosition = new Vector3(worldPosition.x + num, worldPosition.y, worldPosition.z);
					Vector3 p2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(worldPosition);
					p2 = Util.ScreenPointToNGUI(p2);
					base.transform.localPosition = p2;
				}
			}
			else
			{
				Object.DestroyImmediate(base.gameObject);
			}
			break;
		}
	}
}
