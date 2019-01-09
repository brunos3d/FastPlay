#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public class GraphEditorWindow : EditorWindow {

		private class Styles {
			public GUIStyle border;
			public GUIStyle header;

			public Styles() {
				border = FPSkin.border;
				header = FPSkin.header;
			}
		}

		private const float TOOLBAR_HEIGHT = 17.0f;

		private Styles styles;

		private Event current;

		private GenericMenu generic_menu;

		private bool internal_error;

		private Matrix4x4 matrix;

		private float target_zoom = 1.0f;

		private Rect editable_area;

		private Rect view_area;

		private Rect zoom_area;

		private Rect select_box;

		private PropertyInfo docked;

		private Texture2D background;

		private List<Texture2D> anim = new List<Texture2D>();

		private static bool set_dirty = false;

		public static bool is_open;

		public static GraphEditorWindow editor;

		public bool isDocked {
			get {
				if (docked == null) {
					docked = typeof(EditorWindow).GetProperty("docked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				}
				return (bool)docked.GetValue(editor, null) == true;
			}
		}

		[MenuItem("Tools/FastPlay/Graph Editor", priority = 81)]
		public static GraphEditorWindow Init() {
			editor = GetWindow<GraphEditorWindow>(typeof(SceneView));
			Texture2D icon = EditorUtils.FindAssetByName<Texture2D>("title_content");
			editor.titleContent = new GUIContent("FastPlay", icon);
			if (!DetailsEditorWindow.is_open) {
				DetailsEditorWindow.Init();
			}
			return editor;
		}

		public static void OpenEditor(GraphAsset asset) {
			editor = Init();
			SetGraphAsset(asset);
		}

		public static void SetGraphAsset(GraphAsset asset) {
			if (Resources.FindObjectsOfTypeAll<GraphEditorWindow>()[0]) {
				editor = Resources.FindObjectsOfTypeAll<GraphEditorWindow>()[0];
			}
			else {
				editor = Init();
			}
			if (asset != GraphEditor.asset) {
				GraphEditor.ClearSelection();
			}
			if (GraphEditor.asset) {
				GraphEditor.asset.position = GraphEditor.scroll;
				GraphEditor.asset.SaveData();
			}

			if (asset) {
				asset.Validate();
				GraphEditor.scroll = asset.position;
			}

			GraphEditor.asset = asset;
			SetGraph(asset ? asset.graph : null);
		}

		public static void SetGraph(Graph graph) {
			if (Resources.FindObjectsOfTypeAll<GraphEditorWindow>()[0]) {
				editor = Resources.FindObjectsOfTypeAll<GraphEditorWindow>()[0];
			}
			else {
				editor = Init();
			}
			GraphEditor.graph = graph;
		}

		public static T AddNode<T>(Vector2 position = default(Vector2), bool validate = true) where T : Node {
			if (!GraphEditor.graph) return null;
			UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Add Node", GraphEditor.asset.name));
			set_dirty = true;
			return GraphEditor.graph.AddNode<T>(position, validate);
		}

		public static MacroNode AddMacro(GraphAsset reference, Vector2 position = default(Vector2)) {
			if (!GraphEditor.graph) return null;
			UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Add Graph", GraphEditor.asset.name));
			set_dirty = true;
			return GraphEditor.graph.AddMacro(reference, position);
		}

		public static Node AddNode(Type type, Vector2 position = default(Vector2)) {
			if (!GraphEditor.graph) return null;
			UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Add Node", GraphEditor.asset.name));
			set_dirty = true;
			return GraphEditor.graph.AddNode(type, position);
		}

		public static Node AddCustomNode(Type type, Vector2 position = default(Vector2), params object[] args) {
			if (!GraphEditor.graph) return null;
			UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Add Node", GraphEditor.asset.name));
			set_dirty = true;
			return GraphEditor.graph.AddCustomNode(type, position, args);
		}

		private void OnEnable() {
			is_open = true;
			editor = this;
			styles = new Styles();

			Undo.undoRedoPerformed += UndoRedoPerformed;
			EditorApplication.update += Update;
			EditorApplication.playModeStateChanged += OnStateChange;

			background = EditorUtils.FindAssetByName<Texture2D>("background_dark");
			anim = new List<Texture2D>();
			for (int id = 1; id < 22; id++) {
				anim.Add(EditorUtils.FindAssetByName<Texture2D>(string.Format("anim ({0})", id)));
			}
			OnSelectionChange();
		}
		private void OnDisable() {
			Undo.undoRedoPerformed -= UndoRedoPerformed;
			EditorApplication.update -= Update;
			EditorApplication.playModeStateChanged -= OnStateChange;
			is_open = false;
		}

		private void OnFocus() {
			OnSelectionChange();
		}

		private void OnStateChange(PlayModeStateChange state) {
			OnSelectionChange();
		}

		private void OnSelectionChange() {
			if (Selection.activeGameObject) {
				GraphController controller = Selection.activeGameObject.GetComponent<GraphController>();
				if (controller) {
					GraphAsset asset = controller.graph;
					if (asset && GraphEditor.graph != asset.graph) {
						SetGraphAsset(GraphEditor.asset);
					}
				}
			}
		}

		private void UndoRedoPerformed() {
			OpenEditor(UndoManager.asset_undo);
			ClearSelection();
			set_dirty = true;
			Repaint();
		}

		private void Update() {
			if (GraphEditor.current_validate) {
				if (GraphEditor.asset) {
					GraphEditor.asset.Validate();
				}
				GraphEditor.current_validate = false;
			}
			if (GraphEditor.zoom != target_zoom) {
				GraphEditor.zoom = Mathf.MoveTowards(GraphEditor.zoom, target_zoom, EditorTime.deltaTime);
			}
			view_area = new Rect(Vector2.zero, position.size / GraphEditor.zoom);
			editable_area = new Rect(0.0f, 57.0f, position.width, position.height - 57.0f);
		}

		public void DrawLogoAnim() {
			int index = (int)Mathf.Clamp((EditorTime.time * 5.0f) % 21, 0, 21);
			Vector2 size = new Vector2(512.0f, 512.0f);
			GUI.Label(new Rect(position.size / 2.0f - size / 2.0f, size), anim[index]);
		}

		float bkp_zoom = 1.0f;
		Vector2 bkp_scroll = Vector2.zero;
		public void OnGUI() {
			bkp_zoom = GraphEditor.zoom;
			bkp_scroll = GraphEditor.scroll;

			GraphEditor.window = position;
			Color gui_background_color = GUI.backgroundColor;
			if (EditorApplication.isCompiling) {
				DrawLogoAnim();
				ShowNotification(new GUIContent("Compiling, please wait..."));
				return;
			}
			if (internal_error) {
				DrawLogoAnim();
				ShowNotification(new GUIContent("INTERNAL ERROR!\nPlease contact the support."));
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Retry")) {
					internal_error = false;
				}
				if (GUILayout.Button("Close Editor")) {
					editor.Close();
				}
				GUILayout.EndHorizontal();
				return;
			}
			try {
				bool dragging = GraphEditor.is_drag && GraphEditor.hover_node;
				if (GraphEditor.asset && GraphEditor.graph) {
					current = Event.current;
					GraphEditor.mouse_position = current.mousePosition;

					if (isDocked) {
						zoom_area = new Rect(0.0f, 19.0f, position.width, position.height);
					}
					else {
						zoom_area = new Rect(0.0f, 22.0f, position.width, position.height);
					}

					// Window draw layers
					RefreshHoverElements();

					BeginZoomArea();

					DrawScrollBackground();

					if (GraphEditor.drag_port) {
						if (GraphEditor.drag_port is IInputPort) {
							Node.DrawConnection(GraphEditor.drag_port.rect.center, GraphEditor.mouse_position / GraphEditor.zoom, GraphEditor.drag_port.color, true);
						}
						else {
							Node.DrawConnection(GraphEditor.mouse_position / GraphEditor.zoom, GraphEditor.drag_port.rect.center, GraphEditor.drag_port.color, true);
						}
						if (GraphEditor.drag_port is ActionPort) {
							GUI.Label(new Rect((GraphEditor.mouse_position) / GraphEditor.zoom - Node.PORT_SIZE / 2.0f, Node.PORT_SIZE), GUIContent.none, Node.styles.on_input_action);
						}
						else {
							GUI.backgroundColor = GraphEditor.drag_port.color;
							GUI.Label(new Rect((GraphEditor.mouse_position) / GraphEditor.zoom - Node.PORT_SIZE / 2.0f, Node.PORT_SIZE), GUIContent.none, Node.styles.on_input_port);
							GUI.backgroundColor = gui_background_color;
						}
					}

					DrawNodes();

					InputGUI();

					EndZoomArea();

					DrawSelectBox();

					DrawWindowBorder();

					DrawWindowHeader();

					DrawToolbar();

					if (dragging != (GraphEditor.is_drag && GraphEditor.hover_node)) {
						UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Move Nodes", GraphEditor.asset.name));
						set_dirty = true;
					}

					if (set_dirty) {
						UndoManager.SetDirty(GraphEditor.asset);
						set_dirty = false;
					}
				}
				else {
					DrawLogoAnim();
				}
				if (dragging || bkp_zoom != GraphEditor.zoom || bkp_scroll != GraphEditor.scroll) {
					bkp_zoom = GraphEditor.zoom;
					bkp_scroll = GraphEditor.scroll;
					RepaintInfo();
				}
				Repaint();
			}
			catch (Exception except) {
				Debug.LogException(except);
				internal_error = true;
			}
		}

		public void DrawToolbar() {
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();

			if (GUILayout.Toggle(GraphEditor.makeAllNodesActive, new GUIContent("Relations", "Make all nodes active."), (GUIStyle)"ToolbarButton") != GraphEditor.makeAllNodesActive) {
				GraphEditor.makeAllNodesActive = !GraphEditor.makeAllNodesActive;
			}
			if (GUILayout.Toggle(GraphEditor.showPortValues, new GUIContent("Values", "Show port values."), (GUIStyle)"ToolbarButton") != GraphEditor.showPortValues) {
				GraphEditor.showPortValues = !GraphEditor.showPortValues;
			}

			GraphEditor.connectorType = (ConnectorType)EditorGUILayout.EnumPopup(new GUIContent(string.Empty), GraphEditor.connectorType, (GUIStyle)"ToolbarPopup", GUILayout.Width(60.0f));
			GUILayout.Space(5);

			GUILayout.BeginVertical();
			GUILayout.Space(2);

			if (GUILayout.Toggle(GraphEditor.snapToGrid, new GUIContent(string.Empty, "Move nodes to the grid"), (GUIStyle)"GridToggle") != GraphEditor.snapToGrid) {
				GraphEditor.snapToGrid = !GraphEditor.snapToGrid;
			}
			GUILayout.EndVertical();
			GUILayout.Space(5);

			GUILayout.EndHorizontal();
		}

		public void DrawSelectBox() {
			if (GraphEditor.can_select && GraphEditor.is_select) {
				if (GraphEditor.mouse_down_position.x < GraphEditor.mouse_position.x) {
					if (GraphEditor.mouse_down_position.y < GraphEditor.mouse_position.y) {
						select_box = new Rect(GraphEditor.mouse_down_position.x, GraphEditor.mouse_down_position.y, GraphEditor.mouse_position.x - GraphEditor.mouse_down_position.x, GraphEditor.mouse_position.y - GraphEditor.mouse_down_position.y);
					}
					else {
						select_box = new Rect(GraphEditor.mouse_down_position.x, GraphEditor.mouse_position.y, GraphEditor.mouse_position.x - GraphEditor.mouse_down_position.x, GraphEditor.mouse_down_position.y - GraphEditor.mouse_position.y);
					}
				}
				else {
					if (GraphEditor.mouse_down_position.y < GraphEditor.mouse_position.y) {
						select_box = new Rect(GraphEditor.mouse_position.x, GraphEditor.mouse_down_position.y, GraphEditor.mouse_down_position.x - GraphEditor.mouse_position.x, GraphEditor.mouse_position.y - GraphEditor.mouse_down_position.y);
					}
					else {
						select_box = new Rect(GraphEditor.mouse_position.x, GraphEditor.mouse_position.y, GraphEditor.mouse_down_position.x - GraphEditor.mouse_position.x, GraphEditor.mouse_down_position.y - GraphEditor.mouse_position.y);
					}
				}
				GUI.Box(select_box, string.Empty, (GUIStyle)"SelectionRect");
			}
		}

		private void RefreshHoverElements() {
			GraphEditor.hover_node = null;
			GraphEditor.hover_port = null;
		}

		private void InputGUI() {
			Vector2 delta = current.delta;

			switch (current.type) {
				case EventType.MouseDown:
					GraphEditor.mouse_down_position = GraphEditor.mouse_position;
					switch (current.button) {
						case 0:
							if (current.alt) break;
							if (GraphEditor.hover_node) {
								if (current.clickCount >= 2) {
									if (GraphEditor.hover_node is MacroNode) {
										SetGraphAsset(((MacroNode)GraphEditor.hover_node).reference);
										current.Use();
										return;
									}
									else {
										EditorUtils.OpenScriptByType(GraphEditor.hover_node.type);
										current.Use();
									}
								}
								if (GraphEditor.hover_port) {
									if (current.control) {
										if (GraphEditor.hover_port is OutputAction) {
											UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} Add Branch", GraphEditor.asset.name));
											set_dirty = true;
											QuickPlugPort(GraphEditor.hover_port, AddNode<Branch>(GraphEditor.mouse_position / GraphEditor.zoom - GraphEditor.scroll + new Vector2(60.0f, 0.0f)));
										}
									}
									else {
										if (current.shift) {
											GraphEditor.can_drag_node = false;
											GraphEditor.can_drag_port = false;
											if (GraphEditor.hover_port is IPlug) {
												UndoManager.RecordObject(GraphEditor.asset, string.Format("{0} > {1}: {2} Unplug", GraphEditor.asset.name, GraphEditor.hover_node.name, GraphEditor.hover_port.name));
												set_dirty = true;
												((IPlug)GraphEditor.hover_port).Unplug();
											}
											current.Use();
										}
										else {
											GraphEditor.can_drag_node = false;
											GraphEditor.can_drag_port = true;
											SelectOnlyNode(GraphEditor.hover_node);
											current.Use();
										}
									}
								}
								else {
									if (current.shift) {
										if (GraphEditor.hover_node.is_selected) {
											GraphEditor.can_drag_node = false;
											DeselectNode(GraphEditor.hover_node);
											current.Use();
										}
										else {
											GraphEditor.can_drag_node = true;
											SelectNode(GraphEditor.hover_node);
											current.Use();
										}
									}
									else {
										if (GraphEditor.hover_node.is_selected) {
											GraphEditor.can_drag_node = true;
										}
										else {
											GraphEditor.can_drag_node = true;
											SelectOnlyNode(GraphEditor.hover_node);
											current.Use();
										}
									}
								}
							}
							else if (editable_area.Contains(GraphEditor.mouse_position)) {
								GraphEditor.can_select = true;
							}
							break;
						case 1:
							break;
						case 2:
							break;
					}
					break;
				case EventType.MouseUp:
					switch (current.button) {
						case 0:
							if (GraphEditor.can_select && GraphEditor.is_select) {
								Rect select_box_in_scroll = new Rect(select_box.position / target_zoom, select_box.size / target_zoom);
								List<Node> nodes = GraphEditor.graph.nodes.Where((n) => select_box_in_scroll.Overlaps(n.nodeRect)).ToList();
								if (current.shift) {
									foreach (Node node in nodes) {
										if (node.is_selected) {
											DeselectNode(node);
										}
										else {
											SelectNode(node);
										}
									}
									current.Use();
								}
								else {
									ClearSelection();
									SelectNodes(nodes);
									current.Use();
								}
							}
							else {
								if (GraphEditor.hover_node) {
									if (GraphEditor.is_drag) {
										if (GraphEditor.drag_port) {
											if (GraphEditor.hover_port) {
												PlugPort(GraphEditor.drag_port, GraphEditor.hover_port);
											}
											else {
												QuickPlugPort(GraphEditor.drag_port, GraphEditor.hover_node);
											}
											current.Use();
										}
									}
									else {
										if (!current.shift) {
											if (GraphEditor.selection.Count > 1) {
												SelectOnlyNode(GraphEditor.hover_node);
												current.Use();
											}
										}
									}
								}
								else if (!GraphEditor.is_scrolling) {
									ClearSelection();
								}
							}
							GraphEditor.can_select = false;
							GraphEditor.is_select = false;
							GraphEditor.is_scrolling = false;
							GraphEditor.is_drag = false;
							GraphEditor.hover_node = null;
							GraphEditor.drag_node = null;
							GraphEditor.drag_port = null;
							GraphEditor.can_drag_node = false;
							GraphEditor.can_drag_port = false;
							break;
						case 1:
							if (!GraphEditor.is_drag) {
								SearchWindow.Init(EditorGUIUtility.GUIToScreenPoint(GraphEditor.mouse_position), GraphEditor.mouse_position);
								/*generic_menu = new GenericMenu();
								foreach (Parameter param in GraphEditor.graph.inputParameters) {
									generic_menu.AddItem(new GUIContent(string.Format("Local Variables/{0} : {1}", param.name, param.valueType.GetTypeName())), false, (object obj) => {
										AddCustomNode(typeof(VariableNode<>).MakeGenericType(param.valueType), (Vector2)obj, param);
									}, GraphEditor.mouse_position - GraphEditor.scroll);
								}
								generic_menu.AddSeparator("Local Variables/");
								foreach (Type type in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
									if (typeof(Component).IsAssignableFrom(type) && !(type.IsAbstract & type.IsGenericType)) {
										string typeName = type.GetTypeName(true);
										if (typeName.Contains("UnityEngine")) {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/GameObject/GetComponent<T>/UnityEngine/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(GameObjectGetComponent<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);

											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/GetComponent<T>/UnityEngine/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(CurrentGetComponent<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
										else if (typeName.Contains("FastPlay")) {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/GameObject/GetComponent<T>/FastPlay/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(GameObjectGetComponent<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);

											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/GetComponent<T>/FastPlay/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(CurrentGetComponent<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
									}
								}
								//generic_menu.AddSeparator("Utilities/Actions/GetOwner<T>/");
								//generic_menu.AddSeparator("Actions/GameObject/GetComponent<T>/");
								foreach (Type type in typeof(Node).Assembly.GetTypes()) {
									if (typeof(Component).IsAssignableFrom(type) && !(type.IsAbstract & type.IsGenericType)) {
										generic_menu.AddItem(new GUIContent(string.Format("Actions/GameObject/GetComponent<T>/References/{0}", type.GetTypeName())), false, (object obj) => {
											AddNode(typeof(GameObjectGetComponent<>).MakeGenericType(type), (Vector2)obj);
										}, GraphEditor.mouse_position - GraphEditor.scroll);

										generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/GetComponent<T>/References/{0}", type.GetTypeName())), false, (object obj) => {
											AddNode(typeof(CurrentGetComponent<>).MakeGenericType(type), (Vector2)obj);
										}, GraphEditor.mouse_position - GraphEditor.scroll);
									}
								}
								foreach (Type type in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
									if (typeof(Component).IsAssignableFrom(type) || !(type.IsAbstract & type.IsGenericType)) {
										string typeName = type.GetTypeName(true);
										if (typeName.Contains("System")) {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/VariableObject/GetSet<T>/System/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(VariableGetSet<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/FindVariable<T>/System/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(CurrentFindVariable<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
										else if (typeName.Contains("FastPlay")) {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/VariableObject/GetSet<T>/FastPlay/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(VariableGetSet<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/FindVariable<T>/FastPlay/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(CurrentFindVariable<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
										else if (typeName.Contains("UnityEngine")) {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/VariableObject/GetSet<T>/UnityEngine/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(VariableGetSet<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/FindVariable<T>/UnityEngine/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(CurrentFindVariable<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
										else {
											generic_menu.AddItem(new GUIContent(string.Format("Actions/VariableObject/GetSet<T>/References/{0}", type.GetTypeName())), false, (object obj) => {
												AddNode(typeof(VariableGetSet<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
											generic_menu.AddItem(new GUIContent(string.Format("Actions/Current/FindVariable<T>/References/{0}", type.GetTypeName(true).Replace(".", "/"))), false, (object obj) => {
												AddNode(typeof(CurrentFindVariable<>).MakeGenericType(type), (Vector2)obj);
											}, GraphEditor.mouse_position - GraphEditor.scroll);
										}
									}
								}
								foreach (Type type in typeof(Node).Assembly.GetTypes()) {
									if (typeof(Node).IsAssignableFrom(type) && !type.IsAbstract) {
										if (type.HasAttribute<HideInListAttribute>(false)) continue;
										string path = type.GetTypeName();
										PathAttribute path_attribute = type.GetAttribute<PathAttribute>(false);

										if (type.IsGenericType) {
											foreach (Type t in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
												Type type_gen = type.MakeGenericType(t);
												if (path_attribute == null) {
													if (typeof(ActionNode).IsAssignableFrom(type_gen) || typeof(ValueNode).IsAssignableFrom(type_gen)) {
														path = "Actions/" + type_gen.GetTypeName();
													}
													else if (typeof(EventNode).IsAssignableFrom(type_gen)) {
														path = "Events/" + type_gen.GetTypeName();
													}
													else {
														path = "Others/" + type_gen.GetTypeName();
													}
												}
												else {
													path = string.Format("{0}/{1}", path_attribute.path, type_gen.GetTypeName(false, true));
												}
												if (type_gen.HasAttribute<BuiltInNodeAttribute>(false)) {
													generic_menu.AddItem(new GUIContent(path), false, (object obj) => {
														AddNode(type_gen, (Vector2)obj);
													}, GraphEditor.mouse_position - GraphEditor.scroll);
												}
												else {
													generic_menu.AddItem(new GUIContent("References/" + path), false, (object obj) => {
														AddNode(type_gen, (Vector2)obj);
													}, GraphEditor.mouse_position - GraphEditor.scroll);
												}
											}
										}
										else {
											if (path_attribute == null) {
												if (typeof(ActionNode).IsAssignableFrom(type) || typeof(ValueNode).IsAssignableFrom(type)) {
													path = "Actions/" + type.GetTypeName();
												}
												else if (typeof(EventNode).IsAssignableFrom(type)) {
													path = "Events/" + type.GetTypeName();
												}
												else {
													path = "Others/" + type.GetTypeName();
												}
											}
											else {
												path = path_attribute.path;
											}
											if (type.HasAttribute<BuiltInNodeAttribute>(false)) {
												generic_menu.AddItem(new GUIContent(path), false, (object obj) => {
													AddNode(type, (Vector2)obj);
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
											else {
												generic_menu.AddItem(new GUIContent("References/" + path), false, (object obj) => {
													AddNode(type, (Vector2)obj);
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
										}
									}
								}
								foreach (Type type in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
									bool separated = false;
									if (type.IsGenericType) {
										foreach (Type t in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
											Type type_gen = type.MakeGenericType(t);
											MethodInfo[] methods = type_gen.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
											//methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)
											//methods.Where(m => m.IsSpecialName)
											//methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)
											foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type_gen)) {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/{1}/Inherited/{2}", type.GetTypeName(), type_gen.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
											foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/{1}/Properties/{2}", type.GetTypeName(), type_gen.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
											if (!separated) {
												generic_menu.AddSeparator(string.Format("Functions/Reflected/{0}/{1}/", type.GetTypeName(), type_gen.GetTypeName()));
												separated = true;
											}
											foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type_gen)) {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/{1}/{2}", type.GetTypeName(), type_gen.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
										}
									}
									else {
										MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
										//methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)
										//methods.Where(m => m.IsSpecialName)
										//methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)
										foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)) {
											if (method.IsGenericMethod) {
												foreach (Type t in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
													MethodInfo method_gen = method.MakeGenericMethod(t);
													generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/Inherited/{1}/{2}", type.GetTypeName(), method.GetSignName(), method_gen.GetSignName())), false, (object obj) => {
														ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
														instance.SetMethod(method_gen);
														instance.Validate();
														instance.OnGraphAdd();
													}, GraphEditor.mouse_position - GraphEditor.scroll);
												}
											}
											else {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/Inherited/{1}", type.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
										}
										foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
											if (method.IsGenericMethod) {
												foreach (Type t in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
													MethodInfo method_gen = method.MakeGenericMethod(t);
													generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/Properties/{1}/{2}", type.GetTypeName(), method.GetSignName(), method_gen.GetSignName())), false, (object obj) => {
														ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
														instance.SetMethod(method_gen);
														instance.Validate();
														instance.OnGraphAdd();
													}, GraphEditor.mouse_position - GraphEditor.scroll);
												}
											}
											else {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/Properties/{1}", type.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
										}
										if (!separated) {
											generic_menu.AddSeparator(string.Format("Functions/Reflected/{0}/", type.GetTypeName()));
											separated = true;
										}
										foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)) {
											if (method.IsGenericMethod) {
												foreach (Type t in EditorHandler.GetConstantTypesCurrentInstance().current_types) {
													MethodInfo method_gen = method.MakeGenericMethod(t);
													generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/{1}/{2}", type.GetTypeName(), method.GetSignName(), method_gen.GetSignName())), false, (object obj) => {
														ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
														instance.SetMethod(method_gen);
														instance.Validate();
														instance.OnGraphAdd();
													}, GraphEditor.mouse_position - GraphEditor.scroll);
												}
											}
											else {
												generic_menu.AddItem(new GUIContent(string.Format("Functions/Reflected/{0}/{1}", type.GetTypeName(), method.GetSignName())), false, (object obj) => {
													ReflectedNode instance = AddNode<ReflectedNode>((Vector2)obj, false);
													instance.SetMethod(method);
													instance.Validate();
													instance.OnGraphAdd();
												}, GraphEditor.mouse_position - GraphEditor.scroll);
											}
										}
									}
								}
								generic_menu.DropDown(GraphEditor.UnZoomedRect(new Rect(GraphEditor.mouse_position, Vector2.zero)));
								current.Use();*/
							}
							GraphEditor.is_drag = false;
							break;
						case 2:
							break;
					}
					break;
				case EventType.MouseDrag:
					switch (current.button) {
						case 0:
							if (current.alt) {
								GraphEditor.is_drag = true;
								GraphEditor.is_scrolling = true;
								GraphEditor.scroll += delta;
								break;
							}
							if (!GraphEditor.is_drag) {
								if (GraphEditor.can_drag_port && GraphEditor.hover_port) {
									GraphEditor.is_drag = true;
									GraphEditor.can_drag_node = false;
									GraphEditor.drag_port = GraphEditor.hover_port;
								}
								if (GraphEditor.can_drag_node && GraphEditor.hover_node) {
									GraphEditor.is_drag = true;
									GraphEditor.can_drag_port = false;
									GraphEditor.drag_node = GraphEditor.hover_node;
								}
							}
							// ...
							if (GraphEditor.is_drag) {
								if (GraphEditor.drag_node) {
									foreach (Node node in GraphEditor.selection) {
										node.position += delta;
									}
								}
							}
							else {
								if (GraphEditor.can_select) {
									GraphEditor.is_select = true;
								}
							}
							break;
						case 1:
							GraphEditor.is_drag = true;
							GraphEditor.is_scrolling = true;
							GraphEditor.scroll += delta;
							break;
						case 2:
							GraphEditor.is_drag = true;
							GraphEditor.is_scrolling = true;
							GraphEditor.scroll += delta;
							break;
					}
					break;
				case EventType.ScrollWheel:
					if (!GraphEditor.is_drag) {
						float zoom_delta = 0.1f;
						zoom_delta = current.delta.y < 0.0f ? zoom_delta : -zoom_delta;
						target_zoom += zoom_delta;
						target_zoom = Mathf.Clamp(target_zoom, 0.2f, 1.0f);
					}
					break;
				case EventType.ValidateCommand:
					switch (Event.current.commandName) {
						case ("Delete"):
							DeleteSelectedNodes();
							current.Use();
							break;
						case ("Duplicate"):
							DuplicateSelectedNodes();
							current.Use();
							break;
						case ("SelectAll"):
							if (GraphEditor.selection.Count == GraphEditor.graph.nodeCount) {
								ClearSelection();
								current.Use();
							}
							else {
								SelectNodes(GraphEditor.graph.nodes);
								current.Use();
							}
							break;
					}
					break;
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (DragAndDrop.objectReferences.Length > 0) {
						if (DragAndDrop.objectReferences[0] is GraphAsset) {
							DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
							if (current.type == EventType.DragPerform) {
								DragAndDrop.AcceptDrag();
								GraphAsset m_asset = DragAndDrop.objectReferences.FirstOrDefault(r => r is GraphAsset) as GraphAsset;
								if (m_asset) {
									GenericMenu generic_menu = new GenericMenu();
									generic_menu.AddItem(new GUIContent("Add Graph"), false, (obj) => {
										AddMacro(m_asset, (Vector2)obj);
									}, GraphEditor.mouse_position - GraphEditor.scroll);
									generic_menu.ShowAsContext();
								}
								DragAndDrop.PrepareStartDrag();
							}
						}
						else {
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}
					}
					break;
				case (EventType.KeyDown):
					switch (current.keyCode) {
						case KeyCode.Delete:
							DeleteSelectedNodes();
							current.Use();
							break;
						case KeyCode.Home:
							target_zoom = 1.0f;
							GraphEditor.scroll = Vector2.zero;
							current.Use();
							break;
						case KeyCode.PageDown:
							GraphEditor.scroll -= new Vector2(0.0f, position.height);
							current.Use();
							break;
						case KeyCode.PageUp:
							GraphEditor.scroll += new Vector2(0.0f, position.height);
							current.Use();
							break;
					}
					break;
			}
		}

		public void RepaintInfo() {
		}

		private void DrawNodes() {
			foreach (Node node in GraphEditor.graph.nodes) {
				node.EDITOR_Update();

				// optimization
				node.is_occluded = !view_area.Overlaps(node.nodeRect);

				if (GraphEditor.ZoomedRect(node.nodeRect).Contains(GraphEditor.mouse_position)) {
					GraphEditor.hover_node = node;
				}

				node.EDITOR_DrawNode();
			}
		}

		public void BeginZoomArea() {
			GUI.EndGroup();
			matrix = GUI.matrix;

			Matrix4x4 scale = Matrix4x4.Scale(new Vector3(GraphEditor.zoom, GraphEditor.zoom, 1.0f));
			GUI.matrix = scale;

			Rect zoom_area_rect = new Rect(zoom_area.position / GraphEditor.zoom, zoom_area.size / GraphEditor.zoom);
			GUI.BeginGroup(zoom_area_rect);
		}

		public void EndZoomArea() {
			GUI.EndGroup();
			GUI.matrix = matrix;
			if (isDocked) {
				GUI.BeginGroup(new Rect(2.0f, 19.0f, Screen.width, Screen.height));
			}
			else {
				GUI.BeginGroup(new Rect(0.0f, 22.0f, Screen.width, Screen.height));
			}
		}

		private void DrawWindowBorder() {
			GUI.Box(editable_area, string.Empty, styles.border);
		}

		private void DrawWindowHeader() {
			Rect title_rect = new Rect(0.0f, 17.0f, position.width, 40.0f);
			EditorGUI.DrawRect(title_rect, new Color(0.0f, 0.0f, 0.0f, 0.5f));
			if (GraphEditor.graph && GraphEditor.asset) {
				string title = GraphEditor.asset.title.IsNullOrEmpty() ? GraphEditor.asset.name : GraphEditor.asset.title;
				if (GraphEditor.asset.isInstance) {
					GUI.Label(title_rect, new GUIContent(string.Format("{0} <size=12><color=#00FF56FF>(Instance)</color></size>", title)), styles.header);
				}
				else {
					GUI.Label(title_rect, new GUIContent(string.Format("{0} <size=12><color=#FF4C4CFF>(Asset)</color></size>", title)), styles.header);
				}
				if (GraphEditor.graph.graph != GraphEditor.graph) {
					Rect back_rect = new Rect(0.0f, 17.0f, 40.0f, 40.0f);
					if (GUI.Button(back_rect, new GUIContent("<<"), styles.header)) {
						SetGraph(GraphEditor.graph.graph);
					}
				}
			}
			else {
				GUI.Label(title_rect, new GUIContent("<color=#FF4C4CFF>NULL</color>"), styles.header);
			}
			GUI.Label(new Rect(position.width - 130.0f, 17.0f, 130.0f, 40.0f), new GUIContent(target_zoom.ToString("F1") + " | 1.0", "Zoom Factor"), styles.header);
		}

		private void DrawScrollBackground() {
			if (background == null) {
				background = EditorUtils.FindAssetByName<Texture2D>("background_dark");
				if (background == null) {
					Debug.LogError("(INTERNAL_ERROR) The GraphEditorWindow 'background' is NULL!");
					internal_error = true;
					return;
				}
			}
			float width = background.width * GraphEditor.zoom;
			float height = background.height * GraphEditor.zoom;
			float grid_size = GraphEditor.zoom > 0.51 ? 240 : GraphEditor.zoom > 0.3f && GraphEditor.zoom <= 0.51f ? 480 : 960;
			Vector2 offset = new Vector2(GraphEditor.scroll.x % grid_size - grid_size, GraphEditor.scroll.y % grid_size - grid_size);
			int tileX = Mathf.CeilToInt((position.width + (width - offset.x)) / width);
			int tileY = Mathf.CeilToInt((position.height + (height - offset.y)) / height);

			for (float x = 0; x < tileX; x++) {
				for (float y = 0; y < tileY; y++) {
					var backRect = new Rect(offset.x + x * grid_size, offset.y + y * grid_size, grid_size, grid_size);
					GUI.DrawTexture(backRect, background);
				}
			}
		}

		private void ClearSelection() {
			GraphEditor.ClearSelection();
		}

		private void SelectNode(int id, bool move = true) {
			GraphEditor.SelectNode(id, move);
		}

		private void SelectNode(Node node, bool move = true) {
			GraphEditor.SelectNode(node, move);
		}

		private void SelectOnlyNode(int id, bool move = true) {
			GraphEditor.SelectOnlyNode(id, move);
		}

		private void SelectOnlyNode(Node node, bool move = true) {
			GraphEditor.SelectOnlyNode(node, move);
		}

		private void DeselectNode(int id, bool move = true) {
			GraphEditor.DeselectNode(id, move);
		}

		private void DeselectNode(Node node, bool move = true) {
			GraphEditor.DeselectNode(node, move);
		}

		private void DeselectNodes(List<int> ids, bool move = true) {
			GraphEditor.DeselectNodes(ids, move);
		}

		private void DeselectNodes(List<Node> nodes, bool move = true) {
			GraphEditor.DeselectNodes(nodes, move);
		}

		private void SelectNodes(List<int> ids, bool move = true) {
			GraphEditor.SelectNodes(ids, move);
		}

		private void SelectNodes(List<Node> nodes, bool move = true) {
			GraphEditor.SelectNodes(nodes, move);
		}

		private void DuplicateSelectedNodes() {
			set_dirty = true;
			GraphEditor.DuplicateSelectedNodes();
		}

		private void DeleteSelectedNodes() {
			set_dirty = true;
			GraphEditor.DeleteSelectedNodes();
		}

		private void PlugPort(Port a, Port b) {
			set_dirty = true;
			GraphEditor.PlugPort(a, b);
		}

		private void QuickPlugPort(Port port, Node node) {
			set_dirty = true;
			GraphEditor.QuickPlugPort(port, node);
		}

		private void UnplugPort(Port port) {
			set_dirty = true;
			GraphEditor.UnplugPort(port);
		}

		private void DeleteNode(Node node) {
			set_dirty = true;
			GraphEditor.DeleteNode(node);
		}
	}
}
#endif
