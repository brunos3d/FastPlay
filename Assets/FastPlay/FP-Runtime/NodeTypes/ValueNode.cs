using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract partial class ValueNode : Node, IValueNode {

		[NonSerialized]
		public OutputAction output;

		protected virtual bool useGet { get { return true; } }

		protected virtual bool useSet { get { return false; } }

		protected virtual bool displayGet { get { return true; } }

		protected virtual bool displaySet { get { return true; } }

		protected virtual string getDefaultName { get { return "Get"; } }

		protected virtual string setDefaultName { get { return "Set"; } }

		public virtual Type valueType { get { return default(Type); } }

		public virtual Type[] valueTypes { get { return new Type[] { valueType }; } }

		public ValueNode() { }

		public virtual object GetValue() { return null; }

		public virtual void SetValue(object value) { }
	}
}
