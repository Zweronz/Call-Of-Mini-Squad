using CoMDS2;
using UnityEngine;

public class EnemySpawnPointGeneral : EnemySpawnPoint
{
	public Enemy.EnemyType type;

	public float frequency = 1f;

	private float m_time;

	public int spawnOneTime = 1;

	private NumberSection<int> m_triggerDistance;

	private void Awake()
	{
		m_triggerDistance = new NumberSection<int>(400, 625);
	}

	public override void Update()
	{
		if (GameBattle.s_enemyCount >= GameBattle.s_enemyLimitCount)
		{
			return;
		}
		DS2ActiveObject nearestObjFromPlayerList = GameBattle.m_instance.GetNearestObjFromPlayerList(base.gameObject.transform.position, true);
		if (nearestObjFromPlayerList == null)
		{
			return;
		}
		float sqrMagnitude = (nearestObjFromPlayerList.GetTransform().position - base.gameObject.transform.position).sqrMagnitude;
		if (sqrMagnitude <= (float)m_triggerDistance.right && sqrMagnitude >= (float)m_triggerDistance.left && GameBattle.s_envetProcessingCount <= 0)
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
		float min = 1f;
		float max = 1f;
		float min2 = 1f;
		float max2 = 1f;
		if (triggerDistance != -1f)
		{
			min = 0f - triggerDistance;
			max = triggerDistance;
			min2 = 0f - triggerDistance;
			max2 = triggerDistance;
		}
		Vector3 position = new Vector3(base.gameObject.transform.position.x + Random.Range(min, max), 0f, base.gameObject.transform.position.z + Random.Range(min2, max2));
		Enemy enemy = null;
		enemy = BattleBufferManager.Instance.GetEnemyFromBuffer(type);
		if (enemy != null)
		{
			if (type == enemy.enemyType)
			{
				enemy.spawnInfo.spawnPoint = this;
				enemy.GetTransform().position = position;
				enemy.GetModelTransform().rotation = Quaternion.identity;
			}
			enemy.GetGameObject().SetActive(true);
			enemy.Reset();
			GameBattle.m_instance.AddObjToComputerList(enemy);
			GameBattle.s_enemyCount++;
		}
	}

	public override void KillEnemy(int wave = 0)
	{
	}

	protected override void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.8f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0.5f, 0.8f, 0.8f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
		Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}
}
