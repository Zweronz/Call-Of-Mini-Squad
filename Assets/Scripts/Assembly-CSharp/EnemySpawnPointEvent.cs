using CoMDS2;
using UnityEngine;

public class EnemySpawnPointEvent : EnemySpawnPoint
{
	public Enemy.EnemyType type;

	public float frequency = 1f;

	public int spawnOneTime = 1;

	public int count = 10;

	public bool stay;

	public Enemy.EnemyEliteType eliteType;

	public bool isBoss;

	private float m_time;

	private bool m_enable;

	private int m_hasSpawned;

	private void Awake()
	{
		spawnOneTime = Mathf.Max(1, spawnOneTime);
	}

	public void ExecuteEvent(string eventName)
	{
		switch (eventName)
		{
		case "Enter":
			if (m_hasSpawned >= count)
			{
				m_hasSpawned = 0;
			}
			m_enable = true;
			base.gameObject.SetActive(true);
			break;
		case "Exit":
			m_enable = false;
			break;
		case "Destroy":
			Object.Destroy(base.gameObject);
			break;
		}
	}

	public new void Update()
	{
		if (m_enable && GameBattle.s_enemyCount < 30 && m_hasSpawned < count)
		{
			m_time += Time.deltaTime;
			if (m_time > frequency)
			{
				m_time = 0f;
				SpawnEnemy();
			}
		}
	}

	protected override void SpawnEnemy()
	{
		for (int i = 0; i < spawnOneTime; i++)
		{
			float num = (0f - triggerDistance) / 2f;
			float num2 = triggerDistance / 2f;
			float num3 = (0f - triggerDistance) / 2f;
			float num4 = triggerDistance / 2f;
			Vector3 position = new Vector3(base.gameObject.transform.position.x + (float)Random.Range(-1, 1), 0f, base.gameObject.transform.position.z + (float)Random.Range(-1, 1));
			Enemy enemyFromBuffer = BattleBufferManager.Instance.GetEnemyFromBuffer(type);
			if (enemyFromBuffer != null)
			{
				if (type == enemyFromBuffer.enemyType)
				{
					enemyFromBuffer.spawnInfo.spawnPoint = this;
					enemyFromBuffer.GetTransform().position = position;
					enemyFromBuffer.GetModelTransform().rotation = Quaternion.identity;
				}
				if (stay)
				{
					enemyFromBuffer.StateToGuard();
				}
				enemyFromBuffer.isBoss = isBoss;
				enemyFromBuffer.eliteType = eliteType;
				enemyFromBuffer.GetGameObject().SetActive(true);
				enemyFromBuffer.Reset();
				GameBattle.m_instance.AddObjToComputerList(enemyFromBuffer);
				m_hasSpawned++;
				GameBattle.s_enemyCount++;
			}
		}
	}

	public override void KillEnemy(int wave = 0)
	{
	}

	protected override void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.8f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.5f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}
}
