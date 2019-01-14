#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public class AdvancedSearchWindow : EditorWindow {

		private class Styles {

			public Rect search_rect;

			public Rect search_icon_rect;

			public Texture search_icon;

			public GUIStyle label = (GUIStyle)"label";

			public GUIStyle search_bar;

			public GUIStyle search_item;

			public GUIStyle search_description_item;

			public Styles(float width) {
				search_bar = FPSkin.GetStyle("Search Bar");
				search_item = FPSkin.GetStyle("Search Item");
				search_description_item = FPSkin.GetStyle("Search Description");
				search_icon = EditorGUIUtility.FindTexture("Search Icon");
				search_rect = new Rect(15.0f, 10.0f, width - 30.0f, 30.0f);
				search_icon_rect = new Rect(20.0f, 13.0f, 23.0f, 23.0f);
			}
		}

		private static Styles styles;

		public static string search;

		private static Vector2 scroll = Vector2.zero;

		public static bool hasSearch {
			get {
				return !search.IsNullOrEmpty();
			}
		}

		[MenuItem("AdvancedSearchWindow/Show")]
		public static void Init() {
			if (Resources.FindObjectsOfTypeAll<AdvancedSearchWindow>().Length == 0) {
				AdvancedSearchWindow window = AdvancedSearchWindow.CreateInstance<AdvancedSearchWindow>();

				Vector2 pos = new Vector2(Screen.currentResolution.width / 4.0f, 120.0f);
				Vector2 size = new Vector2(Screen.currentResolution.width / 2.0f, 300.0f);

				window.ShowPopup();
				window.position = new Rect(pos, size);
				//window.ShowAsDropDown(new Rect(pos, Vector2.zero), size);
			}
			else {
				FocusWindowIfItsOpen<AdvancedSearchWindow>();
			}
		}

		void OnEnable() {
		}

		void OnGUI() {
			if (styles == null) {
				styles = new Styles(position.width);
			}
			if (styles.search_icon == null) {
				return;
			}
			//Search Bar
			search = EditorGUI.TextField(styles.search_rect, search, styles.search_bar);
			GUI.DrawTexture(styles.search_icon_rect, styles.search_icon);

			GUILayout.BeginArea(new Rect(0.0f, 50.0f, position.width, position.height - 50.0f));
			scroll = EditorGUILayout.BeginScrollView(scroll);

			//GUILayout.Label(position.ToString(), GUILayout.Height(30.0f));

			for (int id = 0; id < 10.0f; id++) {
				if (GUILayout.Button("", styles.label, GUILayout.Height(50.0f))) {
					Debug.Log(id);
				}
				Rect r = GUILayoutUtility.GetLastRect();
				Rect icon_rect = new Rect(r.x + 10.0f, r.y, 50.0f, 50.0f);
				Rect title_rect = new Rect(55.0f, r.y, r.width - 60.0f, 50.0f);
				Rect subtitle_rect = new Rect(title_rect);

				GUI.Label(icon_rect, GUIReferrer.GetTypeIcon(typeof(GameObject)));
				GUI.Label(title_rect, string.Format("{0}. This is an simple title", id), styles.search_item);
				GUI.Label(subtitle_rect, string.Format("{0}. I'm a subtitle, read-me =]", id), styles.search_description_item);
			}

			EditorGUILayout.EndScrollView();
			GUILayout.EndArea();

			Repaint();
		}
	}
}
#endif
