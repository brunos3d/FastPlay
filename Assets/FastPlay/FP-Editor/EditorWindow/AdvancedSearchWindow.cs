#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using FastPlay.Runtime;
using System.Reflection;
using System.Linq;

namespace FastPlay.Editor {
	public class AdvancedSearchWindow : EditorWindow {

		private class Styles {

			public readonly Rect search_rect;

			public readonly Rect search_icon_rect;

			public readonly Texture search_icon;

			public readonly GUIStyle label = (GUIStyle)"label";

			public readonly GUIStyle background = (GUIStyle)"grey_border";

			public readonly GUIStyle search_bar;

			public readonly GUIStyle search_item;

			public readonly GUIStyle search_description_item;

			public Styles(float width) {
				search_bar = FPSkin.GetStyle("Search Bar");
				search_item = FPSkin.GetStyle("Search Item");
				search_description_item = FPSkin.GetStyle("Search Description");
				search_icon = EditorGUIUtility.FindTexture("Search Icon");
				search_rect = new Rect(15.0f, 10.0f, width - 30.0f, 30.0f);
				search_icon_rect = new Rect(20.0f, 13.0f, 23.0f, 23.0f);
			}
		}

		private static readonly float WINDOW_HEAD_HEIGHT = 70.0f;

		private static readonly float ELEMENT_LIST_HEIGHT = 50.0f;

		private static readonly Color FOCUS_COLOR = new Color(62.0f / 255.0f, 95.0f / 255.0f, 150.0f / 255.0f);

		private static Styles styles;

		private static bool drag_scroll;

		public static string search;

		private static float scroll_pos;

		public TreeNode<Act> root_tree;

		public TreeNode<Act> current_tree;

		public static bool hasSearch {
			get {
				return !search.IsNullOrEmpty();
			}
		}

		[MenuItem("AdvancedSearchWindow/Show")]
		public static void Init() {
			AdvancedSearchWindow window = AdvancedSearchWindow.CreateInstance<AdvancedSearchWindow>();

			window.ShowPopup();
			//window.ShowAsDropDown(new Rect(pos, Vector2.zero), size);
			FocusWindowIfItsOpen<AdvancedSearchWindow>();
		}

		void OnEnable() {
			root_tree = new TreeNode<Act>(new GUIContent("Root"), null);

			CreateTreeNode(root_tree);

			GoToNode(root_tree, false);
		}

		public void RecalculateSize() {
			Vector2 pos = new Vector2(Screen.currentResolution.width / 4.0f, 120.0f);
			Vector2 size = new Vector2(Screen.currentResolution.width / 2.0f, Mathf.Min(70.0f + current_tree.Count * 54.0f, Screen.currentResolution.height - 200.0f));

			position = new Rect(pos, size);
		}

		void OnGUI() {
			if (focusedWindow != this || EditorApplication.isCompiling) {
				Close();
			}
			if (styles == null) {
				styles = new Styles(position.width);
			}
			if (styles.search_icon == null) {
				return;
			}

			GUI.Label(new Rect(0.0f, 0.0f, base.position.width, base.position.height), GUIContent.none, styles.background);

			//Search Bar
			string new_search = EditorGUI.TextField(styles.search_rect, search, styles.search_bar);

			if (new_search != search) {
				search = new_search;
				if (new_search.IsNullOrEmpty()) {
					GoToNode(root_tree, false);
				}
				else {
					GoToNode(root_tree.GetTreeNodeInChildren(tn => tn.content.text.ToLower().Contains(new_search.ToLower()) || tn.content.tooltip.ToLower().Contains(new_search.ToLower())), false);
				}
			}

			GUI.DrawTexture(styles.search_icon_rect, styles.search_icon);

			GUILayout.Space(50.0f);

			if (current_tree.parent != null && GUILayout.Button(current_tree.parent.content)) {
				GoToNode(current_tree.parent, false);
			}

			int current_tree_count = current_tree.Count;
			int view_element_capacity = (int)((position.height - WINDOW_HEAD_HEIGHT) / ELEMENT_LIST_HEIGHT);

			if (view_element_capacity < current_tree_count) {
				scroll_pos = GUI.VerticalScrollbar(new Rect(position.width - 17.0f, WINDOW_HEAD_HEIGHT, 20.0f, position.height - WINDOW_HEAD_HEIGHT - 1.0f), scroll_pos, 1.0f, view_element_capacity, current_tree_count);
			}
			else {
				scroll_pos = view_element_capacity;
			}

			scroll_pos = Mathf.Clamp(scroll_pos, 0.0f, current_tree_count);

			PreInputGUI();

			int first_scroll_index = (int)Mathf.Clamp(scroll_pos - view_element_capacity, 0, current_tree_count);
			int last_scroll_index = (int)Mathf.Clamp(scroll_pos + 1, 0, current_tree_count);

			GUI.Label(new Rect(position.width - 100.0f, WINDOW_HEAD_HEIGHT + Mathf.Lerp(0.0f, position.height - WINDOW_HEAD_HEIGHT - 20.0f, (scroll_pos / current_tree_count)), 100.0f, 20.0f), string.Format("{0}/{1}", scroll_pos, last_scroll_index - first_scroll_index));

			int draw_index = 0;
			for (int id = first_scroll_index; id < last_scroll_index; id++) {
				TreeNode<Act> node = current_tree[id];
				Rect layout_rect = new Rect(1.0f, WINDOW_HEAD_HEIGHT + draw_index * ELEMENT_LIST_HEIGHT, position.width - 22.0f, ELEMENT_LIST_HEIGHT);
				if (DrawElementList(layout_rect, node.content)) {
					GoToNode(node, true);
					break;
				}
				draw_index++;
			}

			PostInputGUI();

			Repaint();
		}

