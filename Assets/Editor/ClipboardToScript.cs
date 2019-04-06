using UnityEditor;
using UnityEngine;
using System.IO;

public static class ClipboardToScript {

	static CreateNewFileWithName GetWindow() {
		return EditorWindow.GetWindowWithRect<CreateNewFileWithName>(new Rect(Screen.currentResolution.width / 2.0f - 100.0f, Screen.currentResolution.height / 3.0f - 24.0f, 200f, 68f));
	}

	[MenuItem("Assets/Create/From Clipboard/C# Script")]
	static void CreateScript() {
		GetWindow().fileType = FileType.CSharp;
	}

	[MenuItem("Assets/Create/From Clipboard/Shader")]
	static void CreateShader() {
		GetWindow().fileType = FileType.Shader;
	}

	[MenuItem("Assets/Create/From Clipboard/Text File")]
	static void CreateTextFile() {
		GetWindow().fileType = FileType.TXT;
	}
}

public enum FileType {
	CSharp, Shader, TXT
}

public class CreateNewFileWithName : EditorWindow {

	string m_FileName;
	bool didFocus = false;

	public FileType fileType { get; set; }

	private void OnEnable() {
		base.titleContent = new GUIContent("File Name");
		ShowAuxWindow();
	}

	private void OnGUI() {
		GUILayout.Space(5f);
		fileType = (FileType)EditorGUILayout.EnumPopup(fileType);
		Event current = Event.current;
		bool flag = (current.type == EventType.KeyDown) && ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter));
		GUI.SetNextControlName("m_PreferencesName");
		this.m_FileName = EditorGUILayout.TextField(this.m_FileName, new GUILayoutOption[0]);
		if (!this.didFocus) {
			this.didFocus = true;
			EditorGUI.FocusTextInControl("m_PreferencesName");
		}
		GUI.enabled = !string.IsNullOrEmpty(this.m_FileName);
		if (GUILayout.Button("Save", new GUILayoutOption[0]) || flag) {
			base.Close();
			string extension;
			switch (fileType) {
				case FileType.CSharp:
					extension = ".cs";
					break;
				case FileType.Shader:
					extension = ".shader";
					break;
				case FileType.TXT:
					extension = ".txt";
					break;
				default:
					extension = ".txt";
					break;

			}
			string path = Path.Combine(AssetDatabase.GetAssetPath(Selection.activeObject), this.m_FileName + extension);
			if (!File.Exists(path)) {
				using (StreamWriter sw = File.CreateText(path)) {
					//From Clipboard
					sw.Write(EditorGUIUtility.systemCopyBuffer);
				}
			}
			AssetDatabase.Refresh(); GUIUtility.ExitGUI();
		}
	}
}
