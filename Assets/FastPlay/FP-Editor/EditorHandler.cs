#if UNITY_EDITOR
using FastPlay.Runtime;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace FastPlay.Editor {
	public static class EditorHandler {

		private static ConstantTypes instanceConstantTypes;

		[OnOpenAsset]
		public static bool GraphAssetOpenEditor(int instance_ID, int line) {
			GraphAsset asset = EditorUtility.InstanceIDToObject(instance_ID) as GraphAsset;
			if (asset) {
				GraphEditorWindow.OpenEditor(asset);
				return true;
			}
			return false;
		}

		public static ConstantTypes GetConstantTypesCurrentInstance() {
			instanceConstantTypes = AssetDatabase.LoadAssetAtPath<ConstantTypes>("Assets/FastPlay/FP-Runtime/ConstantTypes.asset");
			if (instanceConstantTypes) {
				return instanceConstantTypes;
			}
			instanceConstantTypes = ScriptableObject.CreateInstance<ConstantTypes>();
			AssetDatabase.CreateAsset(instanceConstantTypes, "Assets/FastPlay/FP-Runtime/ConstantTypes.asset");
			return instanceConstantTypes;
		}
	}
}
#endif
