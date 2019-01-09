using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetMouseButtonDown")]
	[Path("Actions/Input/Input.GetMouseButtonDown")]
	[Body("GetMouseButtonDown", "Input")]
	public class InputGetMouseButtonDown : ValueNode<bool>, IRegisterPorts {

		public InputValue<int> button;

		public void OnRegisterPorts() {
			button = RegisterInputValue<int>("button");
		}

		public override bool OnGetValue() {
			return Input.GetMouseButtonDown(button.value);
		}
	}
}
