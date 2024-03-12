using System;

public class XXTEAUtils
{
	public static byte[] Encrypt(byte[] Data, byte[] Key)
	{
		if (Data.Length == 0)
		{
			return null;
		}
		int num = Data.Length >> 2;
		if (((uint)Data.Length & 3u) != 0)
		{
			num++;
		}
		byte[] array = new byte[num * 4];
		Array.Copy(Data, array, Data.Length);
		num++;
		uint[] array2 = new uint[num];
		array2[num - 1] = (uint)Data.Length;
		for (int i = 0; i < num - 1; i++)
		{
			array2[i] = BitConverter.ToUInt32(array, i * 4);
		}
		int num2 = Key.Length >> 2;
		if (((uint)Key.Length & 3u) != 0)
		{
			num2++;
		}
		byte[] array3 = new byte[num2 * 4];
		Array.Copy(Key, array3, Key.Length);
		uint[] array4 = new uint[num2];
		for (int j = 0; j < num2; j++)
		{
			array4[j] = BitConverter.ToUInt32(array3, j * 4);
		}
		InnerEncrypt(array2, array4);
		int num3 = num << 2;
		byte[] array5 = new byte[num3];
		for (int k = 0; k < num; k++)
		{
			byte[] bytes = BitConverter.GetBytes(array2[k]);
			Array.Copy(bytes, 0, array5, k * 4, 4);
		}
		return array5;
	}

	public static byte[] Decrypt(byte[] Data, byte[] Key)
	{
		if (Data.Length % 4 != 0)
		{
			return null;
		}
		int num = Data.Length >> 2;
		uint[] array = new uint[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = BitConverter.ToUInt32(Data, i * 4);
		}
		int num2 = Key.Length >> 2;
		if (((uint)Key.Length & 3u) != 0)
		{
			num2++;
		}
		byte[] array2 = new byte[num2 * 4];
		Array.Copy(Key, array2, Key.Length);
		uint[] array3 = new uint[num2];
		for (int j = 0; j < num2; j++)
		{
			array3[j] = BitConverter.ToUInt32(array2, j * 4);
		}
		InnerDecrypt(array, array3);
		uint num3 = array[num - 1];
		if (num3 > (num - 1) * 4)
		{
			return null;
		}
		if (num3 <= (num - 2) * 4)
		{
			return null;
		}
		byte[] array4 = new byte[num * 4];
		for (int k = 0; k < num; k++)
		{
			byte[] bytes = BitConverter.GetBytes(array[k]);
			Array.Copy(bytes, 0, array4, k * 4, 4);
		}
		byte[] array5 = new byte[num3];
		Array.Copy(array4, 0L, array5, 0L, num3);
		return array5;
	}

	private static uint[] InnerEncrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		if (k.Length < 4)
		{
			uint[] array = new uint[4];
			k.CopyTo(array, 0);
			k = array;
		}
		uint num2 = v[num];
		uint num3 = v[0];
		uint num4 = 2654435769u;
		uint num5 = 0u;
		int num6 = 6 + 52 / (num + 1);
		while (num6-- > 0)
		{
			num5 += num4;
			uint num7 = (num5 >> 2) & 3u;
			int i;
			for (i = 0; i < num; i++)
			{
				num3 = v[i + 1];
				num2 = (v[i] += (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num5 ^ num3) + (k[(i & 3) ^ num7] ^ num2)));
			}
			num3 = v[0];
			num2 = (v[num] += (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num5 ^ num3) + (k[(i & 3) ^ num7] ^ num2)));
		}
		return v;
	}

	private static uint[] InnerDecrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		if (k.Length < 4)
		{
			uint[] array = new uint[4];
			k.CopyTo(array, 0);
			k = array;
		}
		uint num2 = v[num];
		uint num3 = v[0];
		uint num4 = 2654435769u;
		int num5 = 6 + 52 / (num + 1);
		for (uint num6 = (uint)(num5 * num4); num6 != 0; num6 -= num4)
		{
			uint num7 = (num6 >> 2) & 3u;
			int num8;
			for (num8 = num; num8 > 0; num8--)
			{
				num2 = v[num8 - 1];
				num3 = (v[num8] -= (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num6 ^ num3) + (k[(num8 & 3) ^ num7] ^ num2)));
			}
			num2 = v[num];
			num3 = (v[0] -= (((num2 >> 5) ^ (num3 << 2)) + ((num3 >> 3) ^ (num2 << 4))) ^ ((num6 ^ num3) + (k[(num8 & 3) ^ num7] ^ num2)));
		}
		return v;
	}
}
