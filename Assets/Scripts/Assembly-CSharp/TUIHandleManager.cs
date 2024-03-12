using System.Collections.Generic;
using UnityEngine;

public class TUIHandleManager : MonoBehaviour
{
	private List<TUIInputHandle> m_handles = new List<TUIInputHandle>();

	public static bool s_anyUIButtonDown;

	private void Start()
	{
	}

	private void Update()
	{
		if (GameBattle.m_instance.GameState != GameBattle.State.Game)
		{
			return;
		}
		TUIInput[] input = TUIInputManager.GetInput();
		TUIInput[] array = input;
		for (int i = 0; i < array.Length; i++)
		{
			TUIInput input2 = array[i];
			if (s_anyUIButtonDown && input2.inputType != TUIInputType.Ended)
			{
				break;
			}
			for (int j = 0; j < m_handles.Count && !m_handles[j].HandleInput(input2); j++)
			{
			}
		}
	}

	public void AddHandle(TUIInputHandle handle)
	{
		m_handles.Add(handle);
	}
}
