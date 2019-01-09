#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	[InitializeOnLoad]
	public class GraphHierarchy {

		private class Styles {

			public GUIStyle icon;

			public Styles() {
				icon = new GUIStyle(FPSkin.icon);
			}
		}

		private static bool set_dirty;

		private static Styles styles;

		private static GameObject target;

		static GraphHierarchy() {
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyIndex;
		}

		private static void HierarchyIndex(int instance_id, Rect index_rect) {
			Event current = Event.current;

			if (styles == null) {
				styles = new Styles();
			}

			target = EditorUtility.InstanceIDToObject(instance_id) as GameObject;
			if (index_rect.Contains(current.mousePosition)) {
				InputGUI();
			}
			if (target) {
				if (target.GetComponent<GraphController>()) {
					int graph_count = target.GetComponents<GraphController>().Length;

					Rect rect = new Rect(index_rect.width - 10.0f, index_rect.y - 2.0f, 20.0f, 20.0f);
					GUIContent content = new GUIContent(graph_count > 1 ? graph_count.ToString() : string.Empty, string.Empty);

					if (GUI.Button(rect, content, styles.icon)) {
						Selection.activeObject = target;
						GraphEditorWindow.OpenEditor(target.GetComponent<GraphController>().graph);
					}
				}
				if (set_dirty) {
					UndoManager.SetDirty(target);
					set_dirty = false;
				}
			}
			target = null;
		}

		private static void InputGUI() {
			Event current = Event.current;
			if (target) {
				switch (current.type) {
					case EventType.DragUpdated:
					case EventType.DragPerform:
						if (current.button == 0) {
							if (DragAndDrop.objectReferences.Length > 0) {
								if (DragAndDrop.objectReferences[0] is GraphAsset) {
									if (current.type == EventType.DragPerform) {
										DragAndDrop.AcceptDrag();
										GraphController controller = UndoManager.AddComponent<GraphController>(target);
										controller.graph = (GraphAsset)DragAndDrop.objectReferences[0];
										EditorGUIUtility.PingObject(controller);
										Selection.activeGameObject = target;
										set_dirty = true;
										current.Use();
									}
									DragAndDrop.PrepareStartDrag();
								}
							}
						}
						break;
				}
			}
		}
	}
}
#endif
