using CoMDS2;
using UnityEngine;

public class DS2UIBattle : MonoBehaviour
{
	private float left_wparam;

	private float left_lparam;

	private float right_wparam;

	private float right_lparam;

	public void HandleEventJoystickLeft(int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			left_wparam = 0f;
			left_lparam = 0f;
		}
		else
		{
			left_wparam = wparam;
			left_lparam = lparam;
		}
	}

	public void HandleEventJoystickRight(int eventType, float wparam, float lparam, object data)
	{
		if (eventType == 3)
		{
			right_wparam = 0f;
			right_lparam = 0f;
		}
		else
		{
			right_wparam = wparam;
			right_lparam = lparam;
		}
	}

	public void HandleEventMove(int eventType, float wparam, float lparam, object data)
	{
	}

	public void Start()
	{
	}

	public void Update()
	{
		if (GameBattle.s_bInputLocked || BattleUIEvent.s_anyButtonDown)
		{
			return;
		}
		if (!Application.isMobilePlatform)
		{
			//right_lparam = Mathf.Atan2(Input.GetAxisRaw("StickY"), Input.GetAxisRaw("StickX"));
		}
		if (DataCenter.Save().squadMode)
		{
			SquadController squadController = GameBattle.m_instance.GetSquadController();
			if (squadController != null)
			{
				squadController.UpdateInput(left_wparam, left_lparam, right_wparam, right_lparam);
			}
		}
		else
		{
			Player player2 = GameBattle.m_instance.GetPlayer();
			if (player2 != null)
			{
				player2.UpdateInput(left_wparam, left_lparam, right_wparam, right_lparam);
			}
		}
	}
}
