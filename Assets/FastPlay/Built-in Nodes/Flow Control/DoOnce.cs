using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("DoOnce")]
	[Path("Flow Control/DoOnce")]
	[Body("DoOnce", "Flow Control")]
	public class DoOnce : ActionNode, IRegisterPorts {

		public bool completed;
		public InputAction reset;
		public OutputAction on_completed;

		public void OnRegisterPorts() {
			reset = RegisterEntryPort("Reset", Reset);
			on_completed = RegisterExitPort("Completed");
		}

		public void Reset() {
			completed = false;
		}

		public override void OnExecute() {
			if (!completed) {
				Call(on_completed);
				completed = true;
			}
			Call(output);
		}
	}
}
