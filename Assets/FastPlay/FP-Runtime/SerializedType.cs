using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastPlay.Runtime {
	[Serializable]
	public class SerializedType {

		public bool is_generic;

		public string type_name;

		public List<SerializedType> generic_args = new List<SerializedType>();

		[NonSerialized]
		public Type type;

		public SerializedType() { }

		public SerializedType(Type type) {
			this.type_name = type.AssemblyQualifiedName;
			this.type = type;
		}

		public SerializedType(Type type, params Type[] type_args) {
			this.is_generic = !type_args.IsNullOrEmpty();
			this.type_name = type.AssemblyQualifiedName;
			this.type = type;
			this.generic_args = new List<SerializedType>();
			foreach (Type t in type_args) {
				generic_args.Add(new SerializedType(t, t.GetGenericArguments()));
			}
		}

		public Type Deserialize() {
			if (type == null && this.is_generic) {
				List<Type> type_args = new List<Type>();
				foreach (SerializedType arg in generic_args) {
					type_args.Add(arg.Deserialize());
				}
				return (type = ReflectionUtils.GetTypeByName(type_name).MakeGenericType(type_args.ToArray()));
			}
			return type ?? (type = ReflectionUtils.GetTypeByName(type_name));
		}
	}
}
