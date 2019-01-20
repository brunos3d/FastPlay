using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using FastPlay;
using System.Linq;

public class AssistantWindow : EditorWindow {

	private string text;

	private Vector2 scroll;

	private const int kMaxChars = 7000;

	[MenuItem("Tools/Assistant")]
	static void Init() {
		AssistantWindow window = GetWindow<AssistantWindow>();
		window.Show();
	}

	void OnGUI() {
		if (GUILayout.Button("Generate DeafaultTypes")) {
			text = string.Empty;
			var types = ReflectionUtils.GetTypesWithNamespace("UnityEngine");
			types.Sort((t1, t2) => string.Compare(t1.Name, t2.Name));
			foreach (Type type in types) {
				if (type.IsNotPublic) continue;
				text += "typeof(#TYPE#),";
				text = text.Replace("#TYPE#", type.GetTypeName(true));
				text += Environment.NewLine;
			}
			FastPlay.Editor.EditorUtils.CopyText(text);
			Debug.Log("Copied to clipboard");
			if (text.Length > kMaxChars) {
				text = text.Substring(0, kMaxChars) + "...\n\n<...etc...>";
			}
		}

		if (GUILayout.Button("Generate Icons")) {
			text = string.Empty;
			foreach (Type type in ReflectionUtils.GetFullTypes().Where(t => t.IsSubclassOf(typeof(Component)))) {
				if (type.IsNotPublic || type.HasAttribute<ObsoleteAttribute>(false)) continue;
				text += @"{ typeof(#TYPE#), ""#ICONNAME# Icon"" },";
				text = text.Replace("#TYPE#", type.GetTypeName(true));
				text = text.Replace("#ICONNAME#", type.GetTypeName());
				text += Environment.NewLine;
			}
			FastPlay.Editor.EditorUtils.CopyText(text);
			Debug.Log("Copied to clipboard");
			if (text.Length > kMaxChars) {
				text = text.Substring(0, kMaxChars) + "...\n\n<...etc...>";
			}
		}
		scroll = EditorGUILayout.BeginScrollView(scroll);
		EditorGUILayout.TextArea(text);
		EditorGUILayout.EndScrollView();
	}
}