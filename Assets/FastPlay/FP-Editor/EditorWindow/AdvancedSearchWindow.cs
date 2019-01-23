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

			public readonly Texture search_icon;

			public readonly GUIStyle label = EditorStyles.label;

			public readonly GUIStyle tag_button = EditorStyles.miniButton;

			public readonly GUIStyle background = (GUIStyle)"grey_border";

			public readonly GUIStyle search_bar;

			public readonly GUIStyle search_label;

			public readonly GUIStyle search_item;

			public readonly GUIStyle search_description_item;

			public readonly GUIStyle on_search_item;

			public readonly GUIStyle on_search_description_item;

			public Styles() {
				search_bar = FPSkin.GetStyle("Search Bar");
				search_label = FPSkin.GetStyle("Search Label");
				search_item = FPSkin.GetStyle("Search Item");
				search_description_item = FPSkin.GetStyle("Search Description");

				search_icon = EditorGUIUtility.FindTexture("Search Icon");

				on_search_item = new GUIStyle(FPSkin.GetStyle("Search Item"));
				on_search_description_item = new GUIStyle(FPSkin.GetStyle("Search Description"));

				on_search_item.normal = FPSkin.GetStyle("Search Item").onNormal;
				on_search_description_item.normal = FPSkin.GetStyle("Search Description").onNormal;

				on_search_item.hover = FPSkin.GetStyle("Search Item").onHover;
				on_search_description_item.hover = FPSkin.GetStyle("Search Description").onHover;
			}
		}

		private const float WINDOW_HEAD_HEIGHT = 80.0f;

		private const float WINDOW_FOOT_OFFSET = 15.0f;

		private const float ELEMENT_LIST_HEIGHT = 50.0f;

		private readonly string[] TIPS = new string[] {
			"FastPlay visual scripting...",
			@"Use search filters by typing ""#""!",
			"You can create Branchs by pressing CTRL + LMB on a port!",
			"Scroll the items in this window using Scroll or by dragging!",
			"Create nodes quickly by dragging Objects into the editor!",
			"Quickly connect one port to another by dropping it on the node!"
		};

		private readonly Color FOCUS_COLOR = new Color(62.0f / 255.0f, 95.0f / 255.0f, 150.0f / 255.0f);

		private static Styles styles;

		private bool text_navigation;

		private bool keyboard_navigation;

		private bool keyboard_pressed;

		private bool need_refocus;

		private bool select_all;

		private bool drag_scroll;

		private bool disable_layout;

		private float scroll_pos;

		public int selected_index;

		private int view_element_capacity;

		public string search;

		public string new_search;

		public string search_format;

		public string last_search_format;

		public Vector2 spawn_pos;

		private AutoTypeInstance auto_type;

		public TreeNode<Act> root_tree;

		public TreeNode<Act> current_tree;

		public TreeNode<Act> last_tree;

		public TreeNode<Act> selected_node;

		public readonly List<TreeNode<Act>> parents = new List<TreeNode<Act>>();

		public string searchFormat {
			get {
				if (search.IsNullOrEmpty()) {
					return search;
				}
				else {
					if (last_search_format != search) {
						last_search_format = search;
						search_format = search;
						search_format = search_format.Replace("#description:", string.Empty);
						search_format = search_format.Replace("#event:", string.Empty);
						search_format = search_format.Replace("#in:", string.Empty);
						search_format = search_format.Replace("#node:", string.Empty);
						search_format = search_format.Replace("#path:", string.Empty);
						search_format = search_format.Replace("#tag:", string.Empty);
						search_format = search_format.Replace("#title:", string.Empty);
						return search_format;
					}
					else {
						return search_format;
					}
				}
			}
		}

		public bool hasSearch {
			get {
				return !search.IsNullOrEmpty();
			}
		}

		public static void Init(Vector2 spawn_pos) {
			AdvancedSearchWindow window = AdvancedSearchWindow.CreateInstance<AdvancedSearchWindow>();
			window.spawn_pos = spawn_pos;
			window.wantsMouseMove = true;
			window.ShowPopup();
			//window.ShowAsDropDown(new Rect(pos, Vector2.zero), size);
			FocusWindowIfItsOpen<AdvancedSearchWindow>();
		}

		void OnEnable() {
			//Unity bug fix
			Undo.undoRedoPerformed += Close;
			if (!GraphEditor.graph) {
				Close();
				return;
			}
			auto_type = FindObjectOfType<AutoTypeInstance>() ?? CreateInstance<AutoTypeInstance>();
			auto_type.Init(6, 3.0f, false, TIPS);
			need_refocus = true;
			root_tree = new TreeNode<Act>(new GUIContent("Home"), null);
			CreateTreeNode(root_tree);
			GoToNode(root_tree, false);
		}

		void OnDisable() {
			//Unity bug fix
			Undo.undoRedoPerformed -= Close;
			DestroyImmediate(auto_type);
		}

		void OnGUI() {
			//Unity bug fix
			if (focusedWindow != this || !GraphEditor.graph || EditorApplication.isCompiling) {
				Close();
			}
			if (styles == null) {
				styles = new Styles();
			}
			if (styles.search_icon == null) {
				return;
			}

			disable_layout = false;

			GUI.Box(new Rect(0.0f, 0.0f, base.position.width, base.position.height), GUIContent.none, styles.background);

			view_element_capacity = (int)((position.height - (WINDOW_HEAD_HEIGHT + WINDOW_FOOT_OFFSET)) / ELEMENT_LIST_HEIGHT);

			KeyboardInputGUI();

			RefreshSearchStatus();

			Rect search_rect = new Rect(15.0f, 10.0f, position.width - 30.0f, 30.0f);
			Rect search_icon_rect = new Rect(20.0f, 13.0f, 23.0f, 23.0f);

			//Search Bar
			GUI.SetNextControlName("GUIControlSearchBoxTextField");
			search = GUI.TextField(search_rect, search, styles.search_bar);

			TextEditor txt = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			if (select_all) {
				txt.SelectAll();
				select_all = false;
			}
			if (need_refocus) {
				txt.OnFocus();
				txt.MoveLineEnd();
				txt.SelectNone();
				need_refocus = false;
			}

			if (auto_type) {
				if (!hasSearch) {
					if (!auto_type.isPlaying) {
						auto_type.Play();
					}
					GUI.Label(search_rect, auto_type.GetCurrentText(), styles.search_label);
				}
				else {
					auto_type.Stop();
				}
			}

			GUI.DrawTexture(search_icon_rect, styles.search_icon);

			if (keyboard_pressed) {
				if (!keyboard_navigation && !text_navigation) {
					need_refocus = true;
					GUI.FocusControl("GUIControlSearchBoxTextField");
				}
			}

			if (!disable_layout) {
				GUILayout.Space(50.0f);

				GUILayout.BeginHorizontal();
				{
					if (hasSearch && current_tree.content.text == "#Search") {
						if (GUILayout.Button(new GUIContent("Home"), GUILayout.ExpandWidth(false), GUILayout.Height(20.0f))) {
							GoToHome();
						}
					}
					for (int id = parents.Count - 1; id >= 0; id--) {
						TreeNode<Act> parent = parents[id];
						if (GUILayout.Button(parent.content.text, GUILayout.ExpandWidth(false), GUILayout.Height(20.0f))) {
							GoToNode(parent, true);
							break;
						}
					}
					GUILayout.Button(current_tree.content.text, GUILayout.ExpandWidth(false), GUILayout.Height(20.0f));
				}
				GUILayout.EndHorizontal();
			}

			int current_tree_count = current_tree.Count;

			if (view_element_capacity < current_tree_count) {
				scroll_pos = GUI.VerticalScrollbar(new Rect(position.width - 17.0f, WINDOW_HEAD_HEIGHT, 20.0f, view_element_capacity * ELEMENT_LIST_HEIGHT), scroll_pos, 1.0f, 0.0f, current_tree_count - view_element_capacity + 1);
			}
			else {
				scroll_pos = 0.0f;
			}

			scroll_pos = Mathf.Clamp(scroll_pos, 0.0f, current_tree_count);

			PreInputGUI();

			int first_scroll_index = (int)Mathf.Clamp(scroll_pos, 0, current_tree_count);
			int last_scroll_index = (int)Mathf.Clamp(scroll_pos + view_element_capacity, 0, current_tree_count);

			//Scroll debug info
			GUI.Label(new Rect(position.width - 100.0f, WINDOW_HEAD_HEIGHT + Mathf.Lerp(0.0f, position.height - WINDOW_HEAD_HEIGHT - 20.0f, (scroll_pos / current_tree_count)), 100.0f, 20.0f), string.Format("({0}) {1}/{2}", current_tree_count, scroll_pos, last_scroll_index - first_scroll_index));

			int draw_index = 0;
			for (int id = first_scroll_index; id < last_scroll_index; id++) {
				bool selected = false;
				TreeNode<Act> node = current_tree[id];
				Rect layout_rect = new Rect(1.0f, WINDOW_HEAD_HEIGHT + draw_index * ELEMENT_LIST_HEIGHT, position.width - 22.0f, ELEMENT_LIST_HEIGHT);

				//Draw Selection Box
				if (selected_index == draw_index + first_scroll_index || (Event.current.type == EventType.MouseMove && layout_rect.Contains(Event.current.mousePosition))) {
					selected = true;
					selected_node = node;
					selected_index = draw_index + first_scroll_index;
					EditorGUI.DrawRect(layout_rect, FOCUS_COLOR);
				}

				if (!disable_layout) {
					//Draw Tag Buttons
					GUILayout.BeginArea(new Rect(layout_rect.x, layout_rect.y + 5.0f, layout_rect.width, layout_rect.height));
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					foreach (string tag in node.tags) {
						if (GUILayout.Button(tag, styles.tag_button, GUILayout.ExpandWidth(false))) {
							search = "#tag:" + tag;
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.EndArea();
				}

				//Draw TreeNode Button
				if (DrawElementList(layout_rect, node.content, selected)) {
					GoToNode(node, true);
					break;
				}
				draw_index++;
			}
			PostInputGUI();
			Repaint();
		}

		void RefreshSearchStatus() {
			if (new_search != search) {
				new_search = search;
				if (new_search.IsNullOrEmpty()) {
					GoToNode(root_tree, false);
				}
				else {
					TreeNode<Act> search_result = root_tree;
					if (searchFormat.Contains("#")) {
						search_result = new TreeNode<Act>(new GUIContent("#Commands"), null);
						search_result.AddChild(new GUIContent("#description:", "Search results by description"), () => { search = "#description:"; });
						search_result.AddChild(new GUIContent("#event:", "Search results node events only"), () => { search = "#event:"; });
						search_result.AddChild(new GUIContent("#in:", "All items within the directory searched"), () => { search = "#in:"; });
						search_result.AddChild(new GUIContent("#node:", "Search results nodes only"), () => { search = "#node:"; });
						search_result.AddChild(new GUIContent("#path:", "Search results directories only"), () => { search = "#path:"; });
						search_result.AddChild(new GUIContent("#tag:", "Search results by tag"), () => { search = "#tag:"; });
						search_result.AddChild(new GUIContent("#title:", "Search results by title"), () => { search = "#title:"; });
						search_result = search_result.GetTreeNodeInAllChildren(tn => tn.content.text.ToLower().Contains(search.ToLower()));
					}
					else {
						if (new_search == "*") {
							search_result = root_tree.GetAllChildren();
						}
						else {
							search_result = search_result.GetTreeNodeInAllChildren(tn => tn.content.text == searchFormat || tn.content.tooltip == searchFormat || tn.contentName.ToLower().Contains(searchFormat.ToLower()));
						}

						if (new_search.Contains("#in:")) {
							search_result = root_tree.GetTreeNodeInAllChildren(tn => tn.parent != null && (tn.parent.content.text.ToLower().Contains(searchFormat.ToLower())));
						}
						else if (new_search.Contains("#event:")) {
							search_result = search_result.GetTreeNodeInAllChildren(tn => tn.tags.Contains("EventNode"));
						}
						else if (new_search.Contains("#node:")) {
							search_result = search_result.GetTreeNodeInAllChildren(tn => tn.isLeaf);
						}
						else if (new_search.Contains("#path:")) {
							search_result = search_result.GetTreeNodeInAllChildren(tn => !tn.isLeaf);
						}
						else if (new_search.Contains("#tag:")) {
							search_result = root_tree.GetTreeNodeInAllChildren(tn => tn.tags.Any(tag => tag.ToLower().Contains(searchFormat.ToLower())));
						}
						else if (new_search.Contains("#title:")) {
							search_result = root_tree.GetTreeNodeInAllChildren(tn => tn.content.text.ToLower().Contains(searchFormat.ToLower()));
						}
						else if (new_search.Contains("#description:")) {
							search_result = root_tree.GetTreeNodeInAllChildren(tn => tn.content.tooltip.ToLower().Contains(searchFormat.ToLower()));
						}
						if (searchFormat == "*") {
							search_result = search_result.GetAllChildren();
						}
					}

					GoToNode(search_result, false);
				}
			}
		}

		void KeyboardInputGUI() {
			Event current = Event.current;

			keyboard_pressed = false;
			switch (current.type) {
				case EventType.ValidateCommand:
					switch (Event.current.commandName) {
						case ("SelectAll"):
							select_all = true;
							text_navigation = true;
							keyboard_navigation = false;
							break;
					}
					break;
				case EventType.KeyDown:
					keyboard_pressed = true;
					if (!current.control) {
						char current_char = Event.current.character;
						if (char.IsNumber(current_char)) {
							if (keyboard_navigation || !hasSearch) {
								selected_index = (int)(scroll_pos + (char.GetNumericValue(current_char))) - 1;
								if (selected_index < 0) {
									selected_index = 0;
								}
								else if (selected_index >= current_tree.Count) {
									selected_index = current_tree.Count - 1;
									scroll_pos = current_tree.Count;
								}
								else if (selected_index >= scroll_pos + view_element_capacity) {
									scroll_pos += Mathf.Abs(selected_index - view_element_capacity);
								}
								current.Use();
								keyboard_navigation = true;
							}
						}
						else {
							if (current.keyCode == KeyCode.Home) {
								if (keyboard_navigation || !hasSearch) {
									selected_index = 0;
									scroll_pos = 0.0f;
									current.Use();
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else if (current.keyCode == KeyCode.End) {
								if (keyboard_navigation || !hasSearch) {
									selected_index = current_tree.Count - 1;
									scroll_pos = current_tree.Count;
									current.Use();
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else if (current.keyCode == KeyCode.PageDown) {
								if (keyboard_navigation || !hasSearch) {
									selected_index += view_element_capacity;
									scroll_pos += view_element_capacity;
									if (selected_index >= current_tree.Count) {
										selected_index = 0;
										scroll_pos = 0.0f;
									}
									current.Use();
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else if (current.keyCode == KeyCode.PageUp) {
								if (keyboard_navigation || !hasSearch) {
									selected_index -= view_element_capacity;
									scroll_pos -= view_element_capacity;
									if (selected_index < 0) {
										selected_index = current_tree.Count - 1;
										scroll_pos = current_tree.Count;
									}
									current.Use();
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else if (current.keyCode == KeyCode.DownArrow) {
								selected_index++;
								if (selected_index >= scroll_pos + view_element_capacity) {
									scroll_pos++;
								}
								if (selected_index >= current_tree.Count) {
									selected_index = 0;
									scroll_pos = 0.0f;
								}
								current.Use();
								text_navigation = false;
								keyboard_navigation = true;
							}
							else if (current.keyCode == KeyCode.UpArrow) {
								selected_index--;
								if (selected_index < scroll_pos) {
									scroll_pos--;
								}
								if (selected_index < 0) {
									selected_index = current_tree.Count - 1;
									scroll_pos = current_tree.Count;
								}
								current.Use();
								text_navigation = false;
								keyboard_navigation = true;
							}
							else if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter)) {
								this.GoToNode(selected_node, true);
								GUIUtility.keyboardControl = 0;
								current.Use();
								text_navigation = false;
								keyboard_navigation = true;
							}
							else if ((current.keyCode == KeyCode.LeftArrow) || (current.keyCode == KeyCode.Backspace)) {
								if (keyboard_navigation || !hasSearch) {
									this.GoToParent();
									current.Use();
									text_navigation = false;
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else if (current.keyCode == KeyCode.RightArrow) {
								if (keyboard_navigation || !hasSearch) {
									this.GoToNode(selected_node, false);
									current.Use();
									keyboard_navigation = true;
								}
								else {
									text_navigation = true;
								}
							}
							else {
								text_navigation = false;
								keyboard_navigation = false;
							}
						}
					}
					else {
						text_navigation = true;
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

		private string GetAutoTypeText(string text, int current_char) {
			return text.Substring(0, current_char);
		}

		public bool DrawElementList(Rect rect, GUIContent content, bool selected) {
			Rect layout_rect = new Rect(rect);
			bool trigger = GUI.Button(layout_rect, string.Empty, styles.label);

			Rect icon_rect = new Rect(layout_rect.x + 10.0f, layout_rect.y, ELEMENT_LIST_HEIGHT, ELEMENT_LIST_HEIGHT);
			Rect title_rect = new Rect(55.0f, layout_rect.y, layout_rect.width - 60.0f, layout_rect.height);
			Rect subtitle_rect = new Rect(title_rect);

			GUI.Label(icon_rect, content.image);
			if (!searchFormat.IsNullOrEmpty()) {
				string title = content.text.Replace(searchFormat, string.Format("<color=#ffff00ff><b>{0}</b></color>", searchFormat));
				string subtitle = content.tooltip.Replace(searchFormat, string.Format("<color=#ffff00ff><b>{0}</b></color>", searchFormat));

				EditorGUI.LabelField(title_rect, title, selected ? styles.on_search_item : styles.search_item);
				EditorGUI.LabelField(subtitle_rect, subtitle, selected ? styles.on_search_description_item : styles.search_description_item);
			}
			else {
				EditorGUI.LabelField(title_rect, content.text, selected ? styles.on_search_item : styles.search_item);
				EditorGUI.LabelField(subtitle_rect, content.tooltip, selected ? styles.on_search_description_item : styles.search_description_item);
			}

			return !drag_scroll && trigger;
		}

		void GoToHome() {
			GoToNode(root_tree, false);
			search = string.Empty;
			GUI.FocusControl("GUIControlSearchBoxTextField");
			need_refocus = true;
		}

		void GoToParent() {
			if (!current_tree.isRoot) {
				GoToNode(current_tree.parent, false);
			}
		}

		void GoToNode(TreeNode<Act> node, bool call_if_is_leaf) {
			if (node == null) return;
			disable_layout = true;

			if (node.isLeaf) {
				if (call_if_is_leaf) {
					node.data();
					if (!(node.parent != null && node.parent.content.text == "#Commands")) {
						this.Close();
					}
					else {
						GUI.FocusControl("GUIControlSearchBoxTextField");
						need_refocus = true;
					}
				}
			}
			else {
				last_tree = current_tree;
				current_tree = node;

				parents.Clear();
				TreeNode<Act> parent = current_tree.parent;
				//while parent isNotRoot
				while (parent != null) {
					parents.Add(parent);
					parent = parent.parent;
				}

				selected_index = 0;
				scroll_pos = 0.0f;
				RecalculateSize();
			}
		}

		void RecalculateSize() {
			Vector2 pos = new Vector2(Screen.currentResolution.width / 4.0f, 120.0f);
			Vector2 size = new Vector2(Screen.currentResolution.width / 2.0f, Mathf.Min(WINDOW_HEAD_HEIGHT + (current_tree.Count * ELEMENT_LIST_HEIGHT) + WINDOW_FOOT_OFFSET, Screen.currentResolution.height - 200.0f));

			position = new Rect(pos, size);
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

						icon = icons[type_gen] = GUIReferrer.GetTypeIcon(type_gen);

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
						if (!type_gen.HasAttribute<BuiltInNodeAttribute>(false)) {
							path = "References/" + path;
						}
						List<string> tags = new List<string>();
						if (type.IsSubclassOf(typeof(EventNode))) {
							tags.Add("EventNode");
						}
						if (type.IsSubclassOf(typeof(ActionNode))) {
							tags.Add("ActionNode");
						}
						if (type.IsSubclassOf(typeof(ValueNode))) {
							tags.Add("ValueNode");
						}
						if (type.IsSubclassOf(typeof(ReflectedNode))) {
							tags.Add("ReflectedNode");
						}
						if (type_gen.IsGenericType) {
							tags = tags.Concat(type_gen.GetGenericArguments().Select(gen_arg => gen_arg.GetTypeName(false, true))).ToList();
						}
						root_tree.AddChildByPath(new GUIContent(path, icon, type_descritpion), () => { AddNode(type_gen); }, tags.ToArray());
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
					if (!type.HasAttribute<BuiltInNodeAttribute>(false)) {
						path = "References/" + path;
					}
					List<string> tags = new List<string>();
					if (type.IsSubclassOf(typeof(EventNode))) {
						tags.Add("EventNode");
					}
					if (type.IsSubclassOf(typeof(ActionNode))) {
						tags.Add("ActionNode");
					}
					if (type.IsSubclassOf(typeof(ValueNode))) {
						tags.Add("ValueNode");
					}
					if (type.IsSubclassOf(typeof(ReflectedNode))) {
						tags.Add("ReflectedNode");
					}
					Type base_type = type;
					while (!base_type.IsGenericType) {
						if (base_type.BaseType == null) break;
						base_type = base_type.BaseType;
					}
					if (base_type.IsGenericType) {
						tags = tags.Concat(base_type.GetGenericArguments().Select(gen_arg => gen_arg.GetTypeName(false, true))).ToList();
					}
					root_tree.AddChildByPath(new GUIContent(path, icon, type_descritpion), () => { AddNode(type); }, tags.ToArray());

				}
			}
			//reflected nodes
			TreeNode<Act> codebase = root_tree.AddChild(new GUIContent("Codebase", EditorUtils.FindAssetByName<Texture>("Codebase Icon")), null);

			foreach (Type type in current_types) {
				Texture icon = icons[type];
				string type_name = type.GetTypeName(false, true);
				string namespace_path = type.Namespace.IsNullOrEmpty() ? "Global" : type.Namespace;

				TreeNode<Act> namespace_tree = codebase.AddChild(new GUIContent(namespace_path), null);
				TreeNode<Act> type_tree;
				if (type.IsEnum) {
					TreeNode<Act> enumerators_tree = namespace_tree.AddChild(new GUIContent("Enumerators"), null);
					type_tree = enumerators_tree.AddChild(new GUIContent(type_name, icon, type.GetTypeName(true)), null);
				}
				else {
					type_tree = namespace_tree.AddChild(new GUIContent(type_name, icon, type.GetTypeName(true)), null);
				}

				if (type.IsGenericType) {
					foreach (Type t in current_types) {
						Type type_gen = type.MakeGenericType(t);
						string type_gen_name = type_gen.GetTypeName();

						icon = icons[type_gen] = GUIReferrer.GetTypeIcon(type_gen);

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

		void AddNode(object obj) {
			GraphEditorWindow.AddNode((Type)obj, spawn_pos - GraphEditor.scroll);
		}

		void AddCustomNode(object obj) {
			object[] args = (object[])obj;
			GraphEditorWindow.AddCustomNode((Type)args[0], spawn_pos - GraphEditor.scroll, true, args[1]);
		}

		void AddReflectedNode(object obj) {
			GraphEditorWindow.AddCustomNode<ReflectedNode>(spawn_pos - GraphEditor.scroll, true, (MethodInfo)obj);
		}

		void AddReflectedGenericNode(object obj) {
			object[] args = (object[])obj;
			GraphEditorWindow.AddCustomNode<ReflectedNode>(spawn_pos - GraphEditor.scroll, true, (MethodInfo)args[0], new Type[] { (Type)args[1] });
		}
	}
}
#endif
