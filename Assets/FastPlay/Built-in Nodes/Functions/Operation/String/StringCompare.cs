using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Compare String")]
	[Path("Functions/Operation/String/Compare")]
	[Body("Compare", "String")]
	public class StringCompare : ActionNode, IRegisterPorts {

		public InputValue<string> input_value;
		public InputValue<string> compare;

		public OutputAction on_equals;
		public OutputAction on_not_equals;

		public void OnRegisterPorts() {
			input_value = RegisterInputValue<string>("Input");
			compare = RegisterInputValue<string>("Compare With");

			on_equals = RegisterExitPort("==");
			on_not_equals = RegisterExitPort("!=");
		}

		public override void OnExecute() {
			if (input_value.value == compare.value) {
				Call(on_equals);
			}
			else {
				Call(on_not_equals);
			}
			Call(output);
		}
	}
}
