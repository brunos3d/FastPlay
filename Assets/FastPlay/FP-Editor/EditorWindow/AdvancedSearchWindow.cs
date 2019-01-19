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

		private static bool keyboard_navigation;

		private static bool keyboard_pressed;

		private static bool drag_scroll;

		private static float scroll_pos;

		private static int view_element_capacity;

		public static int selected_index;

		public static string search;

		public TreeNode<Act> root_tree;

		public TreeNode<Act> current_tree;

		public TreeNode<Act> selected_node;

		public static string searchFormat {
			get {
				if (search.IsNullOrEmpty()) {
					return search;
				}
				else {
					string search_format = search.Replace("#node:", string.Empty);
					search_format = search_format.Replace("#path:", string.Empty); 
					search_format = search_format.Replace("#in:", string.Empty);
					return search_format;
				}
			}
		}

		public static bool hasSearch {
			get {
				return !searchFormat.IsNullOrEmpty();
			}
		}

		public static void Init() {
			AdvancedSearchWindow window = AdvancedSearchWindow.CreateInstance<AdvancedSearchWindow>();
			window.wantsMouseMove = true;
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

			GUI.Box(new Rect(0.0f, 0.0f, base.position.width, base.position.height), GUIContent.none, styles.background);

			view_element_capacity = (int)((position.height - WINDOW_HEAD_HEIGHT) / ELEMENT_LIST_HEIGHT);

			KeyboardInputGUI();

			//Search Bar
			GUI.SetNextControlName("GUIControlSearchBoxTextField");
			string new_search = EditorGUI.TextField(styles.search_rect, search, styles.search_bar);

			if (keyboard_pressed) {
				if (!keyboard_navigation) {
					EditorGUI.FocusTextInControl("GUIControlSearchBoxTextField");
				}
			}

			if (new_search != search) {
				search = new_search;
				if (new_search.IsNullOrEmpty()) {
					GoToNode(root_tree, false);
				}
				else {
					TreeNode<Act> search_result = current_tree;
					if (new_search == "*") {
						search_result = root_tree.GetAllTreeNodeInChildren();
					}
					else if (new_search.Contains("#node:")) {
						search_result = root_tree.GetTreeNodeInChildren(tn => tn.isLeaf && (tn.content.text == searchFormat || tn.content.tooltip == searchFormat || tn.contentName.ToLower().Contains(searchFormat.ToLower())));
					}
					else if (new_search.Contains("#path:")) {
						search_result = root_tree.GetTreeNodeInChildren(tn => !tn.isLeaf && (tn.content.text == searchFormat || tn.content.tooltip == searchFormat || tn.contentName.ToLower().Contains(searchFormat.ToLower())));
					}
					else if (new_search.Contains("#in:")) {
						search_result = root_tree.GetTreeNodeInChildren(tn => tn.isLeaf && tn.parent!= null && (tn.parent.content.text == searchFormat || tn.parent.content.tooltip == searchFormat || tn.parent.contentName.ToLower().Contains(searchFormat.ToLower())));
					}
					else {
						search_result = root_tree.GetTreeNodeInChildren(tn => tn.content.text == new_search || tn.content.tooltip == new_search || tn.contentName.ToLower().Contains(new_search.ToLower()));
					}
					GoToNode(search_result, false);
				}
			}

			GUI.DrawTexture(styles.search_icon_rect, styles.search_icon);

			GUILayout.Space(50.0f);

			if (!current_tree.isRoot && GUILayout.Button(current_tree.parent.content)) {
				GoToParent();
			}

			int current_tree_count = current_tree.Count;

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

				//Selection Box
				if (selected_index == draw_index + first_scroll_index || (Event.current.type == EventType.MouseMove && layout_rect.Contains(Event.current.mousePosition))) {
					selected_node = node;
					selected_index = draw_index + first_scroll_index;
					EditorGUI.DrawRect(layout_rect, FOCUS_COLOR);
				}

				//Draw TreeNode Button
				if (DrawElementList(layout_rect, node.content)) {
					GoToNode(node, true);
					break;
				}
				draw_index++;
			}

			PostInputGUI();

			Repaint();
		}

		void GoToParent() {
			if (!current_tree.isRoot) {
				selected_index = 0;
				current_tree = current_tree.parent;
				scroll_pos = 0.0f;
				RecalculateSize();
			}
		}

		void GoToNode(TreeNode<Act> node, bool call_if_is_leaf) {
			if (node.isLeaf) {
				if (call_if_is_leaf) {
					node.data();
					this.Close();
				}
			}
			else {
				current_tree = node;
				selected_index = 0;
				scroll_pos = 0.0f;
				RecalculateSize();
			}
		}

		void KeyboardInputGUI() {
			Event current = Event.current;

			keyboard_pressed = false;
			keyboard_navigation = false;
			switch (current.type) {
				case EventType.KeyDown:
					keyboard_pressed = true;

					if (current.keyCode == KeyCode.Home) {
						selected_index = 0;
						scroll_pos = 0.0f;
						current.Use();
						keyboard_navigation = true;
					}
					if (current.keyCode == KeyCode.End) {
						selected_index = current_tree.Count - 1;
						scroll_pos = current_tree.Count;
						current.Use();
						keyboard_navigation = true;
					}
					if (current.keyCode == KeyCode.DownArrow) {
						selected_index++;
						if (selected_index > scroll_pos) {
							scroll_pos++;
						}
						if (selected_index >= current_tree.Count) {
							selected_index = 0;
							scroll_pos = 0.0f;
						}
						current.Use();
						keyboard_navigation = true;
					}
					if (current.keyCode == KeyCode.UpArrow) {
						selected_index--;
						if (selected_index < scroll_pos - view_element_capacity) {
							scroll_pos--;
						}
						if (selected_index < 0) {
							selected_index = current_tree.Count - 1;
							scroll_pos = current_tree.Count;
						}
						current.Use();
						keyboard_navigation = true;
					}
					if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter)) {
						this.GoToNode(selected_node, true);
						current.Use();
						keyboard_navigation = true;
					}
					if (!hasSearch) {
						if ((current.keyCode == KeyCode.LeftArrow) || (current.keyCode == KeyCode.Backspace)) {
							this.GoToParent();
							current.Use();
							keyboard_navigation = true;
						}
						if (current.keyCode == KeyCode.RightArrow) {
							this.GoToNode(selected_node, false);
							current.Use();
							keyboard_navigation = true;
						}
					}

					break;
			}
		}

		void PreInputGUI() {
			Event current = Event.current;

			switch (current.type) {
				case EventType.MouseDown:
					drag_scroll = false;
					break;
				case EventType.ScrollWheel:
					drag_scroll = true;
					scroll_pos += current.delta.y;
					current.Use();
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

			GUI.Label(icon_rect, content.image);
			if (hasSearch) {
				string title = content.text.Replace(searchFormat, string.Format("<color=#ffff00ff><b>{0}</b></color>", searchFormat));
				string subtitle = content.tooltip.Replace(searchFormat, string.Format("<color=#ffff00ff><b>{0}</b></color>", searchFormat));

				GUI.Label(title_rect, title, styles.search_item);
				GUI.Label(subtitle_rect, subtitle, styles.search_description_item);
			}
			else {
				GUI.Label(title_rect, content.text, styles.search_item);
				GUI.Label(subtitle_rect, content.tooltip, styles.search_description_item);
			}

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

				object[] args = new object[] { typeof(VariableNode<>).MakeGenericType(param.valueType), param };
				root_tree.AddChildByPath(new GUIContent(string.Format("Local Variables/{0} : {1}", param.name, param.valueType.GetTypeName(true)), icon), () => { AddCustomNode(args); });
			}

			foreach (Type type in built_in_nodes) {
				Texture icon = icons[type];
				string path = string.Empty;
				string type_name = type.GetTypeName();
				string type_descritpion = type.GetDescription();
				PathAttribute path_attribute = type.GetAttribute<PathAttribute>(false);

				if (type.IsGenericType) {
					foreach (Type t in current_types) {
						Type type_gen = type.MakeGenericType(t);
						string type_gen_name = type_gen.GetTypeName();
						if (path_attribute == null) {
							if (typeof(ActionNode).IsAssignableFrom(type_gen) || typeof(ValueNode).IsAssignableFrom(type_gen)) {
								path = "Actions/" + type_gen_name;
							}
							else if (typeof(EventNode).IsAssignableFrom(type_gen)) {
								path = "Events/" + type_gen_name;
							}
							else {
								path = "Others/" + type_gen_name;
							}
						}
						else {
							path = string.Format("{0}/{1}", path_attribute.path, type_gen.GetTypeName(false, true));
						}
						if (type_gen.HasAttribute<BuiltInNodeAttribute>(false)) {
							root_tree.AddChildByPath(new GUIContent(path, icon, type_descritpion), () => { AddNode(type); });
						}
						else {
							root_tree.AddChildByPath(new GUIContent("References/" + path, icon, type_descritpion), () => { AddNode(type); });
						}
					}
				}
				else {
					if (path_attribute == null) {
						if (typeof(ActionNode).IsAssignableFrom(type) || typeof(ValueNode).IsAssignableFrom(type)) {
							path = "Actions/" + type_name;
						}
						else if (typeof(EventNode).IsAssignableFrom(type)) {
							path = "Events/" + type_name;
						}
						else {
							path = "Others/" + type_name;
						}
					}
					else {
						path = path_attribute.path;
					}
					if (type.HasAttribute<BuiltInNodeAttribute>(false)) {
						root_tree.AddChildByPath(new GUIContent(path, icon, type_descritpion), () => { AddNode(type); });
					}
					else {
						root_tree.AddChildByPath(new GUIContent("References/" + path, icon, type_descritpion), () => { AddNode(type); });
					}
				}
			}

			TreeNode<Act> codebase = root_tree.AddChild(new GUIContent("Codebase"), null);

			//reflected nodes
			foreach (Type type in current_types) {
				Texture icon = icons[type];
				string type_name = type.GetTypeName(false, true);
				string namespace_path = type.Namespace.IsNullOrEmpty() ? "Global" : type.Namespace;

				TreeNode<Act> namespace_tree = codebase.AddChild(new GUIContent(namespace_path), null);
				TreeNode<Act> type_tree = namespace_tree.AddChild(new GUIContent(type_name, icon, type.GetTypeName(true)), null);

				if (type.IsGenericType) {
					foreach (Type t in current_types) {
						Type type_gen = type.MakeGenericType(t);
						string type_gen_name = type_gen.GetTypeName();

						MethodInfo[] methods = type_gen.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type_gen)) {
							type_tree.AddChildByPath(new GUIContent(string.Format("{0}/Inherited/{1}", type_gen_name, method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
							type_tree.AddChildByPath(new GUIContent(string.Format("{0}/Properties/{1}", type_gen_name, method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}

						//Literal Nodes
						if (!type_gen.IsStatic()) {
							Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type_gen);
							type_tree.AddChildByPath(new GUIContent(string.Format("{0}/Literal {0}", type_gen_name), icon), () => { AddNode(literal_node_type); });
						}

						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type_gen)) {
							type_tree.AddChildByPath(new GUIContent(string.Format("{0}/{1}", type_gen_name, method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}
				}
				else {
					MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)) {
						TreeNode<Act> generic_node = type_tree.AddChildByPath(new GUIContent(string.Format("Inherited/{0}", method.Name), icon, method.GetDescription()), null);
						if (method.IsGenericMethod) {
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								generic_node.AddChildByPath(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							type_tree.AddChildByPath(new GUIContent(string.Format("Inherited/{0}", method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}
					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
						if (method.IsGenericMethod) {
							TreeNode<Act> generic_node = type_tree.AddChildByPath(new GUIContent(string.Format("Properties/{0}", method.Name), icon, method.GetDescription()), null);
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								generic_node.AddChildByPath(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							type_tree.AddChildByPath(new GUIContent(string.Format("Properties/{0}", method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
						}
					}

					//Literal Nodes
					if (!type.IsStatic()) {
						Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type);
						type_tree.AddChildByPath(new GUIContent(string.Format("Literal {0}", type_name), icon), () => { AddNode(literal_node_type); });
					}

					foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)) {
						if (method.IsGenericMethod) {
							TreeNode<Act> generic_node = type_tree.AddChildByPath(new GUIContent(string.Format("{0}", method.Name), icon, method.GetDescription()), null);
							foreach (Type t in current_types) {
								MethodInfo method_gen = method.MakeGenericMethod(t);
								object[] args = new object[] { method_gen, t };
								generic_node.AddChild(new GUIContent(method_gen.Name, icon, method_gen.GetDescription()), () => { AddReflectedGenericNode(args); });
							}
						}
						else {
							type_tree.AddChildByPath(new GUIContent(string.Format("{0}", method.Name), icon, method.GetDescription()), () => { AddReflectedNode(method); });
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
