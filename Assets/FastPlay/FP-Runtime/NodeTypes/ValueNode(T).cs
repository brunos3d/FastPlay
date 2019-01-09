using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract class ValueNode<T> : ValueNode, IRegisterDefaultPorts {

		[NonSerialized]
		public InputValue<T> value_set;

		public override Type valueType {
			get {
				return typeof(T);
			}
		}

		public ValueNode() { }

		public void OnRegisterDefaultPorts() {
			if (useSet) {
				RegisterEntryPort(setDefaultName, () => { SetValue(value_set.value); }, displaySet);
				output = RegisterExitPort("Out", displaySet);
				value_set = RegisterInputValue<T>("Value", default(T), displaySet);
			}
			if (useGet) {
				RegisterOutputValue<T>(getDefaultName, OnGetValue, displayGet);
			}
		}

		public override object GetValue() {
			return OnGetValue();
		}

		public override void SetValue(object value) {
			OnSetValue((T)value);
			Call(output);
		}

		public virtual T OnGetValue() { return default(T); }

		public virtual void OnSetValue(T value) { }
	}
}
