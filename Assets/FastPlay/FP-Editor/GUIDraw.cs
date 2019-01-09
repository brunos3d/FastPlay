#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using FastPlay.Runtime;
using System;
using System.Collections.Generic;
using System.Collections;

namespace FastPlay.Editor {
	public sealed class GUIDraw {

		public static Dictionary<object, bool> foldouts = new Dictionary<object, bool>();

		private class Styles {

			public GUIStyle label;
			public GUIStyle type_button;
			public GUIStyle search_text_field;
			public GUIStyle search_cancel_button;

			public Styles() {
				label = FPSkin.GetStyle("label");
				type_button = new GUIStyle("Popup");
				search_text_field = new GUIStyle("SearchTextField");
				search_cancel_button = new GUIStyle("SearchCancelButton");
			}
		}

		private static Styles m_styles;

		private static Styles styles {
			get {
				if (m_styles == null) {
					m_styles = new Styles();
				}
				return m_styles;
			}
		}

		private static int cached_id;

		private static int current_id;

		private static object cached_value;

		private static Color backgroundColor;

		public static void BeginNewColor(Color background) {
			GUIDraw.backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = background;
		}

		public static void EndNewColor() {
			GUI.backgroundColor = GUIDraw.backgroundColor;
		}

		public static string SearchBar(string search) {
			GUILayout.Space(3.0f);
			GUILayout.BeginHorizontal();
			GUI.SetNextControlName("GUIControlSearchBoxTextField");
			search = GUILayout.TextField(search, styles.search_text_field);
			GUI.SetNextControlName("SearchBoxCancelButton");
			if (GUILayout.Button(string.Empty, styles.search_cancel_button)) {
				GUI.FocusControl(null);
				search = string.Empty;
			}
			GUILayout.EndHorizontal();
			GhostLabel(search, "Search", -1.0f, 25.0f);
			GUILayout.Space(5.0f);
			return search;
		}

		public static void GhostLabel(string check_text, string label, float offset_x = 1.0f, float offset_y = -3.0f) {
			if (check_text.IsNullOrEmpty()) {
				Rect last_rect = GUILayoutUtility.GetLastRect();
				last_rect.y += offset_x;
				last_rect.x += offset_y;
				GUI.color = new Color(1, 1, 1, 0.35f);
				GUI.Label(last_rect, string.Format("<i>{0}</i>", label), styles.label);
				GUI.color = Color.white;
			}
		}

		// Rect

		public static T AnyField<T>(Rect rect, T value) {
			return (T)AnyField(rect, value, typeof(T), GUIContent.none);
		}

		public static T AnyField<T>(Rect rect, T value, string content) {
			return (T)AnyField(rect, value, typeof(T), new GUIContent(content));
		}

		public static T AnyField<T>(Rect rect, T value, GUIContent content) {
			return (T)AnyField(rect, value, typeof(T), content);
		}

		public static object AnyField(Rect rect, object value) {
			return AnyField(rect, value, GUIContent.none);
		}

		public static object AnyField(Rect rect, object value, Type type) {
			return AnyField(rect, value, type, GUIContent.none);
		}

		public static object AnyField(Rect rect, object value, string content) {
			return AnyField(rect, value, new GUIContent(content));
		}

		public static object AnyField(Rect rect, object value, Type type, string content) {
			return AnyField(rect, value, type, new GUIContent(content));
		}

		public static object AnyField(Rect rect, object value, GUIContent content) {
			if (value == null) return null;
			return AnyField(rect, value, value.GetType(), content);
		}

		public static object AnyField(Rect rect, object value, Type type, GUIContent content) {
			if (type == null) return value;

