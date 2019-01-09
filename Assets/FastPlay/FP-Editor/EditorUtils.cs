#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEditor;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public static class EditorUtils {

		public static void CopyText(string text) {
			TextEditor editor = new TextEditor();
			editor.text = text;
			editor.SelectAll();
			editor.Copy();
		}

		public static void RepaintInspector() {
			Type InspWindow = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
			MethodInfo RepaintWindow = InspWindow.GetMethod("Repaint", BindingFlags.Public | BindingFlags.Instance);
			UnityEngine.Object InspWindowInstance = null;
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
			UnityEngine.Object PrefsWindowInstance = null;
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

		public static T FindAssetByName<T>(string name) where T : UnityEngine.Object {
			if (name.IsNullOrEmpty()) return null;
			if (Resources.Load<T>(name)) {
				return Resources.Load<T>(name);
			}

			T[] assets = (T[])Resources.FindObjectsOfTypeAll<T>();
			foreach (T asset in assets) {
				if (asset.name == name) {
					return asset;
				}
			}
			string[] GUIDs = AssetDatabase.FindAssets(name);
			foreach (string GUID in GUIDs) {
				if (AssetDatabase.GUIDToAssetPath(GUID).Contains(name)) {
					return (T)AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(GUID));
				}
			}
			return null;
		}

		public static bool OpenScriptByType(Type type) {
			UnityEngine.Object search = FindScriptByType(type);
			if (search != null) {
				AssetDatabase.OpenAsset(search);
				return true;
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
			GraphAsset assetData = null;
			var path = EditorUtility.SaveFilePanelInProject("Create GraphAsset", "New GraphAsset.asset", "asset", string.Empty);
			assetData = CreateGraphAsset(path);
			return assetData;
		}

		public static GraphAsset CreateGraphAsset(string path) {
			if (path.IsNullOrEmpty() == false) {
				ScriptableObject assetData = ScriptableObject.CreateInstance(typeof(GraphAsset));
				AssetDatabase.CreateAsset(assetData, path);
				AssetDatabase.SaveAssets();
				return (GraphAsset)assetData;
			}
			return null;
		}
	}
}
#endif
