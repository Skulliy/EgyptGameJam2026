using UnityEditor;
using UnityEngine;

public class AnchorsToCorners : Editor
{
	[MenuItem("RectTransform/Anchors to Corners %g")] // Ctrl+G shortcut
	static void Snapping()
	{
		GameObject go = Selection.activeGameObject;
		if (go == null || go.GetComponent<RectTransform>() == null) return;

		RectTransform t = go.GetComponent<RectTransform>();
		RectTransform p = go.transform.parent.GetComponent<RectTransform>();

		Undo.RecordObject(t, "Anchor Snap");

		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / p.rect.width,
											t.anchorMin.y + t.offsetMin.y / p.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / p.rect.width,
											t.anchorMax.y + t.offsetMax.y / p.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = Vector2.zero;
	}
}