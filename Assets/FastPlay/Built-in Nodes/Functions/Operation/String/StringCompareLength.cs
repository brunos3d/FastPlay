using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Compare Length")]
	[Path("Functions/Operation/String/Compare Length")]
	[Body("Compare Length", "String")]
	public class StringCompareLength : ActionNode, IRegisterPorts {

		public InputValue<string> input_value;
		public InputValue<string> compare;
		public OutputAction on_equals;
		public OutputAction on_great;
		public OutputAction on_less;

		public void OnRegisterPorts() {
			input_value = RegisterInputValue<string>("Input");
			compare = RegisterInputValue<string>("Compare With");

			on_equals = RegisterExitPort("==");
			on_great = RegisterExitPort(">");
			on_less = RegisterExitPort("<");
		}

		public override void OnExecute() {
			if (input_value.value.Length == compare.value.Length) {
				Call(on_equals);
			}
			if (input_value.value.Length > compare.value.Length) {
				Call(on_great);
			}
			if (input_value.value.Length < compare.value.Length) {
				Call(on_less);
			}
			Call(output);
		}
	}
}
