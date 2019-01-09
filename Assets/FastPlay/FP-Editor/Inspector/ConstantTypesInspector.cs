#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace FastPlay.Editor {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ConstantTypes), true)]
	public class ConstantTypesInspector : UnityEditor.Editor {

		private GenericMenu generic_menu;

		private ReorderableList list;

		private void OnEnable() {
			CreateTypeList();
			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		private void OnDisable() {
			Undo.undoRedoPerformed -= UndoRedoPerformed;
		}

		private void UndoRedoPerformed() {
			CreateTypeList();
		}

		public override void OnInspectorGUI() {
			ConstantTypes types = (ConstantTypes)target;
			if (GUILayout.Button("Reset to defaults")) {
				UndoManager.RecordObject(target, "Reset to defaults");
				types.current_types = ConstantTypes.default_types;
				CreateTypeList();
			}

			list.DoLayoutList();

			if (GUI.changed) {
				UndoManager.SetDirty(target);
				GUI.changed = true;
			}
		}

		private void CreateTypeList() {
			ConstantTypes types = (ConstantTypes)target;
			list = new ReorderableList(types.current_types, typeof(Type), true, true, true, true);

			list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, "Current Types");
			};

			list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				//rect.y += 2;
				Type last_value = GUIDraw.AnyField(rect, types.current_types[index]);
				if (last_value != types.current_types[index]) {
					UndoManager.RecordObject(target, "Type Change");
					types.current_types[index] = last_value;
				}
				//GUI.Label(rect, types.current_types[index].GetTypeName(true));
			};

			list.onRemoveCallback = (ReorderableList list) => {
				UndoManager.RecordObject(target, "Remove type");
				types.current_types.RemoveAt(list.index);
				list.index = list.count - 1;
			};

			list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
				if (generic_menu == null) {
					generic_menu = new GenericMenu();
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

						generic_menu.AddItem(new GUIContent(type_path), false, () => {
							UndoManager.RecordObject(target, "Add new type");
							types.current_types.Add(type);
							CreateTypeList();
						});
					}
				}
				generic_menu.ShowAsContext();
			};
		}
	}
}
#endif
