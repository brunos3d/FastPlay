#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace FastPlay.Editor {
	public class TestAdvancedDropDownWindow : EditorWindow {

		public static object instance;

		public static Type m_type;

		public static Type type {
			get {
				return m_type ?? (m_type = (typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.AdvancedDropdownWindow") ?? ReflectionUtils.GetTypeByName("AdvancedDropdownWindow")));
			}
		}

		[MenuItem("Tools/FastPlay/AdvancedDropDown")]
		public static TestAdvancedDropDownWindow Init() {
			instance = Invoke(null, "CreateAndInit", BindingFlags.Public | BindingFlags.Static, Rect.zero);
			return GetWindow<TestAdvancedDropDownWindow>();
		}

		void OnGUI() {
			if (instance != null) {
				Invoke(instance, "OnGUI", BindingFlags.NonPublic | BindingFlags.Instance);
			}
		}

		static object Invoke(object target, string method_name, params object[] args) {
			MethodInfo method = type.GetMethod(method_name);
			if (method != null) {
				if (method.ContainsGenericParameters) {
					return method.MakeGenericMethod(type).Invoke(target, args);
				}
				else {
					return method.Invoke(target, args);
				}
			}
			return null;
		}

		static object Invoke(object target, string method_name, BindingFlags flags, params object[] args) {
			MethodInfo method = type.GetMethod(method_name, flags);
			if (method != null) {
				if (method.ContainsGenericParameters) {
					return method.MakeGenericMethod(type).Invoke(target, args);
				}
				else {
					return method.Invoke(target, args);
				}
			}
			return null;
		}
	}
}
#endif
