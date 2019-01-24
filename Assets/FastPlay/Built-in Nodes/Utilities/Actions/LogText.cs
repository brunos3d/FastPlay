using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("Debug Icon")]
	[Title("LogText")]
	[Subtitle("Action")]
	[Path("Utilities/Actions/LogText")]
	public class LogText : ActionNode, IRegisterPorts {

		public InputValue<string> text;

		public void OnRegisterPorts() {
			text = RegisterInputValue<string>("text");
		}

		public override void OnExecute() {
			Debug.Log(text.value, Current.controller);
			Call(output);
		}
	}
}
