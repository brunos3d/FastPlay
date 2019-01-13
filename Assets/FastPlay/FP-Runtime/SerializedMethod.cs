using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastPlay.Runtime {
	[Serializable]
	public class SerializedMethod {

		public bool is_generic;

		public string method_key;

		public string base_type_name;

		public List<string> generic_args = new List<string>();

		[NonSerialized]
		public MethodInfo method_info;

		public SerializedMethod() { }

		public SerializedMethod(MethodInfo method) {
			this.base_type_name = method.ReflectedType.AssemblyQualifiedName;
			this.method_key = ReflectionUtils.GetMethodInfoKey(method);
			method_info = method;
		}

		public SerializedMethod(MethodInfo method, params Type[] type_args) {
			this.is_generic = !type_args.IsNullOrEmpty();
			this.base_type_name = method.ReflectedType.AssemblyQualifiedName;
			this.method_key = ReflectionUtils.GetMethodInfoKey(method);
			this.method_info = method;
			this.generic_args = new List<string>();
			foreach (Type type in type_args) {
				generic_args.Add(type.AssemblyQualifiedName);
			}
		}

		public MethodInfo Deserialize() {
			if (this.is_generic) {
				List<Type> type_args = new List<Type>();
				foreach (string arg in generic_args) {
					type_args.Add(ReflectionUtils.GetTypeByName(base_type_name));
				}
				return this.method_info ?? (this.method_info = ReflectionUtils.GetMethodInfoByKey(ReflectionUtils.GetTypeByName(base_type_name), method_key)).MakeGenericMethod(type_args.ToArray());
			}
			else {
				return this.method_info ?? (this.method_info = ReflectionUtils.GetMethodInfoByKey(ReflectionUtils.GetTypeByName(base_type_name), method_key));
			}
		}
	}
}
