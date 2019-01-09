#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public class SettingsEditorWindow : EditorWindow {

		public Vector2 scroll;

		[MenuItem("Tools/FastPlay/Settings")]
		public static SettingsEditorWindow Init() {
			return GetWindow<SettingsEditorWindow>();
		}

		void OnGUI() {
			scroll = EditorGUILayout.BeginScrollView(scroll);
			if (GUILayout.Button("SAVE")) {
				GraphEditor.asset.SaveData();
			}
			if (GUILayout.Button("RESET POSITION")) {
				GraphEditor.scroll = Vector2.zero;
			}
			if (GUILayout.Button("GO TO CENTER")) {
				GraphEditor.scroll = new Vector2(0.0f, 10.0f) + FPMath.CenterOfPoints(GraphEditor.graph.nodes.Select(n => (-n.position - (n.size / 2.0f)) * GraphEditor.zoom + (GraphEditor.window.size / 2.0f) / GraphEditor.zoom).ToList());
			}
			EditorGUILayout.EndScrollView();
		}
	}
}
#endif
