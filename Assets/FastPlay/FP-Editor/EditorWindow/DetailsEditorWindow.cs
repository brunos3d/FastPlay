#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public class DetailsEditorWindow : EditorWindow {

		public static bool is_open;

		public static DetailsEditorWindow editor;

		private ReorderableList input_list;

		private ReorderableList output_list;

		public Vector2 scroll;

		[MenuItem("Tools/FastPlay/Details")]
		public static DetailsEditorWindow Init() {
			Type inspector_type = Type.GetType("UnityEditor.InspectorWindow, UnityEditor.dll");
			editor = GetWindow<DetailsEditorWindow>(inspector_type);
			Texture2D icon = EditorUtils.FindAssetByName<Texture2D>("title_content");
			editor.titleContent = new GUIContent("Details", icon);
			return editor;
		}

		void OnEnable() {
			is_open = true;
		}

		void OnDisable() {
			is_open = false;
		}

		void OnGUI() {
			Graph graph = GraphEditor.graph;
			GraphAsset asset = GraphEditor.asset;

			scroll = EditorGUILayout.BeginScrollView(scroll);
			if (asset && graph) {
				GUILayout.Label(string.Format("Nodes: {0}", asset.graph.nodes.Count.ToString()));
				if (!GraphEditor.is_drag && GraphEditor.activeNode) {
					GUILayout.Space(20.0f);
					foreach (Node node in GraphEditor.selection) {
						//GUILayout.Label(string.Format("{0} : {1}", GraphEditor.scroll, node.position));
						GUILayout.BeginVertical(node.name, "window");
						if (node is IListPort) {
							GUILayout.BeginHorizontal();
							if (GUILayout.Button("Add Port")) {
								UndoManager.RecordObject(asset, "Add Port");
								((IListPort)node).AddPort();
								node.Validate();
							}
							if (GUILayout.Button("Remove Port")) {
								UndoManager.RecordObject(asset, "Remove Node");
								((IListPort)node).RemovePort();
								node.Validate();
							}
							GUILayout.EndHorizontal();
						}
						foreach (Port port in node.portValues) {
							var plug = port as IPlug;
							var input = port as IInputValue;
							var output = port as IOutputValue;
							if (input != null) {
								if (plug != null) {
									if (plug.IsPlugged()) {
										GUILayout.BeginHorizontal();
										GUI.enabled = false;
										GUIDraw.AnyField(input.GetDefaultValue(), input.valueType, port.name);
										GUI.enabled = true;
										if (GUILayout.Button("x", GUILayout.Width(20.0f))) {
											UndoManager.RecordObject(asset, "Unplug Port");
											plug.Unplug();
										}
										GUILayout.EndHorizontal();
									}
									else {
										object last_value = input.GetDefaultValue();
										object new_value = GUIDraw.AnyField(last_value, input.valueType, port.name);
										if (new_value != last_value) {
											UndoManager.RecordObject(asset, "Change Value");
											input.SetDefaultValue(new_value);
										}
									}
								}
								else {
									object last_value = input.GetDefaultValue();
									object new_value = GUIDraw.AnyField(last_value, input.valueType, port.name);
									if (new_value != last_value) {
										UndoManager.RecordObject(asset, "Change Value");
										input.SetDefaultValue(new_value);
									}
								}
							}
							else if (output != null) {
								GUILayout.BeginHorizontal();
								GUILayout.Label("[PORT] " + port.name);
								GUILayout.FlexibleSpace();
								GUILayout.Label(string.Format("<{0}>", output.valueType.GetTypeName()));
								if (plug != null && plug.IsPlugged()) {
									if (GUILayout.Button("x", GUILayout.Width(20.0f))) {
										UndoManager.RecordObject(asset, "Unplug Port");
										plug.Unplug();
									}
								}
								GUILayout.EndHorizontal();
							}
							else {
								GUILayout.BeginHorizontal();
								GUILayout.Label("[PORT] " + port.name);
								if (plug != null) {
									GUILayout.FlexibleSpace();
									if (plug.IsPlugged()) {
										GUILayout.Label("[PLUGGED]");
									}
									else {
										GUILayout.Label("[UNPLUGGED]");
									}
								}
								GUILayout.FlexibleSpace();
								GUILayout.Label("[ACTION]");
								if (plug != null && plug.IsPlugged()) {
									if (GUILayout.Button("x", GUILayout.Width(20.0f))) {
										UndoManager.RecordObject(asset, "Unplug Port");
										plug.Unplug();
									}
								}
								GUILayout.EndHorizontal();
							}
						}
						GUILayout.EndVertical();
					}
					GUILayout.FlexibleSpace();
				}
				if (GUI.changed) {
					UndoManager.SetDirty(asset);
					GUI.changed = false;
				}
			}
			EditorGUILayout.EndScrollView();
			Repaint();
		}
	}
}
#endif
