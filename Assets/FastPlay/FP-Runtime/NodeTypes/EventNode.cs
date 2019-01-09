using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract class EventNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		public OutputAction output;

		public virtual bool useOut { get { return true; } }

		public EventNode() { }

		public void OnRegisterDefaultPorts() {
			if (useOut) {
				output = RegisterExitPort("Out");
			}
		}
	}
}
