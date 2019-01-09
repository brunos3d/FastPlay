using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Branch")]
	[Path("Flow Control/Branch")]
	[Body("Branch", "Flow Control", "branch_node_icon")]
	public class Branch : ActionNode, IRegisterPorts {

		public OutputAction on_true;
		public OutputAction on_false;
		public InputValue<bool> condition;

		public void OnRegisterPorts() {
			on_true = RegisterExitPort("True");
			on_false = RegisterExitPort("False");
			condition = RegisterInputValue<bool>("condition");
		}

		public override void OnExecute() {
			if (condition.value) {
				Call(on_true);
			}
			else {
				Call(on_false);
			}
			Call(output);
		}
	}
}
