using System;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[HideInList]
	public class LiteralNode<T> : Node, IValueNode, IRegisterDefaultPorts {

		public InputValue<T> value;

		public Type valueType { get { return typeof(T); } }

		public LiteralNode() { }

		public void OnRegisterDefaultPorts() {
			//Only inspector 
			this.value = RegisterInputValue<T>("Value", default(T), false);
			RegisterOutputValue<T>("Get", OnGetValue);
		}

		public object GetValue() {
			return OnGetValue();
		}

		public virtual T OnGetValue() {
			return this.value.value;
		}

		public void SetValue(object value) {
			this.value.value = (T)value;
		}
	}
}
