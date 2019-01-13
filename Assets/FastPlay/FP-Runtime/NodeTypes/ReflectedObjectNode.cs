using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[HideInList]
	public class ReflectedObjectNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		public Type cached_type;

		[NonSerialized]
		public IInputValue target;

		[NonSerialized]
		public Dictionary<string, OutputAction> act_outputs = new Dictionary<string, OutputAction>();

		[NonSerialized]
		public Dictionary<string, MethodInfo> cached_methods = new Dictionary<string, MethodInfo>();

		[NonSerialized]
		public Dictionary<string, List<IInputValue>> parameters = new Dictionary<string, List<IInputValue>>();

		public bool use_private = false;

		public bool use_static = false;

		public bool use_inherit = false;

		public SerializedType serialized_type;

		public Dictionary<string, SerializedMethod> serialized_methods = new Dictionary<string, SerializedMethod>();

		public ReflectedObjectNode() { }

		public ReflectedObjectNode(Type type) {
			SetObject(type);
		}

		public void OnRegisterDefaultPorts() {
			if (serialized_type == null) return;
			SetObject(serialized_type.Deserialize());
		}

		public void SetObject(Type type) {
			if (type == null) {
				Debug.LogError("type parameter is null!");
				return;
			}
			this.cached_type = type;

			Type[] type_args = type.GetGenericArguments();

			serialized_type = serialized_type ?? (serialized_type = new SerializedType(type, type_args));

			this.name = type.GetTypeName(true);
			this.title = type.GetTypeName();

			if (!type.IsStatic()) {
				this.target = (IInputValue)RegisterInputValue(type, "Target");
			}

			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | (use_inherit ? BindingFlags.Default : BindingFlags.DeclaredOnly) | (use_private ? BindingFlags.NonPublic : BindingFlags.Default) | (use_static ? BindingFlags.Static : BindingFlags.Default);
			int index = 0;
			foreach (MethodInfo method_info in type.GetMethods(flags)) {
				string key = method_info.GetSignName();
				string method_name = method_info.Name;

				SerializedMethod sm;
				if (!this.serialized_methods.TryGetValue(key, out sm)) {
					if (type_args.IsNullOrEmpty()) {
						this.serialized_methods[key] = new SerializedMethod(method_info);
					}
					else {
						this.serialized_methods[key] = new SerializedMethod(method_info, type_args);
					}
				}

				if (method_info.ReturnType == typeof(void)) {
					RegisterEntryPort(string.Format("{0}. {1}", index, method_name), () => { Execute(key); });
					this.act_outputs[key] = RegisterExitPort(string.Format("{0}. Out", index));
				}
				else {
					RegisterOutputValue(method_info.ReturnType, string.Format("{0}. {1}", index, method_name), () => { return Invoke(key); });
				}

				parameters[key] = new List<IInputValue>();
				foreach (ParameterInfo parameter in method_info.GetParameters()) {
					parameters[key].Add((IInputValue)RegisterInputValue(parameter.ParameterType, string.Format("{0}. {1}", index, parameter.Name.AddSpacesToSentence())));
				}
				index++;
			}
		}

		public void Execute(string key) {
			Invoke(key);
			Call(act_outputs[key]);
		}

		public object Invoke(string key) {
			object[] args = parameters[key].Select(i => i.GetValue()).ToArray();
			MethodInfo m;
			if (cached_methods.TryGetValue(key, out m)) {
				if (m.IsStatic) {
					return m.Invoke(null, args);
				}
				else {
					return m.Invoke(target.GetValue(), args);
				}
			}
			return null;
		}
	}
}
