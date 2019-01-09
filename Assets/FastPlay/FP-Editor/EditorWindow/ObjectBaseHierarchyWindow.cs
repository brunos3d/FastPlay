#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public class ObjectBaseHierarchyWindow : EditorWindow {

		Vector2 scroll = Vector2.zero;

		[NonSerialized]
		List<int> selection = new List<int>();

		[NonSerialized]
		List<ObjectBase> objs = new List<ObjectBase>();

		[MenuItem("Tools/FastPlay/ObjectBase Hierarchy")]
		static void Init() {
			GetWindow<ObjectBaseHierarchyWindow>();
		}

		void OnEnable() {
			objs.Clear();
			objs = ObjectBase.GetAllInstances();
		}

		void OnFocus() {
			OnEnable();
		}

		string lastFocused = null;
		void OnGUI() {
			Event current = Event.current;
			GUILayout.Space(18.0f);
			GUI.SetNextControlName("BeginScrollView");
			scroll = EditorGUILayout.BeginScrollView(scroll);
			GUILayout.Space(5.0f);
			for (int id = 0; id < objs.Count; id++) {
				ObjectBase obj = objs[id];
				if (obj) {
					bool toggle = selection.Contains(obj.GetInstanceID());
					string content = string.IsNullOrEmpty(obj.name) ? obj.GetType().Name : obj.name;

					if (toggle == true) {
						GUI.FocusControl(content + obj.GetInstanceID());
					}

					GUI.SetNextControlName(content + obj.GetInstanceID());
					GUILayout.BeginHorizontal();
					if (GUILayout.Toggle(toggle, new GUIContent(content), (GUIStyle)"MenuItem") != toggle) {
						if (current.control || current.command) {
							if (toggle == false) { // if not contains
								lastFocused = content + obj.GetInstanceID();
							}
							else { // if contains
								lastFocused = null;
							}
						}
						else {
							lastFocused = content + obj.GetInstanceID();
						}
						Repaint();
					}
					GUILayout.FlexibleSpace();
					GUI.SetNextControlName(content + obj.GetInstanceID());
					GUILayout.Label(new GUIContent(obj.id.ToString()), (GUIStyle)"MenuItem");
					GUILayout.EndHorizontal();
					if (toggle == false) {
						GUI.FocusControl(lastFocused);
					}
				}
			}
			GUILayout.Space(5.0f);
			EditorGUILayout.EndScrollView();
			DrawToolbar();
		}

		void DrawToolbar() {
			GUILayout.BeginArea(new Rect(0.0f, 0.0f, position.width, 18.0f));
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private T[] AddItemToArray<T>(T[] array, T item) {
			List<T> list = array.ToList<T>();
			list.Add(item);
			return list.ToArray<T>();
		}

		private T[] RemoveItemFromArray<T>(T[] array, T item) {
			List<T> list = array.ToList<T>();
			list.Remove(item);
			return list.ToArray<T>();
		}
	}
}
#endif
