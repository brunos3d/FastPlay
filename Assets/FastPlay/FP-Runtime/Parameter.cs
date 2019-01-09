using System;

namespace FastPlay.Runtime {
	public abstract class Parameter : ObjectBase {

		public bool is_public = true;

		public virtual Type valueType {
			get {
				return GetType();
			}
		}
	}
}