		void GoToNode(TreeNode<Act> node, bool call_if_is_leaf) {
			if (node.isLeaf && call_if_is_leaf) {
				node.data();
				this.Close();
			}
			else {
				current_tree = node;
				scroll_pos = 0.0f;
				RecalculateSize();
			}
			Repaint();
		}

		void PreInputGUI() {
			Event current = Event.current;

			switch (current.type) {
				case EventType.MouseDown:
					drag_scroll = false;
					break;
				case EventType.MouseDrag:
					drag_scroll = true;
					scroll_pos -= current.delta.y / ELEMENT_LIST_HEIGHT;
					current.Use();
					break;
			}
		}

		void PostInputGUI() {
			Event current = Event.current;

			switch (current.type) {
				case EventType.KeyDown:
					break;
				case EventType.MouseUp:
					drag_scroll = false;
					break;
			}
		}

		public bool DrawElementList(Rect rect, GUIContent content) {
			Rect layout_rect = new Rect(rect);
			bool trigger = GUI.Button(layout_rect, "", styles.label);

			Rect icon_rect = new Rect(layout_rect.x + 10.0f, layout_rect.y, layout_rect.height, layout_rect.height);
			Rect title_rect = new Rect(55.0f, layout_rect.y, layout_rect.width - 60.0f, layout_rect.height);
			Rect subtitle_rect = new Rect(title_rect);

			if (Event.current.type == EventType.Repaint && layout_rect.Contains(Event.current.mousePosition)) {
				EditorGUI.DrawRect(layout_rect, FOCUS_COLOR);
			}
			GUI.Label(icon_rect, content.image);
			GUI.Label(title_rect, content.text, styles.search_item);
			GUI.Label(subtitle_rect, content.tooltip, styles.search_description_item);

			return !drag_scroll && trigger;
		}

