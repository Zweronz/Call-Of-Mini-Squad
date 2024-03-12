using CoMDS2;
using UnityEngine;

public class EnemySpawnPointByTime : EnemySpawnPoint
{
	public Enemy.EnemyType type;

	public float frequency = 1f;

	private float m_time;

	public int spawnOneTime = 1;

	private NumberSection<int> m_triggerDistance;

	public override void Update()
	{
		if (GameBattle.m_instance.GameState != GameBattle.State.Game || GameBattle.s_enemyCount >= 30)
		{
			return;
		}
		m_time += Time.deltaTime;
		if (m_time > frequency)
		{
			m_time = 0f;
			for (int i = 0; i < spawnOneTime; i++)
			{
				SpawnEnemy();
			}
		}
	}

	protected override void SpawnEnemy()
	{
		float min = 0f - triggerDistance;
		float max = triggerDistance;
		float min2 = 0f - triggerDistance;
		float max2 = triggerDistance;
		Transform transform = GameBattle.m_instance.GetPlayer().GetTransform();
		Vector3 position = new Vector3(transform.position.x + Random.Range(min, max), 0f, transform.position.z + Random.Range(min2, max2));
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
		Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
	}
}
