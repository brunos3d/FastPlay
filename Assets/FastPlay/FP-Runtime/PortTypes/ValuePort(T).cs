using System;

namespace FastPlay.Runtime {
	public class ValuePort<T> : ValuePort {

		public override Type valueType { get { return typeof(T); } }

		public ValuePort() { }
	}
}
