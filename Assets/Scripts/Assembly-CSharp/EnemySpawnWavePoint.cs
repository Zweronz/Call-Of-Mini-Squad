using CoMDS2;
using UnityEngine;

public class EnemySpawnWavePoint : EnemySpawnPoint
{
	public void SpawnEnemy(EnemySpawnPointMain.EnemyWaitSpawnInfo spawnInfo)
	{
		float min = 0f - triggerDistance;
		float max = triggerDistance;
		float min2 = 0f - triggerDistance;
		float max2 = triggerDistance;
		Vector3 position = new Vector3(base.gameObject.transform.position.x + Random.Range(min, max), 0f, base.gameObject.transform.position.z + Random.Range(min2, max2));
		Enemy enemy = null;
		enemy = BattleBufferManager.Instance.GetEnemyFromBuffer(spawnInfo.type);
		if (enemy != null)
		{
			if (spawnInfo.type == enemy.enemyType)
			{
				enemy.spawnInfo = spawnInfo.spawnInfo;
				enemy.GetTransform().position = position;
				enemy.GetModelTransform().rotation = Quaternion.identity;
			}
			enemy.isBoss = spawnInfo.isBoss;
			enemy.eliteType = spawnInfo.eliteType;
			enemy.specialAttribute.Clear();
			if (spawnInfo.specialAttribute != null && spawnInfo.specialAttribute.Count > 0)
			{
				enemy.specialAttribute.AddRange(spawnInfo.specialAttribute);
			}
			enemy.GetGameObject().SetActive(true);
			enemy.Reset();
			GameBattle.m_instance.AddObjToComputerList(enemy);
			GameBattle.s_enemyCount++;
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.8f, 0.5f, 0.8f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}

	protected override void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.8f, 0.5f, 0.8f, 0.8f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
		Gizmos.color = new Color(0.5f, 0.8f, 0.5f, 0.2f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}
}
