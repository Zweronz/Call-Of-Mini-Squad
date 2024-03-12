using UnityEngine;

namespace CoMDS2
{
	public interface IPathFinding
	{
		bool HasNavigation();

		UnityEngine.AI.NavMeshAgent GetNavMeshAgent();

		void SetNavDesination(Vector3 target);

		void SetNavSpeed(float speed);

		void StopNav(bool stopUpdate = true);
	}
}
