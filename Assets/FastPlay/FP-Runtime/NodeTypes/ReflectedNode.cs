using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using FastPlay.Editor;
using UnityEngine;

namespace FastPlay.Runtime {
	[HideInList]
	public class ReflectedNode : Node, IRegisterDefaultPorts {

		public bool is_obsolete;

		public SerializedMethod serialized_method;

		[NonSerialized]
		public MethodInfo cached_method;

		[NonSerialized]
		public IInputValue target;

		[NonSerialized]
		public OutputAction output;

		[NonSerialized]
		List<IInputValue> parameters = new List<IInputValue>();

		public ReflectedNode() { }

		public ReflectedNode(MethodInfo method_info) {
			SetMethod(method_info);
		}

		public ReflectedNode(MethodInfo method_info, params Type[] type_args) {
			SetMethod(method_info, type_args);
		}

		public void OnRegisterDefaultPorts() {
			if (serialized_method == null) return;
			SetMethod(serialized_method.Deserialize());
		}

		public void SetMethod(MethodInfo method_info, params Type[] type_args) {
			if (method_info == null) {
#if UNITY_EDITOR
				node_color = Color.red;
				DisplayMessage("Missing Method", UnityEditor.MessageType.Error);
#endif
				Debug.LogError("type method_info is null!");
				return;
			}

			this.cached_method = method_info;

			if (type_args.IsNullOrEmpty()) {
				this.serialized_method = this.serialized_method ?? new SerializedMethod(method_info);
			}
			else {
				this.serialized_method = this.serialized_method ?? new SerializedMethod(method_info, type_args);
			}

			string method_name = cached_method.Name;
			this.name = method_name;

			RegisterEntryPort("In", Execute);
			this.output = RegisterExitPort("Out");

			if (method_info.ReturnType != typeof(void)) {
				RegisterOutputValue(method_info.ReturnType, "Get", Invoke);
			}
#if UNITY_EDITOR
			node_color = GUIReferrer.GetTypeColor(method_info.ReturnType);

			ObsoleteAttribute obsolete_flag = method_info.GetAttribute<ObsoleteAttribute>(false);
			if (obsolete_flag != null) {
				DisplayMessage(obsolete_flag.Message, UnityEditor.MessageType.Warning);
			}
#endif

			if (method_info.IsStatic) {
				this.target = null;
				string title = method_name;
				if (title.Contains("get_")) {
					title = title.Replace("get_", string.Empty);
				}
				else if (title.Contains("set_")) {
					title = title.Replace("set_", string.Empty);
				}
				this.title = title;
			}
			else {
				this.target = (IInputValue)RegisterInputValue(method_info.ReflectedType, "Target");
				string title = method_name;
				if (title.Contains("get_")) {
					title = /*"[Get] " + */title.Replace("get_", string.Empty).AddSpacesToSentence();
				}
				else if (title.Contains("set_")) {
					title = /*"[Set] " + */title.Replace("set_", string.Empty).AddSpacesToSentence();
				}
				this.title = title;
			}

			parameters = new List<IInputValue>();
			foreach (ParameterInfo parameter in method_info.GetParameters()) {
				parameters.Add((IInputValue)RegisterInputValue(parameter.ParameterType, parameter.Name.AddSpacesToSentence()));
			}
		}

		public void Execute() {
			Invoke();
			Call(output);
		}

		public object Invoke() {
			object[] args = parameters.Select(i => i.GetValue()).ToArray();
			if (cached_method.IsStatic) {
				return cached_method.Invoke(null, args);
			}
			else {
				return cached_method.Invoke(target.GetValue(), args);
			}
		}
	}
}
