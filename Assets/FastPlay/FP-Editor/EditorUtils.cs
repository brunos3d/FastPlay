#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using FastPlay.Runtime;
using System.Collections.Generic;

namespace FastPlay.Editor {
	public static class EditorUtils {

		public static Dictionary<string, object> cached_assets = new Dictionary<string, object>();

		public static void CopyText(string text) {
			TextEditor editor = new TextEditor();
			editor.text = text;
			editor.SelectAll();
			editor.Copy();
		}

		public static void RepaintInspector() {
			Type InspWindow = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
			MethodInfo RepaintWindow = InspWindow.GetMethod("Repaint", BindingFlags.Public | BindingFlags.Instance);
			UnityObject InspWindowInstance = null;
			if (Resources.FindObjectsOfTypeAll(InspWindow).Length == 0) {
				return;
				//ShowWindow.Invoke(null, null);
				//InspWindowInstance = Resources.FindObjectsOfTypeAll(InspWindow)[0];
			}
			else {
				//InspWindowInstance = EditorWindow.GetWindow(InspWindow);
				InspWindowInstance = Resources.FindObjectsOfTypeAll(InspWindow)[0];
			}
			if (InspWindowInstance) {
				RepaintWindow.Invoke(InspWindowInstance, null);
			}
		}

		public static void OpenPreferencesItem(string ItemName) {
			Type PrefsWindow = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.PreferencesWindow");
			MethodInfo ShowPreferencesWindow = PrefsWindow.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static);
			MethodInfo AddCustomSections = PrefsWindow.GetMethod("AddCustomSections", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo Sections = PrefsWindow.GetField("m_Sections", BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo RefreshCustomPreferences = PrefsWindow.GetField("m_RefreshCustomPreferences", BindingFlags.NonPublic | BindingFlags.Instance);
			int index = 0;
			UnityObject PrefsWindowInstance = null;
			if (Sections != null) {
				if (Resources.FindObjectsOfTypeAll(PrefsWindow).Length == 0) {
					ShowPreferencesWindow.Invoke(null, null);
					PrefsWindowInstance = Resources.FindObjectsOfTypeAll(PrefsWindow)[0];
					AddCustomSections.Invoke(PrefsWindowInstance, null);
				}
				else {
					PrefsWindowInstance = EditorWindow.GetWindow(PrefsWindow);
				}
				RefreshCustomPreferences.SetValue(PrefsWindowInstance, false);
				IList ListSections = Sections.GetValue(PrefsWindowInstance) as IList;
				for (int id = 0; id < ListSections.Count; id++) {
					var section = ListSections[id];
					if (section != null) {
						GUIContent content = (GUIContent)section.GetType().GetField("content", BindingFlags.Public | BindingFlags.Instance).GetValue(section);
						if (content != null) {
							if (content.text.ToLower() == ItemName.ToLower()) {
								index = id;
								break;
							}
						}
					}
				}
			}
			if (PrefsWindowInstance != null) {
				PrefsWindow.GetProperty("selectedSectionIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(PrefsWindowInstance, index, null);
			}
		}

		public static T FindAssetByName<T>(string name) where T : UnityObject {
			if (name.IsNullOrEmpty()) return null;
			object o;
			if (cached_assets.TryGetValue(name, out o)) {
				return (T)o;
			}
			T result = Resources.Load<T>(name);
			if (result) {
				cached_assets[name] = result;
				return result;
			}

			T[] assets = (T[])Resources.FindObjectsOfTypeAll<T>();
			foreach (T asset in assets) {
				if (asset.name == name) {
					cached_assets[name] = asset;
					return asset;
				}
			}
			string[] GUIDs = AssetDatabase.FindAssets(name);
			foreach (string GUID in GUIDs) {
				if (AssetDatabase.GUIDToAssetPath(GUID).Contains(name)) {
					T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(GUID));
					cached_assets[name] = asset;
					return asset;
				}
			}
			return null;
		}

		public static bool OpenScriptByType(Type type) {
			UnityObject search = FindScriptByType(type);
			if (search != null) {
				return AssetDatabase.OpenAsset(search);
			}
			return false;
		}

		public static MonoScript FindScriptByType(Type type) {
			foreach (MonoScript mono in Resources.FindObjectsOfTypeAll<MonoScript>()) {
				if (mono.GetClass() == type) {
					return mono;
				}
			}
			return FindAssetByName<MonoScript>(type.Name);
		}

		public static GraphAsset CreateGraphAsset() {
			return CreateGraphAsset(EditorUtility.SaveFilePanelInProject("Create GraphAsset", "New GraphAsset.asset", "asset", string.Empty));
		}

		public static GraphAsset CreateGraphAsset(string path) {
			if (path.IsNullOrEmpty()) return null;
			ScriptableObject asset_data = ScriptableObject.CreateInstance(typeof(GraphAsset));
			AssetDatabase.CreateAsset(asset_data, path);
			AssetDatabase.SaveAssets();
			return (GraphAsset)asset_data;
		}
	}
}
#endif
