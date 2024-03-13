using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationHelper
{
	public static string TryGetAnimationName(Animation anim, string animation)
	{
		//thanks jeremy
		for (int i = -1; i < 11; i++)
		{
			if (anim[animation + (i == -1 ? "" : "_" + i)] != null)
			{
				return animation + (i == -1 ? "" : "_" + i);
			}
		}

		return animation;
	}
}
