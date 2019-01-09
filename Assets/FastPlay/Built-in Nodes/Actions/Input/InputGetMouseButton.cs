using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetMouseButton")]
	[Path("Actions/Input/Input.GetMouseButton")]
	[Body("GetMouseButton", "Input")]
	public class InputGetMouseButton : ValueNode<bool>, IRegisterPorts {

		public InputValue<int> button;

		public void OnRegisterPorts() {
			button = RegisterInputValue<int>("button");
		}

		public override bool OnGetValue() {
			return Input.GetMouseButton(button.value);
		}
	}
}
