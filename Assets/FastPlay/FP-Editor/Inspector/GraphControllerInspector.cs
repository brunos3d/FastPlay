#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GraphController), true)]
	public class GraphControllerInspector : UnityEditor.Editor {

		private const string PREFS_INSPECTOR_FOLDOUT  = "FastPlay: GraphControllerInspector foldout";

		private const string PREFS_INSPECTOR_DEBUG = "FastPlay: GraphControllerInspector debug";

		private bool foldout {
			get {
				return EditorPrefs.GetBool(PREFS_INSPECTOR_FOLDOUT, true);
			}
			set {
				EditorPrefs.SetBool(PREFS_INSPECTOR_FOLDOUT, value);
			}
		}

		private bool debug {
			get {
				return EditorPrefs.GetBool(PREFS_INSPECTOR_DEBUG, false);
			}
			set {
				EditorPrefs.SetBool(PREFS_INSPECTOR_DEBUG, value);
			}
		}

		private float height;

		private void OnEnable() {
			Undo.undoRedoPerformed += UndoRedoPerformed;
			EditorApplication.update += Repaint;
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= UndoRedoPerformed;
			EditorApplication.update -= Repaint;
		}

		private void UndoRedoPerformed() {
			GraphController controller = (GraphController)target;
			controller.CopyParameters();
		}

		public override void OnInspectorGUI() {
			GraphController controller = (GraphController)target;
			GraphAsset graph = controller.graph;

			OnEnableAction on_enable = controller.on_enable;
			bool once = controller.once;
			float seconds = controller.seconds;
			OnDisableAction on_disable = controller.on_disable;

			controller.CopyParameters();

			graph = (GraphAsset)EditorGUILayout.ObjectField(new GUIContent("Graph"), controller.graph, typeof(GraphAsset), false);

			if (controller.graph != graph) {
				controller.OnGraphChange(controller.graph, graph);
			}

			if (controller.graph) {
				on_enable = (OnEnableAction)EditorGUILayout.EnumPopup(new GUIContent("OnEnable"), controller.on_enable);

				switch (on_enable) {
					case OnEnableAction.DoNothing:
					case OnEnableAction.PlayGraph:
						if (height != 0.0f) {
							height = 0.0f;
							EditorUtils.RepaintInspector();
						}
						seconds = 0.0f;
						break;
					case OnEnableAction.WaitForSeconds:
						GUILayout.BeginVertical("Box");
						if (height != EditorGUIUtility.singleLineHeight) {
							height = Mathf.MoveTowards(height, EditorGUIUtility.singleLineHeight, 1.0f);
							EditorUtils.RepaintInspector();
						}
						once = EditorGUILayout.Toggle(new GUIContent("Only Once"), controller.once, GUILayout.Height(height));
						seconds = EditorGUILayout.FloatField(new GUIContent("Seconds"), controller.seconds, GUILayout.Height(height));
						GUILayout.EndVertical();
						break;
				}
				on_disable = (OnDisableAction)EditorGUILayout.EnumPopup(new GUIContent("OnDisable"), controller.on_disable);
			}

			GUILayout.BeginHorizontal("RL Header");
			{
				GUILayout.Space(15.0f);
				GUILayout.BeginVertical();
				{
					foldout = EditorGUILayout.Foldout(foldout, new GUIContent(string.Format("Variables [{0}]", controller.properties.Values.Where(p => p.is_public).Count())));
				}
				GUILayout.EndVertical();
				if (GUILayout.Toggle(debug, debug ? "Debug" : "Normal", (GUIStyle)"minibutton") != debug) {
					debug = !debug;
				}
			}
			GUILayout.EndHorizontal();

			if (foldout) {
				GUILayout.BeginHorizontal("RL Background", GUILayout.MinHeight(10.0f));
				{
					GUILayout.Space(10.0f);
					GUILayout.BeginVertical();
					{
						foreach (VariableObject property in controller.properties.Values.ToList()) {
							if (property.is_public || debug) {
								object last_value = property.GetValue();
								object new_value = GUIDraw.AnyField(last_value, property.valueType, debug ? property.name + string.Format(" (ID = {0}, Type = {1})", property.id, property.valueType.GetTypeName(true, true)) : property.name);
								if (new_value != last_value) {
									UndoManager.RecordObject(target, "Change Value");
									property.SetValue(new_value);
								}
							}
						}
					}
					GUILayout.Space(5.0f);
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
			}

			if (GUI.changed) {
				UndoManager.RecordObject(target, "GraphController Inspector");
				controller.graph = graph;
				controller.on_enable = on_enable;
				controller.once = once;
				controller.seconds = seconds;
				controller.on_disable = on_disable;
				UndoManager.SetDirty(target);
			}
		}
	}
}
#endif