		void CreateTreeNode(TreeNode<Act> root_tree) {
			List<Parameter> var_parameters = GraphEditor.graph.variableParameters;
			List<Type> built_in_nodes = typeof(Node).Assembly.GetTypes().Where(t => typeof(Node).IsAssignableFrom(t) && !t.IsAbstract && !t.HasAttribute<HideInListAttribute>(false)).ToList();
			List<Type> current_types = EditorHandler.GetConstantTypesCurrentInstance().current_types;
			Dictionary<Type, Texture> icons = new Dictionary<Type, Texture>();

			var_parameters.ForEach(p => icons[p.valueType] = GUIReferrer.GetTypeIcon(p.valueType));
			current_types.ForEach(t => icons[t] = GUIReferrer.GetTypeIcon(t));
			built_in_nodes.ForEach(t => icons[t] = GUIReferrer.GetTypeIcon(t));

			foreach (Parameter param in var_parameters) {
				Texture icon = icons[param.valueType];

				TreeNode<Act> variables_tree;
				object[] args = new object[] { typeof(VariableNode<>).MakeGenericType(param.valueType), param };
				variables_tree = root_tree.AddChild(new GUIContent(string.Format("Local Variables")), null);
				variables_tree.AddChild(new GUIContent(string.Format("{0} : {1}", param.name, param.valueType.GetTypeName()), icon), () => { AddCustomNode(args); });
			}

			foreach (Type type in built_in_nodes) {
				Texture icon = icons[type];
				string type_name = type.GetTypeName();
				string summary = type.GetDescription();

				PathAttribute path_attribute = type.GetAttribute<PathAttribute>(false);
				TreeNode<Act> target_tree;

				if (type.IsGenericType) {
					foreach (Type t in current_types) {
						Type type_gen = type.MakeGenericType(t);
						string type_gen_name = type_gen.GetTypeName();
						if (path_attribute == null) {
							if (typeof(ActionNode).IsAssignableFrom(type_gen) || typeof(ValueNode).IsAssignableFrom(type_gen)) {
								target_tree = root_tree.AddChild(new GUIContent("Actions"), null);
							}
							else if (typeof(EventNode).IsAssignableFrom(type_gen)) {
								target_tree = root_tree.AddChild(new GUIContent("Events"), null);
							}
							else {
								target_tree = root_tree.AddChild(new GUIContent("Others"), null);
							}
							if (type.HasAttribute<BuiltInNodeAttribute>(false)) {
								target_tree.AddChild(new GUIContent(type_gen_name, icon), () => { AddNode(type_gen); });
							}
							else {
								TreeNode<Act> references_tree = target_tree.AddChild(new GUIContent("References"), null);
								references_tree.AddChild(new GUIContent(type_gen_name, icon), () => { AddNode(type_gen); });
							}
						}
						else {
							string path = string.Format("{0}/{1}", path_attribute.path, type_gen_name);
							root_tree.AddChildByPath(new GUIContent(path, icon, summary), () => { AddNode(type_gen); });
						}
					}
				}
				else {
					if (path_attribute == null) {
						if (typeof(ActionNode).IsAssignableFrom(type) || typeof(ValueNode).IsAssignableFrom(type)) {
							target_tree = root_tree.AddChild(new GUIContent("Actions"), null);
						}
						else if (typeof(EventNode).IsAssignableFrom(type)) {
							target_tree = root_tree.AddChild(new GUIContent("Events"), null);
						}
						else {
							target_tree = root_tree.AddChild(new GUIContent("Others"), null);
						}
						if (type.HasAttribute<BuiltInNodeAttribute>(false)) {
							target_tree.AddChild(new GUIContent(type_name, icon), () => { AddNode(type); });
						}
						else {
							TreeNode<Act> references_tree = target_tree.AddChild(new GUIContent("References"), null);
							references_tree.AddChild(new GUIContent(type_name, icon), () => { AddNode(type); });
						}
					}
					else {
						root_tree.AddChildByPath(new GUIContent(path_attribute.path, icon, summary), () => { AddNode(type); });
					}
				}
			}

			Dictionary<string, TreeNode<Act>> namespace_trees = new Dictionary<string, TreeNode<Act>>();

			TreeNode<Act> codebase_tree = root_tree.AddChild(new GUIContent("Codebase"), null);

			foreach (Type type in current_types) {
				TreeNode<Act> namespace_tree;
				if (!namespace_trees.TryGetValue(type.Namespace, out namespace_tree)) {
					namespace_tree = namespace_trees[type.Namespace] = codebase_tree.AddChild(new GUIContent(type.Namespace), null);
				}

				Texture icon = icons[type];
				string type_name = type.GetTypeName();

				TreeNode<Act> type_tree = namespace_tree.AddChild(new GUIContent(type_name, GUIReferrer.GetTypeIcon(type)), null);

				if (type.IsGenericType) {
					foreach (Type t in current_types) {
						Type type_gen = type.MakeGenericType(t);
						string type_gen_name = type_gen.GetTypeName();

						MethodInfo[] methods = type_gen.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();

						TreeNode<Act> inherit_tree = type_tree.AddChild(new GUIContent("Inherited", icon), null);
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type_gen)) {
							inherit_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => {
								AddReflectedNode(method);
							});
						}
						TreeNode<Act> property_tree = type_tree.AddChild(new GUIContent("Properties", icon), null);
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
							property_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}

						//Literal Nodes 
						if (!type_gen.IsStatic()) {
							Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type_gen);
							type_tree.AddChild(new GUIContent(string.Format("Literal {0}", type_gen_name), icon), () => { AddNode(literal_node_type); });
						}

						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type_gen)) {
							type_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}
				}
				else {
					MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();

					TreeNode<Act> inherit_tree = type_tree.AddChild(new GUIContent("Inherited", icon), null);
					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)) {
						if (method.IsGenericMethod) {
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								inherit_tree.AddChild(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							inherit_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}
					TreeNode<Act> property_tree = type_tree.AddChild(new GUIContent("Properties", icon), null);
					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
						if (method.IsGenericMethod) {
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								property_tree.AddChild(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							property_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}

					//Literal Nodes
					if (!type.IsStatic()) {
						Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type);
						type_tree.AddChild(new GUIContent(string.Format("Literal {0}", type_name), icon), () => { AddNode(literal_node_type); });
					}

					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)) {
						if (method.IsGenericMethod) {
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								type_tree.AddChild(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							type_tree.AddChild(new GUIContent(method.Name, icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}
				}
			}
		}

		public void AddNode(object obj) {
			GraphEditorWindow.AddNode((Type)obj, -GraphEditor.scroll);
		}

		public void AddCustomNode(object obj) {
			object[] args = (object[])obj;
			GraphEditorWindow.AddCustomNode((Type)args[0], -GraphEditor.scroll, true, args[1]);
		}

		public void AddReflectedNode(object obj) {
			GraphEditorWindow.AddCustomNode<ReflectedNode>(-GraphEditor.scroll, true, (MethodInfo)obj);
		}

		public void AddReflectedGenericNode(object obj) {
			object[] args = (object[])obj;
			GraphEditorWindow.AddCustomNode<ReflectedNode>(-GraphEditor.scroll, true, (MethodInfo)args[0], new Type[] { (Type)args[1] });
		}
	}
}
#endif
