#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public static class GUIReferrer {

		private static Dictionary<Type, Texture> icons = new Dictionary<Type, Texture>();

		private static Dictionary<Type, string> paths = new Dictionary<Type, string>() {
			{ typeof(EventNode), "Event Icon" },
			{ typeof(MacroNode), "Macro Icon" },
			{ typeof(Graph), "GraphAsset Icon" },
			{ typeof(GraphAsset), "GraphAsset Icon" },
			{ typeof(GraphController), "GraphAsset Icon" },
			{ typeof(int), "Integer Icon" },
			{ typeof(float), "Float Icon" },
			{ typeof(bool), "Boolean Icon" },
			{ typeof(string), "String Icon" },
			{ typeof(object), "SystemObject Icon" },
			{ typeof(Enum), "Enum Icon" },
			{ typeof(Rect), "Rect Icon" },
			{ typeof(Color), "Color Icon" },
			{ typeof(Vector2), "Vector2 Icon" },
			{ typeof(Vector3), "Vector3 Icon" },
			{ typeof(Vector4), "Vector4 Icon" },
			{ typeof(Quaternion), "Quaternion Icon" },
			{ typeof(UnityEngine.Object), "UnityObject Icon" },
			{ typeof(UnityEngine.ScriptableObject), "UnityScriptableObject Icon" },
			{ typeof(Time), "Time Icon" },
			{ typeof(Physics), "Physics Icon" },
			{ typeof(Physics2D), "Physics2D Icon" },
			{ typeof(Debug), "Debug Icon" },
			{ typeof(Input), "Input Icon" },
			{ typeof(PlayerPrefs), "PlayerPrefs Icon" },
			{ typeof(Screen), "Screen Icon" },
			{ typeof(Cursor), "Cursor Icon" },

	};

		private static Dictionary<Type, Color> colors = new Dictionary<Type, Color>() {
			{ typeof(ActionPort), Color.white },
			{ typeof(InputNode), new Color(1.0f, 76.0f / 255.0f, 76.0f / 255.0f) },
			{ typeof(OutputNode), new Color(1.0f, 76.0f / 255.0f, 76.0f / 255.0f) },
			{ typeof(EventNode), new Color(1.0f, 76.0f / 255.0f, 76.0f / 255.0f) },
			{ typeof(ActionNode), new Color(90.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f) },
			{ typeof(void), new Color(90.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f) },
			{ typeof(int), new Color(0.0f, 175 / 255.0f, 255.0f / 255.0f) },
			{ typeof(bool), new Color(232.0f / 255.0f, 43.0f / 255.0f, 46.0f / 255.0f) },
			{ typeof(float), new Color(21.0f / 255.0f, 127.0f / 255.0f, 109.0f / 255.0f) },
			{ typeof(string), new Color(255.0f / 255.0f, 160.0f/255.0f, 99.0f / 255.0f) },
			{ typeof(object), new Color(0.7f, 0.7f, 0.7f) },
			{ typeof(Enum), new Color(255.0f / 255.0f, 213.0f / 255.0f, 120.0f / 255.0f) },
			{ typeof(Rect), new Color(95.0f / 255.0f, 146.0f / 255.0f, 87.0f / 255.0f) },
			{ typeof(Color), new Color(200.0f / 255.0f, 100.0f / 255.0f, 205.0f / 255.0f) },
			{ typeof(Vector2), new Color(1.0f, 193.0f / 255.0f, 64.0f / 255.0f) },
			{ typeof(Vector3), new Color(1.0f, 193.0f / 255.0f, 64.0f / 255.0f) },
			{ typeof(Vector4), new Color(1.0f, 193.0f / 255.0f, 64.0f / 255.0f) },
			{ typeof(Quaternion), new Color(85.0f / 255.0f, 90.0f / 255.0f, 232.0f / 255.0f) },
			{ typeof(UnityEngine.Object), Color.gray },
			{ typeof(Texture), new Color(0.0f / 255.0f, 232.0f / 255.0f, 87.0f / 255.0f) },
	};

		public static Texture GetTypeIcon(Type type) {
			Texture t;
			if (icons.TryGetValue(type, out t)) {
				return t;
			}
			string path;
			if (paths.TryGetValue(type, out path)) {
				return icons[type] = EditorUtils.FindAssetByName<Texture>(path);
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return icons[type] = EditorGUIUtility.ObjectContent(null, type).image;
			}
			return icons[type] = EditorUtils.FindAssetByName<Texture>("404_icon_not_found");
		}

		public static Color GetTypeColor(Type type) {
			Color c;
			if (colors.TryGetValue(type, out c)) {
				return colors[type];
			}
			if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return colors[type.BaseType] = Color.gray;
			}
			if (type.BaseType != null && colors.TryGetValue(type.BaseType, out c)) {
				return colors[type] = colors[type.BaseType];
			}
			//float r = UnityEngine.Random.Range(0.0f, 1.0f);
			//float g = UnityEngine.Random.Range(0.0f, 1.0f);
			//float b = UnityEngine.Random.Range(0.0f, 1.0f);
			//return colors[type] = new Color(r, g, b, 1.0f);
			return colors[type] = new Color(56.0f / 255.0f, 56.0f / 255.0f, 56.0f / 255.0f);
		}
	}
}
#endif
