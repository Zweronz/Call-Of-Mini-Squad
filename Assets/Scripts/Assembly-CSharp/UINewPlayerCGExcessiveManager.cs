using CoMDS2;
using UnityEngine;

public class UINewPlayerCGExcessiveManager : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		Util.PlayCG(true);
		SceneLoadingManager.SwitchScene("NEW MAP");
	}
}
