#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public static class GUIUtils {

		public static float GetTextWidth(string text, GUIStyle style) {
			return GetTextSize(text, style).x;
		}

		public static float GetTextHeight(string text, GUIStyle style) {
			return GetTextSize(text, style).y;
		}

		public static Vector2 GetTextSize(string text, GUIStyle style) {
			GUIContent content = new GUIContent(text);
			return style.CalcSize(content);
		}
	}
}
#endif
