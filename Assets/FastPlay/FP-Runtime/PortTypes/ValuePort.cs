using System;

namespace FastPlay.Runtime {
	public abstract class ValuePort : Port {

		public virtual Type valueType { get { return null; } }

		public ValuePort() { }
	}
}
