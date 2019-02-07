#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public static class GUIReferrer {

		private static Dictionary<Type, Texture> icons = new Dictionary<Type, Texture>();

		private static Dictionary<Type, string> paths = new Dictionary<Type, string>() {
			{ typeof(EventNode), "event_icon" },
			{ typeof(MacroNode), "script_bricks" },
			{ typeof(Graph), "graph_asset_icon" },
			{ typeof(GraphAsset), "graph_asset_icon" },
			{ typeof(GraphController), "graph_asset_icon" },
			{ typeof(int), "integer_icon" },
			{ typeof(float), "float_icon" },
			{ typeof(bool), "boolean_icon" },
			{ typeof(string), "string_icon" },
			{ typeof(object), "system_object_icon" },
			{ typeof(Type), "class_module" },
			{ typeof(Enum), "enum_icon" },
			{ typeof(Rect), "canvas" },
			{ typeof(Color), "color_wheel" },
			{ typeof(Vector2), "vector2_icon" },
			{ typeof(Vector3), "vector3_icon" },
			{ typeof(Vector4), "vector4_icon" },
			{ typeof(Quaternion), "measure" },
			{ typeof(UnityObject), "unity_object_icon" },
			{ typeof(LayerMask), "layers" },
			{ typeof(UnityEngine.ScriptableObject), "unity_scriptable_object_icon" },
			{ typeof(Ray), "ray_icon" },
			{ typeof(Time), "time" },
			{ typeof(Physics), "physics_icon" },
			{ typeof(Physics2D), "physics2D_icon" },
			{ typeof(Mathf), "math_functions" },
			{ typeof(Debug), "bug" },
			{ typeof(Input), "keyboard" },
			{ typeof(PlayerPrefs), "diskette" },
			{ typeof(Screen), "lcd_tv_test" },
			{ typeof(Cursor), "cursor" },
			{ typeof(Application), "application" },
			{ typeof(GUI), "application_view_icons" },
			{ typeof(GUILayout), "application_view_tile" },
			{ typeof(WWW), "www_page" },

	};

		private static readonly Color DEFAULT_COLOR = new Color(0.3f, 0.3f, 0.3f);

		private static Dictionary<Type, Color> colors = new Dictionary<Type, Color>() {
			{ typeof(EventNode), new Color(30.0f / 255.0f, 120.0f / 255.0f, 54.0f / 255.0f) },
			{ typeof(InputNode), new Color(1.0f, 76.0f / 255.0f, 76.0f / 255.0f) },
			{ typeof(OutputNode), new Color(1.0f, 76.0f / 255.0f, 76.0f / 255.0f) },
			{ typeof(ActionPort), Color.white },
			/*
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
			{ typeof(UnityObject), Color.gray },
			{ typeof(LayerMask), Color.gray },
			{ typeof(Texture), new Color(0.0f / 255.0f, 232.0f / 255.0f, 87.0f / 255.0f) },
			*/
	};

		public static string GetTypeIconName(Type type, bool return_default_icon = true) {
			Texture t = GetTypeIcon(type, return_default_icon);
			if (t != null) {
				return t.name;
			}
			else {
				return string.Empty;
			}
		}

		public static Texture GetTypeIcon(Type type, bool return_default_icon = true) {
			if (type == null) {
				if (return_default_icon) {
					return icons[typeof(Nullable)] = EditorUtils.FindAssetByName<Texture>("http_status_not_found");
				}
				return null;
			}
			Texture t;
			if (icons.TryGetValue(type, out t)) {
				return t;
			}
			string path;
			if (paths.TryGetValue(type, out path)) {
				return icons[type] = EditorUtils.FindAssetByName<Texture>(path);
			}
			IconAttribute icon_flag = type.GetAttribute<IconAttribute>(false);
			if (icon_flag != null) {
				return icons[type] = icon_flag.GetIcon();
			}
			else if (typeof(UnityObject).IsAssignableFrom(type)) {
				return icons[type] = EditorGUIUtility.ObjectContent(null, type).image;
			}
			else if (type.IsSubclassOf(typeof(EventNode))) {
				return icons[type] = GetTypeIcon(typeof(EventNode), return_default_icon);
			}
			else {
				Type base_type = type;
				while (!base_type.IsGenericType) {
					if (base_type.BaseType == null) break;
					base_type = base_type.BaseType;
				}
				if (base_type.IsGenericType) {
					t = GetTypeIcon(base_type.GetGenericArguments()[0], false);
					if (t != null) {
						return icons[type] = t;
					}
				}
			}
			if (return_default_icon) {
				return icons[type] = EditorUtils.FindAssetByName<Texture>("http_status_not_found");
			}
			else {
				return null;
			}
		}

		public static Color GetTypeColor(Type type, bool return_default_color = true) {
			if (type == null) {
				if (return_default_color) {
					return colors[type] = DEFAULT_COLOR;
				}
				return Color.magenta;
			}
			Color c;
			if (colors.TryGetValue(type, out c)) {
				return colors[type];
			}
			if (typeof(UnityObject).IsAssignableFrom(type)) {
				return colors[type.BaseType] = Color.gray;
			}
			if (type.BaseType != null && colors.TryGetValue(type.BaseType, out c)) {
				return colors[type] = colors[type.BaseType];
			}
			//float r = UnityEngine.Random.Range(0.0f, 1.0f);
			//float g = UnityEngine.Random.Range(0.0f, 1.0f);
			//float b = UnityEngine.Random.Range(0.0f, 1.0f);
			//return colors[type] = new Color(r, g, b, 1.0f);
			if (return_default_color) {
				return colors[type] = DEFAULT_COLOR;
			}
			else {
				return Color.magenta;
			}
		}
	}
}
#endif
