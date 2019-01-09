using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Compare Int")]
	[Path("Functions/Operation/Integer/Compare")]
	[Body("Compare", "Integer")]
	public class IntCompare : ActionNode, IRegisterPorts {

		public InputValue<int> input_value;
		public InputValue<int> compare;
		public OutputAction on_equals;
		public OutputAction on_not_equals;
		public OutputAction on_great;
		public OutputAction on_less;

		public void OnRegisterPorts() {
			input_value = RegisterInputValue<int>("Input");
			compare = RegisterInputValue<int>("Compare With");

			on_equals = RegisterExitPort("==");
			on_not_equals = RegisterExitPort("!=");
			on_great = RegisterExitPort(">");
			on_less = RegisterExitPort("<");
		}

		public override void OnExecute() {
			if (input_value.value == compare.value) {
				Call(on_equals);
			}
			else {
				Call(on_not_equals);
			}
			if (input_value.value > compare.value) {
				Call(on_great);
			}
			if (input_value.value < compare.value) {
				Call(on_less);
			}
			Call(output);
		}
	}
}
