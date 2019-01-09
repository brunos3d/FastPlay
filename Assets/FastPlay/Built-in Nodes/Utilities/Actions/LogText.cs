using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("LogText")]
	[Path("Utilities/Actions/LogText")]
	[Body("LogText", "Action")]
	public class LogText : ActionNode, IRegisterPorts {

		public InputValue<string> text;

		public void OnRegisterPorts() {
			text = RegisterInputValue<string>("text");
		}

		public override void OnExecute() {
			new log(text.value);
			Call(output);
		}
	}
}
