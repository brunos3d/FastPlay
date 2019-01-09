using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("ForLoopWithBreak")]
	[Path("Flow Control/ForLoopWithBreak")]
	[Body("ForLoopWithBreak", "Flow Control")]
	public class ForLoopWithBreak : ActionNode, IRegisterPorts {

		public int index;
		public bool broken;
		public OutputAction on_loop;
		public InputValue<int> first_index;
		public InputValue<int> last_index;

		public void OnRegisterPorts() {
			first_index = RegisterInputValue<int>("first index");
			last_index = RegisterInputValue<int>("last index");

			RegisterEntryPort("Break", () => { broken = true; });
			on_loop = RegisterExitPort("Loop");
			RegisterOutputValue<int>("index", () => { return index; });
		}

		public override void OnExecute() {
			broken = false;
			for (index = first_index.value; index < last_index.value; index++) {
				if (broken) {
					break;
				}
				Call(on_loop);
			}
			Call(output);
		}
	}
}
