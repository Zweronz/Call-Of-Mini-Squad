using UnityEngine;

namespace CoMDS2
{
	public class Formula
	{
		public float a;

		public float b;

		public float c;

		public float d;

		public float e;

		public NumberSection<int> section;

		public Formula(float a = 0f, float b = 0f, float c = 0f)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public float A_x_X_plus_B(float x)
		{
			return a * x + b;
		}

		public float A_x_X_plus_S(float x)
		{
			int num = Random.Range(section.left, section.right);
			return a * x + (float)num;
		}

		public float A_plus_B_power_E_x_C()
		{
			return a + Mathf.Pow(b, e) * c;
		}

		public float A_x_squaX_plus_B(float x)
		{
			return a * (x * x) + b;
		}

		public float A_x_Xminus1_plus_B(float x)
		{
			return a * (x - 1f) + b;
		}
	}
}
