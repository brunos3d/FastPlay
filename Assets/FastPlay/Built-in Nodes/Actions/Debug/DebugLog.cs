using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Debug.Log")]
	[Path("Actions/Debug/Debug.Log")]
	[Body("Log", "Debug")]
	public class DebugLog : ActionNode, IRegisterPorts {

		public InputValue<object> message;

		public void OnRegisterPorts() {
			message = RegisterInputValue<object>("message");
		}

		public override void OnExecute() {
			Debug.Log(message.value);
			Call(output);
		}
	}
}
