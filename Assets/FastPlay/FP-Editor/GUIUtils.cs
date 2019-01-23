#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public static class GUIUtils {
		public static float GetTextWidth(string text) {
			return GetTextSize(text, EditorStyles.label).x;
		}

		public static float GetTextWidth(string text, GUIStyle style) {
			return GetTextSize(text, style).x;
		}

		public static float GetTextHeight(string text) {
			return GetTextSize(text, EditorStyles.label).y;
		}

		public static float GetTextHeight(string text, GUIStyle style) {
			return GetTextSize(text, style).y;
		}

		public static Vector2 GetTextSize(string text) {
			GUIContent content = new GUIContent(text);
			return EditorStyles.label.CalcSize(content);
		}

		public static Vector2 GetTextSize(string text, GUIStyle style) {
			GUIContent content = new GUIContent(text);
			return style.CalcSize(content);
		}

		public static Vector2 GetTextSize(GUIContent content) {
			return EditorStyles.label.CalcSize(content);
		}

		public static Vector2 GetTextSize(GUIContent content, GUIStyle style) {
			return style.CalcSize(content);
		}
	}
}
#endif
