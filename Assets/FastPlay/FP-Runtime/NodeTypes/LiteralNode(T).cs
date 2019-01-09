using System;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[HideInList]
	public class LiteralNode<T> : LiteralNode, IRegisterDefaultPorts {

		public InputValue<T> value;

		public override Type valueType { get { return typeof(T); } }

		public LiteralNode() { }

		public void OnRegisterDefaultPorts() {
			//Only inspector 
			this.value = RegisterInputValue<T>("Value", default(T), false);
			RegisterOutputValue<T>("Get", OnGetValue);
		}

		public override object GetValue() {
			return OnGetValue();
		}

		public virtual T OnGetValue() {
			return this.value.value;
		}
	}
}