			if (typeof(float).IsAssignableFrom(type)) {
				return EditorGUI.FloatField(rect, content, (float)value);
			}
			else if (typeof(int).IsAssignableFrom(type)) {
				return EditorGUI.IntField(rect, content, (int)value);
			}
			else if (typeof(string).IsAssignableFrom(type)) {
				return EditorGUI.TextField(rect, content, (string)value);
			}
			else if (typeof(bool).IsAssignableFrom(type)) {
				return EditorGUI.Toggle(rect, content, (bool)value);
			}
			else if (typeof(Vector2).IsAssignableFrom(type)) {
				return EditorGUI.Vector2Field(rect, content, (Vector2)value);
			}
			else if (typeof(Vector3).IsAssignableFrom(type)) {
				return EditorGUI.Vector3Field(rect, content, (Vector3)value);
			}
			else if (typeof(Vector4).IsAssignableFrom(type)) {
				return EditorGUI.Vector4Field(rect, content.text, (Vector4)value);
			}
			else if (typeof(Quaternion).IsAssignableFrom(type)) {
				return Vector4ToQuaternion(EditorGUI.Vector4Field(rect, content.text, QuaternionToVector4((Quaternion)value)));
			}
			else if (typeof(Rect).IsAssignableFrom(type)) {
				return EditorGUI.RectField(rect, content, (Rect)value);
			}
			else if (typeof(Color).IsAssignableFrom(type)) {
				return EditorGUI.ColorField(rect, content, (Color)value);
			}
			else if (typeof(LayerMask).IsAssignableFrom(type)) {
				return EditorGUI.LayerField(rect, content, (LayerMask)value);
			}
			else if (typeof(Enum).IsAssignableFrom(type)) {
				return EditorGUI.EnumPopup(rect, content, (Enum)value);
			}
			else if (typeof(Type).IsAssignableFrom(type)) {
				current_id = GUIUtility.GetControlID(FocusType.Passive);
				if (value == null) {
					if (GUI.Button(rect, "(Empty Type)", styles.type_button)) {
						cached_id = current_id;
						ShowContentTypeMenu();
					}
				}
				else {
					Type value_cast = (Type)value;
					GUIContent b_content = new GUIContent(value_cast.GetTypeName(false, true), GUIReferrer.GetTypeIcon(value_cast));
					Rect b_rect = new Rect(rect);
					if (!content.text.IsNullOrEmpty()) {
						b_rect.x += 5.0f + GUIUtils.GetTextWidth(content.text, styles.label);
						GUI.Label(rect, content);
					}
					if (GUI.Button(b_rect, b_content, styles.type_button)) {
						cached_id = current_id;
						ShowContentTypeMenu();
					}
				}
				if (cached_value == null) {
					return value;
				}
				else {
					if (current_id == cached_id) {
						object result = cached_value;
						cached_value = null;
						return result;
					}
					else {
						return value;
					}
				}
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return EditorGUI.ObjectField(rect, content, (UnityEngine.Object)value, type, true);
			}
			else if (typeof(IList).IsAssignableFrom(type) && !typeof(Array).IsAssignableFrom(type)) {
				Rect area = new Rect(rect);
				GUILayout.BeginArea(area);
				object collection = AnyField(value, type, content);
				GUILayout.EndArea();
				return collection;
			}
			GUI.Label(rect, string.Format("ERROR 404. {0} Field Not Found!", type.GetTypeName(true)));
			return value;
		}

		// Layout

		public static T AnyField<T>(T value, params GUILayoutOption[] options) {
			return (T)AnyField(value, typeof(T), GUIContent.none, options);
		}

		public static T AnyField<T>(T value, string content, params GUILayoutOption[] options) {
			return (T)AnyField(value, typeof(T), new GUIContent(content), options);
		}

		public static T AnyField<T>(T value, GUIContent content, params GUILayoutOption[] options) {
			return (T)AnyField(value, typeof(T), content, options);
		}

		public static object AnyField(object value, params GUILayoutOption[] options) {
			return AnyField(value, GUIContent.none);
		}

		public static object AnyField(object value, Type type, params GUILayoutOption[] options) {
			return AnyField(value, type, GUIContent.none, options);
		}

		public static object AnyField(object value, string content, params GUILayoutOption[] options) {
			return AnyField(value, new GUIContent(content), options);
		}

		public static object AnyField(object value, Type type, string content, params GUILayoutOption[] options) {
			return AnyField(value, type, new GUIContent(content), options);
		}

		public static object AnyField(object value, GUIContent content, params GUILayoutOption[] options) {
			if (value == null) return null;
			return AnyField(value, value.GetType(), content, options);
		}

		public static object AnyField(object value, Type type, GUIContent content, params GUILayoutOption[] options) {
			if (type == null) return value;

