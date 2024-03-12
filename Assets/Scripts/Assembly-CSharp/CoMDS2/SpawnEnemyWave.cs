using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	[Serializable]
	public class SpawnEnemyWave
	{
		[Serializable]
		public class SpawnInfo
		{
			public Enemy.EnemyType type;

			public int count;

			public Enemy.EnemyEliteType eliteType;

			public bool isBoss;

			public List<SpecialAttribute.AttributeType> specialAttribute;
		}

		public List<SpawnInfo> m_spawnInfos;

		public EnemySpawnWavePoint spawnPosition;

		public float waitSpawnTime;

		private int m_spawnCountTotal;

		private List<int> m_hasSpawnCountEachType;

		private int m_hasSpawnCount;

		private int m_kills;

		private int m_spawnType;

		private int waveNum;

		public SpawnEnemyWave()
		{
		}

		public SpawnEnemyWave(List<SpawnInfo> spawnInfos)
		{
			m_spawnInfos = spawnInfos;
		}

		public void Init(int waveNum)
		{
			this.waveNum = waveNum;
			m_hasSpawnCountEachType = new List<int>();
			foreach (SpawnInfo spawnInfo in m_spawnInfos)
			{
				m_spawnCountTotal += spawnInfo.count;
				m_hasSpawnCountEachType.Add(0);
				if (spawnInfo.specialAttribute == null || spawnInfo.specialAttribute.Count <= 0)
				{
					continue;
				}
				for (int i = 0; i < spawnInfo.specialAttribute.Count; i++)
				{
					if (spawnInfo.specialAttribute[i] == SpecialAttribute.AttributeType.Random)
					{
						int value = UnityEngine.Random.Range(1, 33);
						spawnInfo.specialAttribute[i] = (SpecialAttribute.AttributeType)value;
						BattleBufferManager.Instance.CreateSpecialAttributeEffectBufferByType(spawnInfo.specialAttribute[i]);
					}
				}
			}
		}

		public void SpawnEnemy(Vector3 position, Quaternion rotation, EnemySpawnPoint spawnPoint)
		{
			if (m_hasSpawnCount >= m_spawnCountTotal)
			{
				return;
			}
			Enemy.EnemyType type = getType();
			Enemy enemyFromBuffer = BattleBufferManager.Instance.GetEnemyFromBuffer(type);
			if (enemyFromBuffer != null)
			{
				m_hasSpawnCount++;
				List<int> hasSpawnCountEachType;
				List<int> list = (hasSpawnCountEachType = m_hasSpawnCountEachType);
				int spawnType;
				int index = (spawnType = m_spawnType);
				spawnType = hasSpawnCountEachType[spawnType];
				list[index] = spawnType + 1;
				EnemySpawnPointMain.EnemyWaitSpawnInfo enemyWaitSpawnInfo = default(EnemySpawnPointMain.EnemyWaitSpawnInfo);
				enemyWaitSpawnInfo.type = type;
				Enemy.SpawnInfo spawnInfo = new Enemy.SpawnInfo();
				spawnInfo.spawnPoint = spawnPoint;
				spawnInfo.spawnWave = waveNum;
				enemyWaitSpawnInfo.spawnInfo = spawnInfo;
				enemyWaitSpawnInfo.isBoss = m_spawnInfos[m_spawnType].isBoss;
				enemyWaitSpawnInfo.eliteType = m_spawnInfos[m_spawnType].eliteType;
				enemyWaitSpawnInfo.specialAttribute = new List<SpecialAttribute.AttributeType>();
				if (m_spawnInfos[m_spawnType].specialAttribute != null && m_spawnInfos[m_spawnType].specialAttribute.Count > 0)
				{
					enemyWaitSpawnInfo.specialAttribute.AddRange(m_spawnInfos[m_spawnType].specialAttribute);
				}
				if (spawnPosition != null)
				{
					spawnPosition.SpawnEnemy(enemyWaitSpawnInfo);
				}
				else
				{
					GameBattle.s_enemyWaitToVisibleList.Add(enemyWaitSpawnInfo);
				}
				if (m_hasSpawnCountEachType[m_spawnType] >= m_spawnInfos[m_spawnType].count)
				{
					m_spawnInfos.RemoveAt(m_spawnType);
					m_hasSpawnCountEachType.RemoveAt(m_spawnType);
				}
			}
		}

		public Enemy.EnemyType getType()
		{
			m_spawnType = UnityEngine.Random.Range(0, m_spawnInfos.Count);
			return m_spawnInfos[m_spawnType].type;
		}

		public void Kill()
		{
			m_kills++;
		}

		public bool WaveEnd()
		{
			return m_kills == m_spawnCountTotal;
		}

		public int WaveNum()
		{
			return waveNum;
		}
	}
}
