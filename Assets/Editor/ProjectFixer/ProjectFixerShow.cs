
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProjectFixer))]
public class ProjectFixerShow : Editor
{
	[MenuItem("Project Fixer/Fix CombinedMesh")]
	public static void FixCMRS()
	{
		ProjectFixer.Fix();
	}

	[MenuItem("Project Fixer/Sort Animations")]
	public static void SortAnims()
	{
		ProjectFixer.FixAnims();
	}

	[MenuItem("Project Fixer/Sort Meshes")]
	public static void SortMeshes()
	{
		ProjectFixer.FixMeshes();
	}
}
#endif