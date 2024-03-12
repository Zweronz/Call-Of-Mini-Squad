using UnityEngine;

public class EffectNumManager : MonoBehaviour
{
	private GameObject m_effectNumDamage;

	private GameObject m_effectNumHeal;

	private GameObject m_effectNumCrit;

	private GameObject m_effectNumMiss;

	private GameObject m_effectNumIgnore;

	private GameObject m_effectNumShield;

	public static EffectNumManager instance;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void Start()
	{
		m_effectNumDamage = base.transform.Find("Damage").gameObject;
		EffectNumber effectNumber = m_effectNumDamage.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumDamage.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Damge;
		effectNumber.direction = EffectNumber.Direction.Up;
		m_effectNumDamage.SetActive(false);
		m_effectNumCrit = base.transform.Find("CritDamage").gameObject;
		effectNumber = m_effectNumCrit.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumCrit.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Crit;
		effectNumber.direction = EffectNumber.Direction.Up;
		m_effectNumCrit.SetActive(false);
		m_effectNumHeal = base.transform.Find("Heal").gameObject;
		effectNumber = m_effectNumHeal.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumHeal.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Heal;
		effectNumber.direction = EffectNumber.Direction.Up;
		m_effectNumHeal.SetActive(false);
		m_effectNumMiss = base.transform.Find("Miss").gameObject;
		effectNumber = m_effectNumMiss.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumMiss.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Miss;
		effectNumber.direction = EffectNumber.Direction.Left;
		m_effectNumMiss.SetActive(false);
		m_effectNumIgnore = base.transform.Find("Ignore").gameObject;
		effectNumber = m_effectNumIgnore.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumIgnore.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Ignore;
		effectNumber.direction = EffectNumber.Direction.Up;
		m_effectNumIgnore.SetActive(false);
		m_effectNumShield = base.transform.Find("Shield").gameObject;
		effectNumber = m_effectNumShield.GetComponent<EffectNumber>();
		if (effectNumber == null)
		{
			effectNumber = m_effectNumShield.AddComponent<EffectNumber>();
		}
		effectNumber.type = EffectNumber.EffectNumType.Shield;
		effectNumber.direction = EffectNumber.Direction.Up;
		m_effectNumShield.SetActive(false);
	}

	public void Update()
	{
	}

	public void GenerateEffectDamage(int num, Vector3 pos)
	{
		float num2 = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x + num2, pos.y, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumDamage) as GameObject;
		gameObject.SetActive(true);
		gameObject.name = m_effectNumDamage.name;
		gameObject.transform.parent = m_effectNumDamage.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		component.SetNum(num);
	}

	public void GenerateEffectHeal(int num, Vector3 pos)
	{
		float num2 = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x + num2, pos.y, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumHeal) as GameObject;
		gameObject.name = m_effectNumHeal.name;
		gameObject.transform.parent = m_effectNumHeal.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		component.SetNum(num);
		gameObject.SetActive(true);
	}

	public void GenerateEffectCrit(int num, Vector3 pos)
	{
		float num2 = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x + num2, pos.y, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumCrit) as GameObject;
		gameObject.name = m_effectNumCrit.name;
		gameObject.transform.parent = m_effectNumCrit.transform.parent;
		gameObject.transform.localScale = new Vector3(2f, 2f, 1f);
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		component.SetNum(num);
		gameObject.SetActive(true);
	}

	public void GenerateEffectMiss(Vector3 pos)
	{
		float num = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x, pos.y + num, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumMiss) as GameObject;
		gameObject.name = m_effectNumMiss.name;
		gameObject.transform.parent = m_effectNumMiss.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		component.direction = ((Random.Range(0, 100) % 2 != 0) ? EffectNumber.Direction.Right : EffectNumber.Direction.Left);
		gameObject.SetActive(true);
	}

	public void GenerateEffectIgnore(Vector3 pos)
	{
		float num = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x + num, pos.y, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumIgnore) as GameObject;
		gameObject.name = m_effectNumIgnore.name;
		gameObject.transform.parent = m_effectNumMiss.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		gameObject.SetActive(true);
	}

	public void GenerateEffectShield(int num, Vector3 pos)
	{
		float num2 = Random.Range(-1f, 1f);
		Vector3 vector = new Vector3(pos.x + num2, pos.y, pos.z);
		Vector3 vector2 = GameBattle.m_instance.GetCamera().WorldToScreenPoint(vector);
		vector2 = new Vector3(vector2.x - (float)(Screen.width / 2), vector2.y - (float)(Screen.height / 2), vector2.z);
		GameObject gameObject = Object.Instantiate(m_effectNumShield) as GameObject;
		gameObject.name = m_effectNumShield.name;
		gameObject.transform.parent = m_effectNumShield.transform.parent;
		gameObject.transform.localScale = Vector3.one;
		EffectNumber component = gameObject.GetComponent<EffectNumber>();
		component.worldPosition = vector;
		component.SetNum(num);
		gameObject.SetActive(true);
	}

	public void GenerageEffectNum(EffectNumber.EffectNumType type, float num, Vector3 pos)
	{
		switch (type)
		{
		case EffectNumber.EffectNumType.Damge:
			GenerateEffectDamage((int)num, pos);
			break;
		case EffectNumber.EffectNumType.Heal:
			GenerateEffectHeal((int)num, pos);
			break;
		case EffectNumber.EffectNumType.Crit:
			GenerateEffectCrit((int)num, pos);
			break;
		case EffectNumber.EffectNumType.Miss:
			GenerateEffectMiss(pos);
			break;
		case EffectNumber.EffectNumType.Ignore:
			GenerateEffectIgnore(pos);
			break;
		case EffectNumber.EffectNumType.Shield:
			GenerateEffectShield((int)num, pos);
			break;
		}
	}
}
