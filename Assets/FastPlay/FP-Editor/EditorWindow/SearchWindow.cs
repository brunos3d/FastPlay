#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public class SearchWindow : EditorWindow {

		private class Styles {

			public GUIStyle background = "grey_border";

			public GUIStyle button = "MenuItem";

			public GUIStyle leftArrow = "AC LeftArrow";

			public GUIStyle previewBackground = "PopupCurveSwatchBackground";

			public Styles() { }
		}

		private static Styles styles;

		private static readonly Vector2 V2x300y400 = new Vector2(300.0f, 400.0f);

		private static readonly Color FOCUS_COLOR = new Color(62.0f / 255.0f, 95.0f / 255.0f, 150.0f / 255.0f);

		private static bool keyboard_pressed;

		private static bool keyboard_used;

		private static Vector2 start_pos = Vector2.zero;

		private static Vector2 spawn_pos = Vector2.zero;

		private static Vector2 scroll = Vector2.zero;

		public static string search;

		public static string last_search;

		public static ContextItem context;

		public static ContextItem current_context;

		public static ContextItem active_element;

		[SerializeField]
		private ThreadLoopInstance thread;

		public static bool hasSearch {
			get {
				return !search.IsNullOrEmpty();
			}
		}

		public static void Init(Vector2 show_pos, Vector2 spawn_pos) {
			EditorWindow window = EditorWindow.CreateInstance<SearchWindow>();
			SearchWindow.start_pos = show_pos;
			SearchWindow.spawn_pos = spawn_pos;
			window.ShowAsDropDown(new Rect(show_pos, Vector2.zero), V2x300y400);
			GetWindow<SearchWindow>();
		}

		void OnEnable() {
			search = string.Empty;
			if (thread || (thread = FindObjectOfType<ThreadLoopInstance>())) {
				if (thread.variable_count != GraphEditor.graph.variableParameters.Count) {
					//an little refresh to update variables
					thread.StopThread();
					DestroyImmediate(thread);
				}
			}
			if (thread) {
				thread.StartThread();
			}
			else {
				if (!(thread = FindObjectOfType<ThreadLoopInstance>())) {
					thread = CreateInstance<ThreadLoopInstance>();
					thread.CreateThread();
				}
				thread.StartThread();
			}
			thread.spawn_pos = spawn_pos;
			current_context = context = thread.context;
		}

		public void OnDisable() {
		}

		void OnGUI() {
			if (styles == null) {
				styles = new Styles();
			}
			GUI.Label(new Rect(0.0f, 0.0f, base.position.width, base.position.height), GUIContent.none, styles.background);

			if (keyboard_pressed) {
				if (keyboard_used) {
					if (hasSearch) {
						GUI.FocusControl("0");
						current_context.selected_index = 0;
					}
				}
				else {
					GUI.FocusControl("GUIControlSearchBoxTextField");
				}
			}

			search = GUIDraw.SearchBar(search);

			if (thread) {
				if (thread.is_ready) {
					if (hasSearch) {
						if (last_search != search) {
							last_search = search;
						}
						if (GUILayout.Button("Clear", GUILayout.ExpandWidth(true), GUILayout.Height(40.0f))) {
							last_search = search = string.Empty;
							GoToChild(context.root, false);
						}
					}
					else {
						if (GUILayout.Button(current_context.content, GUILayout.ExpandWidth(true), GUILayout.Height(40.0f))) {
							GoToParent();
						}
					}
					scroll = GUILayout.BeginScrollView(scroll);

					int index = 0;
					foreach (ContextItem item in current_context.items.Values) {
						Rect layout_rect = new Rect(1.0f, (index * 22.0f), position.width - 2.0f, 22.0f);
						if (layout_rect.Contains(Event.current.mousePosition)) {
							current_context.selected_index = index;
						}
						if (current_context.selected_index == index) {
							active_element = item;
							EditorGUI.DrawRect(layout_rect, FOCUS_COLOR);
						}
						if (!item.is_button) {
							GUILayout.BeginHorizontal();
						}
						GUILayout.Label(item.content, EditorStyles.label, GUILayout.Height(20.0f));
						GUI.SetNextControlName(index.ToString());
						if (GUI.Button(layout_rect, "", EditorStyles.label)) {
							GoToChild(item, true);
						}
						if (!item.is_button) {
							GUILayout.FlexibleSpace();
							GUILayout.Button(">", EditorStyles.label);
							GUILayout.EndHorizontal();
						}
						index++;
					}
					GUILayout.EndScrollView();
				}
				else {
					EditorGUI.ProgressBar(new Rect(20.0f, position.height / 2.0f - 10.0f, position.width - 40.0f, 20.0f), (float)thread.loop_index / thread.loop_length, thread.thread_info);
				}
			}
			OnInputGUI();
			Repaint();
		}

		void OnInputGUI() {
			Event current = Event.current;

			keyboard_used = false;
			keyboard_pressed = false;
			switch (current.type) {
				case EventType.MouseDown:
					break;
				case EventType.MouseDrag:
					break;
				case EventType.KeyDown:
					keyboard_pressed = true;
					if (hasSearch) {
					}
					else {
						if (current.keyCode == KeyCode.Home) {
							current_context.selected_index = 0;
							scroll = Vector2.zero;
							current.Use();
							keyboard_used = true;
						}
						if (current.keyCode == KeyCode.End) {
							current_context.selected_index = current_context.items.Count - 1;
							scroll = new Vector2(0.0f, (current_context.selected_index * 22.0f) - position.height / 2.0f);
							current.Use();
							keyboard_used = true;
						}
						if (current.keyCode == KeyCode.DownArrow) {
							current_context.selected_index++;
							if (current_context.selected_index >= current_context.items.Count) {
								current_context.selected_index = 0;
							}
							scroll = new Vector2(0.0f, (current_context.selected_index * 22.0f) - position.height / 2.0f);
							current.Use();
							keyboard_used = true;
						}
						if (current.keyCode == KeyCode.UpArrow) {
							current_context.selected_index--;
							if (current_context.selected_index < 0) {
								current_context.selected_index = current_context.items.Count - 1;
							}
							scroll = new Vector2(0.0f, (current_context.selected_index * 22.0f) - position.height / 2.0f);
							current.Use();
							keyboard_used = true;
						}
						if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter)) {
							this.GoToChild(active_element, true);
							current.Use();
							keyboard_used = true;
						}
						if ((current.keyCode == KeyCode.LeftArrow) || (current.keyCode == KeyCode.Backspace)) {
							this.GoToParent();
							current.Use();
							keyboard_used = true;
						}
						if (current.keyCode == KeyCode.RightArrow) {
							this.GoToChild(active_element, false);
							current.Use();
							keyboard_used = true;
						}
					}
					break;
			}
		}

		void GoToChild(ContextItem child, bool call_if_button) {
			if (child.is_button) {
				if (call_if_button) {
					child.act(child.data);
					this.Close();
				}
			}
			else {
				current_context = child;
				RecalculateWindowWidth();
				scroll = new Vector2(0.0f, (current_context.selected_index * 22.0f) - position.height / 2.0f);
			}
			Repaint();
		}

		void GoToParent() {
			current_context = current_context.parent;
			RecalculateWindowWidth();
			scroll = new Vector2(0.0f, (current_context.selected_index * 22.0f) - position.height / 2.0f);
			Repaint();
		}

		void RecalculateWindowWidth() {
			float width = 0.0f;
			foreach (ContextItem item in current_context.items.Values) {
				width = Mathf.Max(width, GUIUtils.GetTextWidth(item.content.text, EditorStyles.label));
			}
			ShowAsDropDown(new Rect(start_pos, Vector2.zero), new Vector2(Mathf.Max(width + 150.0f, 300.0f), position.height));
		}
	}
}
#endif
