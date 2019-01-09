using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetMouseButtonUp")]
	[Path("Actions/Input/Input.GetMouseButtonUp")]
	[Body("GetMouseButtonUp", "Input")]
	public class InputGetMouseButtonUp : ValueNode<bool>, IRegisterPorts {

		public InputValue<int> button;

		public void OnRegisterPorts() {
			button = RegisterInputValue<int>("button");
		}

		public override bool OnGetValue() {
			return Input.GetMouseButtonUp(button.value);
		}
	}
}
