#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using FastPlay.Runtime;

namespace FastPlay.Editor {
	public static class UndoManager {

		public static GraphAsset asset_undo;

		public static void SetDirty(Object target) {
			if (Application.isPlaying || target == null) {
				return;
			}

			//Debug.Log("SetDirty");
			EditorUtility.SetDirty(target);
		}

		public static T AddComponent<T>(GameObject obj) where T : Component {
			if (obj == null) {
				return default(T);
			}

			//Debug.Log("AddComponent");
			return Undo.AddComponent<T>(obj);
		}

		public static void RecordObject(Object target, string name) {
			if (Application.isPlaying || target == null) {
				return;
			}
			//new log("RecordObject: " + name);
			if (target is GraphAsset) {
				asset_undo = (GraphAsset)target;
			}
			Undo.RecordObject(target, name);
		}
	}
}
#endif
