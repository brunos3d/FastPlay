using System;

namespace FastPlay.Runtime {
	public interface IValueNode {

		Type valueType { get; }

		object GetValue();

		void SetValue(object value);
	}
}
