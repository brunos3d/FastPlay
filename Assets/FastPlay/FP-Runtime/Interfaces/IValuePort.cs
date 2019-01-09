using System;

namespace FastPlay.Runtime {
	public interface IValuePort : IPort {

		Type valueType { get; }

		object GetValue();
	}
}
