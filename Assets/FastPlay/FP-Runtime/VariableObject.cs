using System;

namespace FastPlay.Runtime {
	public abstract class VariableObject : ObjectBase, IVariable {

		public int key;

		public bool is_public = true;

		public virtual Type valueType {
			get {
				return GetType();
			}
		}

		public virtual object GetValue() {
			return null;
		}

		public virtual int GetVariableKey() {
			return key;
		}

		public virtual string GetVariableName() {
			return name;
		}

		public virtual void SetValue(object value) {
		}
	}
}
