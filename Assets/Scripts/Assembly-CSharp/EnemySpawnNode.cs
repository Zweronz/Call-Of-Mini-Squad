using CoMDS2;
using UnityEngine;

public class EnemySpawnNode
{
	private float frequency = 1f;

	private float m_time;

	private NumberSection<int> m_triggerDistance = new NumberSection<int>(144, 625);

	public Enemy.EnemyType type { get; set; }

	public Vector3 position { get; set; }

	public void Update()
	{
		if (GameBattle.s_enemyCount >= GameBattle.s_enemyLimitCount)
		{
			return;
		}
		DS2ActiveObject nearestObjFromPlayerList = GameBattle.m_instance.GetNearestObjFromPlayerList(position, true);
		if (nearestObjFromPlayerList == null)
		{
			return;
		}
		float sqrMagnitude = (nearestObjFromPlayerList.GetTransform().position - position).sqrMagnitude;
		if (!(sqrMagnitude <= (float)m_triggerDistance.right) || !(sqrMagnitude >= (float)m_triggerDistance.left))
		{
			return;
		}
		if (GameBattle.s_envetProcessingCount > 0)
		{
			m_time += Time.deltaTime;
			if (!(m_time > frequency))
			{
				return;
			}
			m_time = 0f;
			Enemy enemy = null;
			if (GameBattle.s_enemyWaitToVisibleList.Count <= 0)
			{
				return;
			}
			EnemySpawnPointMain.EnemyWaitSpawnInfo enemyWaitSpawnInfo = GameBattle.s_enemyWaitToVisibleList[0];
			enemy = BattleBufferManager.Instance.GetEnemyFromBuffer(enemyWaitSpawnInfo.type);
			if (enemy != null)
			{
				GameBattle.s_enemyWaitToVisibleList.RemoveAt(0);
				if (enemyWaitSpawnInfo.type == enemy.enemyType)
				{
					enemy.spawnInfo = enemyWaitSpawnInfo.spawnInfo;
					enemy.GetTransform().position = position;
					enemy.GetModelTransform().rotation = Quaternion.identity;
				}
				enemy.isBoss = enemyWaitSpawnInfo.isBoss;
				enemy.eliteType = enemyWaitSpawnInfo.eliteType;
				enemy.specialAttribute.Clear();
				if (enemyWaitSpawnInfo.specialAttribute != null && enemyWaitSpawnInfo.specialAttribute.Count > 0)
				{
					enemy.specialAttribute.AddRange(enemyWaitSpawnInfo.specialAttribute);
				}
				enemy.GetGameObject().SetActive(true);
				enemy.Reset();
				GameBattle.m_instance.AddObjToComputerList(enemy);
				GameBattle.s_enemyCount++;
			}
		}
		else
		{
			m_time += Time.deltaTime;
			if (m_time > frequency)
			{
				m_time = 0f;
				SpawnEnemy();
			}
		}
	}

	protected void SpawnEnemy()
	{
		float min = position.x - 0.5f;
		float max = position.x + 0.5f;
		float min2 = position.z - 0.5f;
		float max2 = position.z + 0.5f;
		Vector3 vector = new Vector3(Random.Range(min, max), 0f, Random.Range(min2, max2));
		Enemy enemy = null;
		enemy = BattleBufferManager.Instance.GetEnemyFromBuffer(type);
		if (enemy != null)
		{
			if (type == enemy.enemyType)
			{
				enemy.spawnInfo = null;
				enemy.GetTransform().position = vector;
				enemy.GetModelTransform().rotation = Quaternion.identity;
			}
			enemy.GetGameObject().SetActive(true);
			enemy.Reset();
			GameBattle.m_instance.AddObjToComputerList(enemy);
			GameBattle.s_enemyCount++;
		}
	}

	public void TeleportObject()
	{
		if (GameBattle.s_objectWaitToTeleport.Count <= 0)
		{
			return;
		}
		DS2ActiveObject nearestObjFromPlayerList = GameBattle.m_instance.GetNearestObjFromPlayerList(position, true);
		if (nearestObjFromPlayerList == null)
		{
			return;
		}
		float sqrMagnitude = (nearestObjFromPlayerList.GetTransform().position - position).sqrMagnitude;
		if (sqrMagnitude >= 25f && sqrMagnitude <= 100f)
		{
			DS2ActiveObject dS2ActiveObject = GameBattle.s_objectWaitToTeleport[0];
			dS2ActiveObject.GetTransform().position = position;
			IPathFinding pathFinding = dS2ActiveObject.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.GetNavMeshAgent().Warp(position);
			}
			dS2ActiveObject.ChangeToDefaultAIState();
			GameBattle.s_objectWaitToTeleport.RemoveAt(0);
		}
	}
}
