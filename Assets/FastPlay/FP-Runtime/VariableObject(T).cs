using System;
using System.Collections;
using System.Collections.Generic;

namespace FastPlay.Runtime {
	public class VariableObject<T> : VariableObject {

		public T value;

		public VariableObject() { }

		public VariableObject(int key, string name, bool is_public = true) {
			this.key = key;
			this.name = name;
			this.is_public = is_public;
			if (typeof(T) is IList) {
				Type listType = typeof(List<>);
				Type constructedListType = listType.MakeGenericType(value.GetType().GetGenericArguments());

				value = (T)Activator.CreateInstance(constructedListType);
			}
		}

		public override Type valueType {
			get {
				return typeof(T);
			}
		}

		public override object GetValue() {
			return value;
		}

		public override void SetValue(object value) {
			this.value = (T)value;
		}
	}
}
