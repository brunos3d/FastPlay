using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract class ActionNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		public InputAction input;

		[NonSerialized]
		public OutputAction output;

		protected virtual bool useIn { get { return true; } }

		protected virtual bool useOut { get { return true; } }

		public ActionNode() { }

		public void OnRegisterDefaultPorts() {
			if (useIn) {
				input = RegisterEntryPort("In", OnExecute);
			}
			if (useOut) {
				output = RegisterExitPort("Out");
			}
		}

		public virtual void OnExecute() { }
	}
}
