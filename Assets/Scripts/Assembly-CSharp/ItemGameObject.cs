using CoMDS2;
using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
	public int hp = 50;

	public Item.ItemType type;

	public float damage = 10f;

	private Item m_itemCallBack;

	private float m_explodeRange = 3f;

	private void Awake()
	{
		m_itemCallBack = new Item();
		Item itemCallBack = m_itemCallBack;
		int hpMax = hp;
		m_itemCallBack.hpMax = hpMax;
		itemCallBack.hp = hpMax;
		m_itemCallBack.clique = DS2ActiveObject.Clique.Neutral;
		m_itemCallBack.Initialize(base.gameObject);
		base.gameObject.layer = 19;
	}

	private void Start()
	{
		if (type == Item.ItemType.Jerrican)
		{
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_BOMB_1, 1, 1);
		}
	}

	private void Update()
	{
		switch (type)
		{
		case Item.ItemType.Jerrican:
			updateJerrican();
			break;
		case Item.ItemType.Box:
			updateBox1();
			break;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
	}

	private void updateJerrican()
	{
		if (m_itemCallBack.hp > 0)
		{
			return;
		}
		BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_BOMB_1, new Vector3(base.transform.position.x, 1f, base.transform.position.z), 2f);
		int layerMask = 2048;
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_explodeRange, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
			if (@object == null)
			{
			}
			HitInfo hitInfo = new HitInfo();
			hitInfo.damage = new NumberSection<float>(damage - damage * 0.25f, damage + damage * 0.25f);
			hitInfo.repelTime = 0.2f;
			hitInfo.repelDirection = @object.GetTransform().position - base.transform.position;
			hitInfo.repelDistance = new NumberSection<float>(3f, 5f);
			hitInfo.hitPoint = base.transform.position;
			hitInfo.source = m_itemCallBack;
			@object.OnHit(hitInfo);
		}
		m_itemCallBack.SetSplash(false);
		m_itemCallBack.Destroy(true);
	}

	private void updateBox1()
	{
		if (m_itemCallBack.hp <= 0)
		{
			m_itemCallBack.SetSplash(false);
			m_itemCallBack.Destroy(true);
		}
	}
}
