using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetKeyDown")]
	[Path("Actions/Input/Input.GetKeyDown")]
	[Body("GetKeyDown", "Input")]
	public class InputGetKeyDown : ValueNode<bool>, IRegisterPorts {

		public InputValue<KeyCode> key;

		public void OnRegisterPorts() {
			key = RegisterInputValue<KeyCode>("key");
		}

		public override bool OnGetValue() {
			return Input.GetKeyDown(key.value);
		}
	}
}
