using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[HideInList]
	public class ReflectedNode : Node, IRegisterDefaultPorts {

		public bool is_generic;

		public SerializedMethod method;

		[NonSerialized]
		public OutputAction output;

		[NonSerialized]
		public MethodInfo method_info;

		[NonSerialized]
		public IInputValue target;

		List<IInputValue> parameters = new List<IInputValue>();

		public ReflectedNode() { }

		public void OnRegisterDefaultPorts() {
			if (method == null) return;
			SetMethod(method.Deserialize());
		}

		public void SetMethod(MethodInfo method, params Type[] type_args) {
			if (method == null) return;
			this.method_info = method;
			if (type_args.IsNullOrEmpty()) {
				this.method = this.method ?? new SerializedMethod(method);
			}
			else {
				this.method = this.method ?? new SerializedMethod(method, type_args);
			}

			RegisterEntryPort("In", Execute);
			this.output = RegisterExitPort("Out");

			if (method.ReturnType != typeof(void)) {
				RegisterOutputValue(method.ReturnType, "Get", Invoke);
			}
#if UNITY_EDITOR
			node_color = GUIReferrer.GetTypeColor(method.ReturnType);
#endif

			if (method.IsStatic) {
				this.target = null;
				string new_name = method.Name;
				if (new_name.Contains("get_")) {
					new_name = new_name.Replace("get_", string.Empty);
					//this.name = string.Format("[Get] {0}.{1}", method.DeclaringType.GetTypeName(), new_name);
				}
				else if (new_name.Contains("set_")) {
					new_name = new_name.Replace("set_", string.Empty);
					//this.name = string.Format("[Set] {0}.{1}", method.DeclaringType.GetTypeName(), new_name);
				}
				this.name = new_name;
			}
			else {
				this.target = (IInputValue)RegisterInputValue(method.ReflectedType, "Target");
				string new_name = method.Name;
				if (new_name.Contains("get_")) {
					new_name = /*"[Get] " + */new_name.Replace("get_", string.Empty).AddSpacesToSentence();
				}
				else if (new_name.Contains("set_")) {
					new_name = /*"[Set] " + */new_name.Replace("set_", string.Empty).AddSpacesToSentence();
				}
				this.name = new_name;
			}

			parameters = new List<IInputValue>();
			foreach (ParameterInfo parameter in method.GetParameters()) {
				parameters.Add((IInputValue)RegisterInputValue(parameter.ParameterType, parameter.Name.AddSpacesToSentence()));
			}
		}

		public void Execute() {
			Invoke();
			Call(output);
		}

		public object Invoke() {
			object[] args = parameters.Select(i => i.GetValue()).ToArray();
			if (method_info.IsStatic) {
				return method_info.Invoke(null, args);
			}
			else {
				return method_info.Invoke(target.GetValue(), args);
			}
		}
	}
}
