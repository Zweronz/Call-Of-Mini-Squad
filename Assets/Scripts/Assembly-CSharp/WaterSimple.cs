using UnityEngine;

[ExecuteInEditMode]
public class WaterSimple : MonoBehaviour
{
	private void Update()
	{
		if ((bool)base.GetComponent<Renderer>())
		{
			Material sharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
			if ((bool)sharedMaterial)
			{
				Vector4 vector = sharedMaterial.GetVector("WaveSpeed");
				float @float = sharedMaterial.GetFloat("_WaveScale");
				float num = Time.time / 20f;
				Vector4 vector2 = vector * (num * @float);
				Vector4 vector3 = new Vector4(Mathf.Repeat(vector2.x, 1f), Mathf.Repeat(vector2.y, 1f), Mathf.Repeat(vector2.z, 1f), Mathf.Repeat(vector2.w, 1f));
				sharedMaterial.SetVector("_WaveOffset", vector3);
				Vector3 vector4 = new Vector3(1f / @float, 1f / @float, 1f);
				Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(vector3.x, vector3.y, 0f), Quaternion.identity, vector4);
				sharedMaterial.SetMatrix("_WaveMatrix", matrix);
				matrix = Matrix4x4.TRS(new Vector3(vector3.z, vector3.w, 0f), Quaternion.identity, vector4 * 0.45f);
				sharedMaterial.SetMatrix("_WaveMatrix2", matrix);
			}
		}
	}
}
