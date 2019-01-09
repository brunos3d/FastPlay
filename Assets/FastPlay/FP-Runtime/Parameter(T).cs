using System;

namespace FastPlay.Runtime {
	public class Parameter<T> : Parameter {

		public Parameter() { }

		public Parameter(string name) {
			this.name = name;
		}

		public override Type valueType {
			get {
				return typeof(T);
			}
		}
	}
}
