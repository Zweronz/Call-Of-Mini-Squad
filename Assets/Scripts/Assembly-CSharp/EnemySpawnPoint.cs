using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
	public float triggerDistance = -1f;

	private void Awake()
	{
	}

	public virtual void Update()
	{
		if (GameBattle.s_enemyCount < 30)
		{
		}
	}

	protected virtual void SpawnEnemy()
	{
	}

	public virtual void KillEnemy(int wave = 0)
	{
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawWireSphere(base.transform.position, triggerDistance);
	}
}
