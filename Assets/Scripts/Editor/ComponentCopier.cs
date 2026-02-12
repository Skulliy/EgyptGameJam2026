using UnityEngine;
using UnityEditor;

namespace EditorTools
{
	// This script runs only in the Unity Editor
	public class ComponentCopier : Editor
	{
		// --- Static storage for the copied data ---
		private static bool hasData = false;
		private static string copiedName;

		// Transform Data
		private static Vector3 copiedPosition;
		private static Quaternion copiedRotation;
		private static Vector3 copiedScale;

		// Mesh Data
		private static Mesh copiedMesh;
		private static Material[] copiedMaterials;

		// Box Collider Data
		private static Vector3 copiedBoxCenter;
		private static Vector3 copiedBoxSize;
		private static bool copiedIsTrigger;
		private static PhysicMaterial copiedPhysicMaterial;

		// --- SHORTCUT P: COPY ---
		// The " _p" defines the hotkey as the "P" key.
		[MenuItem("Tools/Copy Components _p")]
		static void CopyComponents()
		{
			GameObject source = Selection.activeGameObject;

			if (source == null)
			{
				Debug.LogWarning("Component Copier: No object selected.");
				return;
			}

			copiedName = source.name; // Capture the name here

			// Get the required components
			MeshFilter mf = source.GetComponent<MeshFilter>();
			MeshRenderer mr = source.GetComponent<MeshRenderer>();
			BoxCollider bc = source.GetComponent<BoxCollider>();

			// Validation: Ensure the source has strictly all required components
			if (mf == null || mr == null || bc == null)
			{
				Debug.LogError($"Component Copier: Failed. Object '{source.name}' must have a MeshFilter, MeshRenderer, and BoxCollider.");
				return;
			}

			// Store Transform Data
			copiedPosition = source.transform.localPosition;
			copiedRotation = source.transform.localRotation;
			copiedScale = source.transform.localScale;

			// Store Mesh Data
			copiedMesh = mf.sharedMesh;
			copiedMaterials = mr.sharedMaterials;

			// Store Box Collider Data
			copiedBoxCenter = bc.center;
			copiedBoxSize = bc.size;
			copiedIsTrigger = bc.isTrigger;
			copiedPhysicMaterial = bc.sharedMaterial;

			hasData = true;

			source.SetActive(false);

			Debug.Log($"<color=green>Success:</color> Components copied from <b>{source.name}</b>. Select another object and press 'O' to paste.");
		}

		// --- SHORTCUT O: PASTE ---
		// The " _o" defines the hotkey as the "O" key.
		[MenuItem("Tools/Paste Components _o")]
		static void PasteComponents()
		{
			if (!hasData)
			{
				Debug.LogError("Component Copier: Clipboard is empty. Select an object and press 'P' first.");
				return;
			}

			GameObject target = Selection.activeGameObject;
			if (target == null) return;

			// REGISTER UNDO: This allows you to Ctrl+Z the changes
			Undo.RegisterCompleteObjectUndo(target, "Paste Components");


			target.name = copiedName + " Pickable";

			// 1. CLEANUP: Remove specific existing components as requested

			// Remove ANY Collider (Box, Sphere, Mesh, Capsule, etc.)
			Collider[] existingColliders = target.GetComponents<Collider>();
			foreach (var col in existingColliders)
			{
				Undo.DestroyObjectImmediate(col);
			}

			// Remove existing MeshFilter if present
			MeshFilter existingMF = target.GetComponent<MeshFilter>();
			if (existingMF != null) Undo.DestroyObjectImmediate(existingMF);

			// Remove existing MeshRenderer if present
			MeshRenderer existingMR = target.GetComponent<MeshRenderer>();
			if (existingMR != null) Undo.DestroyObjectImmediate(existingMR);

			// 2. APPLY: Add the copied components and values

			// Apply Transform (We cannot destroy Transform, so we overwrite values)
			target.transform.localPosition = copiedPosition;
			target.transform.localRotation = copiedRotation;
			target.transform.localScale = copiedScale;

			// Add and Setup MeshFilter
			MeshFilter newMF = Undo.AddComponent<MeshFilter>(target);
			newMF.sharedMesh = copiedMesh;

			// Add and Setup MeshRenderer
			MeshRenderer newMR = Undo.AddComponent<MeshRenderer>(target);
			newMR.sharedMaterials = copiedMaterials;

			// Add and Setup BoxCollider
			BoxCollider newBC = Undo.AddComponent<BoxCollider>(target);
			newBC.center = copiedBoxCenter;
			newBC.size = copiedBoxSize;
			newBC.isTrigger = copiedIsTrigger;
			newBC.sharedMaterial = copiedPhysicMaterial;

			Debug.Log($"<color=cyan>Success:</color> Applied components to <b>{target.name}</b>.");
		}
	}
}