using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("WhileLoop")]
	[Path("Flow Control/WhileLoop")]
	[Body("WhileLoop", "Flow Control")]
	public class WhileLoop : ActionNode, IRegisterPorts {

		public OutputAction on_loop;

		public InputValue<bool> condition;

		public void OnRegisterPorts() {
			condition = RegisterInputValue<bool>("condition");
			on_loop = RegisterExitPort("Loop");
		}

		public override void OnExecute() {
			while (condition.value) {
				Call(on_loop);
			}
			Call(output);
		}
	}
}