			if (typeof(float).IsAssignableFrom(type)) {
				return EditorGUILayout.FloatField(content, (float)value, options);
			}
			else if (typeof(int).IsAssignableFrom(type)) {
				return EditorGUILayout.IntField(content, (int)value, options);
			}
			else if (typeof(string).IsAssignableFrom(type)) {
				return EditorGUILayout.TextField(content, (string)value, options);
			}
			else if (typeof(bool).IsAssignableFrom(type)) {
				return EditorGUILayout.Toggle(content, (bool)value, options);
			}
			else if (typeof(Vector2).IsAssignableFrom(type)) {
				return EditorGUILayout.Vector2Field(content, (Vector2)value, options);
			}
			else if (typeof(Vector3).IsAssignableFrom(type)) {
				return EditorGUILayout.Vector3Field(content, (Vector3)value, options);
			}
			else if (typeof(Vector4).IsAssignableFrom(type)) {
				return EditorGUILayout.Vector4Field(content.text, (Vector4)value, options);
			}
			else if (typeof(Quaternion).IsAssignableFrom(type)) {
				return Vector4ToQuaternion(EditorGUILayout.Vector4Field(content.text, QuaternionToVector4((Quaternion)value), options));
			}
			else if (typeof(Rect).IsAssignableFrom(type)) {
				return EditorGUILayout.RectField(content, (Rect)value, options);
			}
			else if (typeof(Color).IsAssignableFrom(type)) {
				return EditorGUILayout.ColorField(content, (Color)value, options);
			}
			else if (typeof(LayerMask).IsAssignableFrom(type)) {
				return EditorGUILayout.LayerField(content, (LayerMask)value, options);
			}
			else if (typeof(Enum).IsAssignableFrom(type)) {
				return EditorGUILayout.EnumPopup(content, (Enum)value, options);
			}
			else if (typeof(Type).IsAssignableFrom(type)) {
				current_id = GUIUtility.GetControlID(FocusType.Passive);

				if (value == null) {
					if (!content.text.IsNullOrEmpty()) {
						GUILayout.BeginHorizontal(options);
						GUILayout.Label(content, options);
					}
					if (GUILayout.Button("(Empty Type)", styles.type_button, options)) {
						cached_id = current_id;
						ShowContentTypeMenu();
					}
					if (!content.text.IsNullOrEmpty()) {
						GUILayout.EndHorizontal();
					}
				}
				else {
					Type value_cast = (Type)value;
					GUIContent b_content = new GUIContent(value_cast.GetTypeName(false, true), GUIReferrer.GetTypeIcon(value_cast));
					if (!content.text.IsNullOrEmpty()) {
						GUILayout.BeginHorizontal(options);
						GUILayout.Label(content, options);
					}
					if (GUILayout.Button(b_content, styles.type_button, options)) {
						cached_id = current_id;
						ShowContentTypeMenu();
					}
					if (!content.text.IsNullOrEmpty()) {
						GUILayout.EndHorizontal();
					}
				}
				if (cached_value == null) {
					return value;
				}
				else {
					if (current_id == cached_id) {
						object result = cached_value;
						cached_value = null;
						return result;
					}
					else {
						return value;
					}
				}
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) {
				return EditorGUILayout.ObjectField(content, (UnityEngine.Object)value, type, true, options);
			}
			else if (typeof(Array).IsAssignableFrom(type)) {
				Array array = (Array)value;
				Type array_type = type.GetElementType();
				if (array == null) {
					GUILayout.BeginHorizontal();
					GUILayout.Label(content, options);
					GUILayout.Label("[NULL]", options);
					GUILayout.EndHorizontal();
					if (GUILayout.Button("Create Array")) {
						array = Array.CreateInstance(array_type, 1);
					}
				}
				else {
					bool f;
					if (!foldouts.TryGetValue(value, out f)) {
						foldouts[value] = true;
					}
					GUILayout.BeginVertical("HelpBox", GUILayout.MinHeight(10.0f));
					{
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(10.0f);
							GUILayout.BeginVertical();
							{
								GUILayout.BeginHorizontal();
								if (array.Length > 0) {
									foldouts[value] = EditorGUILayout.Foldout(foldouts[value], new GUIContent(string.Format("{0}: [{1}]", content.text, array.Length), content.image, content.tooltip));
								}
								else {
									GUILayout.Label(new GUIContent(string.Format("{0}: [0]", content.text), content.image, content.tooltip), options);
								}

								GUILayout.FlexibleSpace();
								if (GUILayout.Button("+", "minibutton")) {
									Extensions.Resize(ref array, array_type, array.Length + 1);
								}
								if (GUILayout.Button("x", "minibutton")) {
									if (array.Length > 0) {
										Extensions.Resize(ref array, array_type, array.Length - 1);
									}
									else {
										return null;
									}
								}
								GUILayout.EndHorizontal();
								if (foldouts[value]) {
									for (int id = 0; id < array.Length; id++) {
										array.SetValue(AnyField(array.GetValue(id), array_type, new GUIContent(string.Format("Element {0}", id)), options), id);
									}
								}
							}
							GUILayout.EndVertical();
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();
				}
				return array;
			}
			else if (typeof(IList).IsAssignableFrom(type) && !typeof(Array).IsAssignableFrom(type)) {
				IList collection = (IList)value;
				Type[] list_gen_type = type.GetGenericArguments();
				// is IList
				if (list_gen_type.Length == 0) {
					GUILayout.BeginHorizontal();
					GUILayout.Label(content);
					GUILayout.FlexibleSpace();
					if (collection == null) {
						GUILayout.Label("[NULL]");
					}
					else {
						GUILayout.Label(string.Format("[{0}]", collection.Count));
					}
					GUILayout.EndHorizontal();
					return value;
				}
				// is List<T>
				else {
					Type list_type = list_gen_type[0];

					if (collection == null) {
						GUILayout.BeginHorizontal();
						GUILayout.Label(content, options);
						GUILayout.Label("[NULL]", options);
						GUILayout.EndHorizontal();
						if (GUILayout.Button("Create List")) {
							Type listType = typeof(List<>);
							Type constructedListType = listType.MakeGenericType(list_type);
							collection = (IList)Activator.CreateInstance(constructedListType);
						}
					}
					else {
						bool f;
						if (!foldouts.TryGetValue(value, out f)) {
							foldouts[value] = true;
						}
						GUILayout.BeginVertical("HelpBox", GUILayout.MinHeight(10.0f));
						{
							GUILayout.BeginHorizontal();
							{
								GUILayout.Space(10.0f);
								GUILayout.BeginVertical();
								{
									GUILayout.BeginHorizontal();
									if (collection.Count > 0) {
										foldouts[value] = EditorGUILayout.Foldout(foldouts[value], new GUIContent(string.Format("{0}: [{1}]", content.text, collection.Count), content.image, content.tooltip));
									}
									else {
										GUILayout.Label(new GUIContent(string.Format("{0}: [0]", content.text), content.image, content.tooltip), options);
									}

									GUILayout.FlexibleSpace();
									if (GUILayout.Button("+", "minibutton")) {
										if (list_type.IsAbstract || typeof(UnityEngine.Object).IsAssignableFrom(list_type)) {
											collection.Add(null);
										}
										else {
											collection.Add(Activator.CreateInstance(list_type));
										}
									}
									if (GUILayout.Button("x", "minibutton")) {
										if (collection.Count > 0) {
											collection.RemoveAt(collection.Count - 1);
										}
										else {
											return null;
										}
									}
									GUILayout.EndHorizontal();
									if (foldouts[value]) {
										for (int id = 0; id < collection.Count; id++) {
											collection[id] = AnyField(collection[id], list_type, new GUIContent(string.Format("Element {0}", id)), options);
										}
									}
								}
								GUILayout.EndVertical();
							}
							GUILayout.EndHorizontal();
						}
						GUILayout.EndVertical();
					}
				}
				return collection;
			}
			GUILayout.Label(string.Format("ERROR 404. {0} Field Not Found!", type.GetTypeName(true)));
			return value;
		}

		private static void ShowContentTypeMenu() {
			GenericMenu menu = new GenericMenu();
			foreach (Type type in ReflectionUtils.GetFullTypes()) {
				if (type == null) continue;
				string namespace_path = type.Namespace;
				if (namespace_path.IsNullOrEmpty()) {
					namespace_path = "Global";
				}
				else {
					namespace_path = namespace_path.Replace(".", "/");
				}
				string type_path = string.Format("{0}/{1}", namespace_path, type.GetTypeName(false, true));

				menu.AddItem(new GUIContent(type_path), false, () => {
					cached_value = type;
				});
			}
			menu.ShowAsContext();
		}

		private static Vector4 QuaternionToVector4(Quaternion rot) {
			return new Vector4(rot.x, rot.y, rot.z, rot.w);
		}

		private static Quaternion Vector4ToQuaternion(Vector4 pos) {
			return new Quaternion(pos.x, pos.y, pos.z, pos.w);
		}
	}
}
#endif
