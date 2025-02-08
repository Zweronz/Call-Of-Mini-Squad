using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NavMeshTest : MonoBehaviour
{
	Vector3[] vertices;

	void OnDrawGizmos()
	{
		if (vertices == null)
		{
			using (BinaryReader reader = new BinaryReader(File.Open("Assets/verts.bin", FileMode.Open)))
			{
				vertices = new Vector3[reader.BaseStream.Length / 12];

				for (int i = 0; reader.BaseStream.Position < reader.BaseStream.Length; i++)
				{
					vertices[i].x = reader.ReadSingle();
					vertices[i].y = reader.ReadSingle();
					vertices[i].z = reader.ReadSingle();
				}
			}
		}

		foreach (Vector3 vertex in vertices)
		{
			Gizmos.DrawSphere(transform.rotation * (transform.position + vertex), 0.33f);
		}
	}
}
