using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ForLoop")]
	[Subtitle("Flow Control")]
	[Path("Flow Control/ForLoop")]
	public class ForLoop : ActionNode, IRegisterPorts {

		public int index;
		public OutputAction on_loop;
		public InputValue<int> first_index;
		public InputValue<int> last_index;

		public void OnRegisterPorts() {
			first_index = RegisterInputValue<int>("first index");
			last_index = RegisterInputValue<int>("last index");

			on_loop = RegisterExitPort("Loop");
			RegisterOutputValue<int>("index", () => { return index; });
		}

		public override void OnExecute() {
			for (index = first_index.value; index < last_index.value; index++) {
				Call(on_loop);
			}
			Call(output);
		}
	}
}
