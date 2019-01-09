#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(GraphAsset), true)]
	public class GraphAssetInspector : UnityEditor.Editor {

		private class Styles {

			public static readonly string[] MENUS = new string[] { "Graph", "Parameters", "Variables", "Node Inspector" };

			public GUIStyle menu_button;

			public Styles() {
				menu_button = FPSkin.GetStyle("LargeButton");
			}
		}

		private static Styles m_styles;

		private GraphAsset m_asset;

		private int current_menu;

		private ReorderableList variable_list;

		private ReorderableList input_list;

		private ReorderableList output_list;

		private static Styles styles {
			get {
				return m_styles ?? (m_styles = new Styles());
			}
		}

		private GraphAsset asset {
			get {
				return m_asset ?? (m_asset = (GraphAsset)target);
			}
		}

		private void OnEnable() {
			CreateReorderableLists();
			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= UndoRedoPerformed;
		}

		private void UndoRedoPerformed() {
			CreateReorderableLists();
		}

		public override void OnInspectorGUI() {
			bool restore_wide_mode = EditorGUIUtility.wideMode;
			EditorGUIUtility.wideMode = true;
			EditorGUIUtility.labelWidth = 0.0f;
			EditorGUIUtility.fieldWidth = 0.0f;

			if (GUILayout.Button("Open in Editor")) {
				GraphEditorWindow.OpenEditor(asset);
			}
			try {
				switch (MenuMode()) {
					//Graph
					case 0:
						GUILayout.Label(string.Format("Nodes: {0}", asset.graph.nodes.Count.ToString()));
						break;
					//Parameters
					case 1:
						input_list.DoLayoutList();
						output_list.DoLayoutList();
						break;
					//Variables
					case 2:
						variable_list.DoLayoutList();
						break;
					//Node Inspector
					case 3:
						DrawNodeInspector();
						break;
				}
			}
			catch {
				CreateReorderableLists();
			}


			if (GUI.changed) {
				UndoManager.SetDirty(target);
				GUI.changed = false;
			}

			EditorGUIUtility.wideMode = restore_wide_mode;
		}

		private int MenuMode() {
			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			current_menu = GUILayout.Toolbar(current_menu, Styles.MENUS, styles.menu_button, GUI.ToolbarButtonSize.FitToContents);

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			return current_menu;
		}

		private void DrawNodeInspector() {
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
							if (plug != null && plug.IsPlugged()) {
								GUILayout.BeginHorizontal();
								GUI.enabled = false;
								GUIDraw.AnyField(input.GetDefaultValue(), input.valueType, port.name, GUILayout.ExpandWidth(true));
								GUI.enabled = true;
								if (GUILayout.Button("x", GUILayout.Width(20.0f))) {
									UndoManager.RecordObject(asset, "Unplug Port");
									plug.Unplug();
								}
								GUILayout.EndHorizontal();
							}
							else {
								object last_value = input.GetDefaultValue();
								object new_value = GUIDraw.AnyField(last_value, input.valueType, port.name, GUILayout.ExpandWidth(true));
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
		}

		private void CreateReorderableLists() {
			GraphAsset asset = (GraphAsset)target;

			variable_list = new ReorderableList(asset.graph.variableParameters, typeof(Parameter), true, true, true, true);
			input_list = new ReorderableList(asset.graph.inputParameters, typeof(Parameter), true, true, true, true);
			output_list = new ReorderableList(asset.graph.outputParameters, typeof(Parameter), true, true, true, true);

			variable_list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, "Variables Parameters");
			};
			input_list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, "Input Parameters");
			};
			output_list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, "Output Parameters");
			};

			variable_list.onReorderCallback = list => {
				asset.Validate();
			};
			input_list.onReorderCallback = list => {
				asset.Validate();
			};
			output_list.onReorderCallback = list => {
				asset.Validate();
			};

			variable_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				rect.height = EditorGUIUtility.singleLineHeight;
				Rect rect_declaring = new Rect(rect.x, rect.y, 10.0f, EditorGUIUtility.singleLineHeight);
				rect.y += 2;
				Rect rect_name = new Rect(rect.x + 20.0f, rect.y, rect.width / 2.0f, EditorGUIUtility.singleLineHeight);
				Rect rect_type = new Rect(rect.x + rect.width / 2.0f + 30.0f, rect.y, rect.width - (rect.width / 2.0f + 30.0f), EditorGUIUtility.singleLineHeight);
				string new_name = EditorGUI.TextField(rect_name, asset.graph.variableParameters[index].name);
				bool new_declaring_value = EditorGUI.Toggle(rect_declaring, asset.graph.variableParameters[index].is_public);
				if (new_name != asset.graph.variableParameters[index].name) {
					UndoManager.RecordObject(asset, "VariableObject name change");
					asset.graph.variableParameters[index].name = new_name;
					GraphEditor.current_validate = true;
				}
				if (new_declaring_value != asset.graph.variableParameters[index].is_public) {
					UndoManager.RecordObject(asset, "VariableObject declaring type change");
					asset.graph.variableParameters[index].is_public = new_declaring_value;
					GraphEditor.current_validate = true;
				}
				GUI.Label(rect_type, asset.graph.variableParameters[index].valueType.GetTypeName());
			};
			input_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				rect.height = EditorGUIUtility.singleLineHeight;
				Rect rect_declaring = new Rect(rect.x, rect.y, 10.0f, EditorGUIUtility.singleLineHeight);
				rect.y += 2;
				Rect rect_name = new Rect(rect.x + 20.0f, rect.y, rect.width / 2.0f, EditorGUIUtility.singleLineHeight);
				Rect rect_type = new Rect(rect.x + rect.width / 2.0f + 30.0f, rect.y, rect.width - (rect.width / 2.0f + 30.0f), EditorGUIUtility.singleLineHeight);
				string new_name = EditorGUI.TextField(rect_name, asset.graph.inputParameters[index].name);
				bool new_declaring_value = EditorGUI.Toggle(rect_declaring, asset.graph.inputParameters[index].is_public);
				if (new_name != asset.graph.inputParameters[index].name) {
					UndoManager.RecordObject(asset, "VariableObject name change");
					asset.graph.inputParameters[index].name = new_name;
					GraphEditor.current_validate = true;
				}
				if (new_declaring_value != asset.graph.inputParameters[index].is_public) {
					UndoManager.RecordObject(asset, "VariableObject declaring type change");
					asset.graph.inputParameters[index].is_public = new_declaring_value;
					GraphEditor.current_validate = true;
				}
				GUI.Label(rect_type, asset.graph.inputParameters[index].valueType.GetTypeName());
			};
			output_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				rect.height = EditorGUIUtility.singleLineHeight;
				Rect rect_declaring = new Rect(rect.x, rect.y, 10.0f, EditorGUIUtility.singleLineHeight);
				rect.y += 2;
				Rect rect_name = new Rect(rect.x + 20.0f, rect.y, rect.width / 2.0f, EditorGUIUtility.singleLineHeight);
				Rect rect_type = new Rect(rect.x + rect.width / 2.0f + 30.0f, rect.y, rect.width - (rect.width / 2.0f + 30.0f), EditorGUIUtility.singleLineHeight);
				string new_name = EditorGUI.TextField(rect_name, asset.graph.outputParameters[index].name);
				bool new_declaring_value = EditorGUI.Toggle(rect_declaring, asset.graph.outputParameters[index].is_public);
				if (new_name != asset.graph.outputParameters[index].name) {
					UndoManager.RecordObject(asset, "VariableObject name change");
					asset.graph.outputParameters[index].name = new_name;
					GraphEditor.current_validate = true;
				}
				if (new_declaring_value != asset.graph.outputParameters[index].is_public) {
					UndoManager.RecordObject(asset, "VariableObject declaring type change");
					asset.graph.outputParameters[index].is_public = new_declaring_value;
					GraphEditor.current_validate = true;
				}
				GUI.Label(rect_type, asset.graph.outputParameters[index].valueType.GetTypeName());
			};

			variable_list.onRemoveCallback = (ReorderableList list) => {
				UndoManager.RecordObject(target, "Remove parameter");
				asset.graph.RemoveParameterAt(list.index, ParameterType.Variable);
				list.index = list.count - 1;
				GraphEditor.current_validate = true;
			};
			input_list.onRemoveCallback = (ReorderableList list) => {
				UndoManager.RecordObject(target, "Remove parameter");
				asset.graph.RemoveParameterAt(list.index, ParameterType.Input);
				list.index = list.count - 1;
				GraphEditor.current_validate = true;
			};
			output_list.onRemoveCallback = (ReorderableList list) => {
				UndoManager.RecordObject(target, "Remove parameter");
				asset.graph.RemoveParameterAt(list.index, ParameterType.Output);
				list.index = list.count - 1;
				GraphEditor.current_validate = true;
			};

			variable_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
				ShowMenu(asset, ParameterType.Variable);
			};

			input_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
				ShowMenu(asset, ParameterType.Input);
			};

			output_list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
				ShowMenu(asset, ParameterType.Output);
			};
		}

		private void ShowMenu(GraphAsset asset, ParameterType parameter_type) {
			GenericMenu generic_menu = new GenericMenu();
			if (parameter_type == ParameterType.Input) {
				generic_menu.AddItem(new GUIContent("Input"), false, () => {
					UndoManager.RecordObject(target, "Add new parameter");
					asset.graph.AddCustomParameter("new InputPort", typeof(ParameterInput), parameter_type);
					GraphEditor.current_validate = true;
					CreateReorderableLists();
				});
			}
			if (parameter_type == ParameterType.Output) {
					generic_menu.AddItem(new GUIContent("Output"), false, () => {
					UndoManager.RecordObject(target, "Add new parameter");
					asset.graph.AddCustomParameter("new OutputPort", typeof(ParameterOutput), parameter_type);
					GraphEditor.current_validate = true;
					CreateReorderableLists();
				});
			}

			foreach (Type type in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
				if (!(type.IsAbstract & type.IsSealed) && !type.IsGenericType) {
					string type_name = type.GetTypeName(true);
					if (type_name.Contains("System")) {
						//List
						generic_menu.AddItem(new GUIContent("List/System/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), typeof(List<>).MakeGenericType(type), parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
						generic_menu.AddItem(new GUIContent("System/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), type, parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
					}
					else if (type_name.Contains("FastPlay")) {
						//List
						generic_menu.AddItem(new GUIContent("List/FastPlay/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), typeof(List<>).MakeGenericType(type), parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
						generic_menu.AddItem(new GUIContent("FastPlay/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), type, parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
					}
					else if (type_name.Contains("UnityEngine")) {
						//List
						generic_menu.AddItem(new GUIContent("List/UnityEngine/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), typeof(List<>).MakeGenericType(type), parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
						generic_menu.AddItem(new GUIContent("UnityEngine/" + type.GetTypeName()), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), type, parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
					}
					else {
						//List
						generic_menu.AddItem(new GUIContent("List/References/" + type.GetTypeName(true).Replace(".", "/")), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), typeof(List<>).MakeGenericType(type), parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
						generic_menu.AddItem(new GUIContent("References/" + type.GetTypeName(true).Replace(".", "/")), false, (object obj) => {
							UndoManager.RecordObject(target, "Add new parameter");
							asset.graph.AddParameter("new " + type.GetTypeName(), type, parameter_type);
							GraphEditor.current_validate = true;
							CreateReorderableLists();
						}, type);
					}
				}
			}
			generic_menu.ShowAsContext();
		}
	}
}
#endif
