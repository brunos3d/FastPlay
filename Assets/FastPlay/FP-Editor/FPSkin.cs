#if UNITY_EDITOR
using UnityEngine;

namespace FastPlay.Editor {
	public static class FPSkin {

		private static GUISkin m_skin;

		public static GUISkin skin {
			get {
				if (!m_skin) return m_skin = EditorUtils.FindAssetByName<GUISkin>("FPSkin 1");
				return m_skin;
			}
		}

		public static GUIStyle icon {
			get {
				return GetStyle("Icon");
			}
		}

		public static GUIStyle unplugButton {
			get {
				return GetStyle("UnplugButton");
			}
		}

		public static GUIStyle unit {
			get {
				return GetStyle("Unit");
			}
		}

		public static GUIStyle border {
			get {
				return GetStyle("Border");
			}
		}

		public static GUIStyle header {
			get {
				return GetStyle("Header");
			}
		}

		public static GUIStyle headNode {
			get {
				return GetStyle("head");
			}
		}

		public static GUIStyle bodyNode {
			get {
				return GetStyle("body");
			}
		}

		public static GUIStyle bodyBackgroundNode {
			get {
				return GetStyle("body background");
			}
		}

		public static GUIStyle titleHead {

			get {
				return GetStyle("TitleHead");
			}
		}

		public static GUIStyle subtitleHead {
			get {
				return GetStyle("SubtitleHead");
			}
		}

		public static GUIStyle slimNode {
			get {
				return GetStyle("SlimNode");
			}
		}

		public static GUIStyle actionNode {
			get {
				return GetStyle("ActionNode");
			}
		}

		public static GUIStyle eventNode {
			get {
				return GetStyle("EventNode");
			}
		}

		public static GUIStyle valueNode {
			get {
				return GetStyle("ValueNode");
			}
		}

		public static GUIStyle otherNode {
			get {
				return GetStyle("OtherNode");
			}
		}

		public static GUIStyle iconNode {
			get {
				return GetStyle("IconNode");
			}
		}

		public static GUIStyle highlightNode {
			get {
				return GetStyle("HighlightNode");
			}
		}

		public static GUIStyle inputPort {
			get {
				return GetStyle("InputPort");
			}
		}

		public static GUIStyle outputPort {
			get {
				return GetStyle("OutputPort");
			}
		}

		public static GUIStyle inputAction {
			get {
				return GetStyle("InputAction");
			}
		}

		public static GUIStyle outputAction {
			get {
				return GetStyle("OutputAction");
			}
		}

		public static GUIStyle inputLabel {
			get {
				return GetStyle("InputLabel");
			}
		}

		public static GUIStyle outputLabel {
			get {
				return GetStyle("OutputLabel");
			}
		}

		public static GUIStyle GetStyle(string style_name) {
			return skin.FindStyle(style_name);
		}
	}
}
#endif