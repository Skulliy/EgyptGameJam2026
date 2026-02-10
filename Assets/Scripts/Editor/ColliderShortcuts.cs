using UnityEngine;
using UnityEditor;

public class ColliderShortcuts : EditorWindow
{
	// [MenuItem] allows us to create a shortcut. 
	// The " _m" at the end tells Unity to use the 'M' key as the shortcut.
	[MenuItem("Tools/Colliders/Add Mesh Collider _m")]
	static void AddMeshColliderToSelected()
	{
		GameObject selected = Selection.activeGameObject;

		if (selected == null) return;

		// Check if it has a MeshFilter as you requested
		MeshFilter filter = selected.GetComponent<MeshFilter>();

		if (filter != null)
		{
			// Use Undo.AddComponent so you can press Ctrl+Z to remove it if you make a mistake
			MeshCollider meshCol = Undo.AddComponent<MeshCollider>(selected);

			// Unity usually assigns the mesh automatically if a MeshFilter is present,
			// but we'll explicitly set it to be 100% sure.
			meshCol.sharedMesh = filter.sharedMesh;

			Debug.Log($"Added Mesh Collider to {selected.name}");
		}
		else
		{
			Debug.LogWarning("Selected object has no MeshFilter. MeshCollider not added.");
		}
	}

	// The " _n" at the end tells Unity to use the 'N' key as the shortcut.
	[MenuItem("Tools/Colliders/Add Box Collider _n")]
	static void AddBoxColliderToSelected()
	{
		GameObject selected = Selection.activeGameObject;

		if (selected == null) return;

		// Adding a BoxCollider automatically triggers Unity's bounds calculation
		Undo.AddComponent<BoxCollider>(selected);

		Debug.Log($"Added Box Collider to {selected.name}");
	}
}