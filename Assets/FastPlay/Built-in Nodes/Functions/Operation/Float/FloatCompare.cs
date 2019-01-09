using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Compare")]
	[Subtitle("Float")]
	[Path("Functions/Operation/Float/Compare")]
	public class FloatCompare : ActionNode, IRegisterPorts {

		public InputValue<float> input_value;
		public InputValue<float> compare;
		public OutputAction on_equals;
		public OutputAction on_not_equals;
		public OutputAction on_great;
		public OutputAction on_less;

		public void OnRegisterPorts() {
			input_value = RegisterInputValue<float>("Input");
			compare = RegisterInputValue<float>("Compare With");

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
