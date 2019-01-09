using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetKeyUp")]
	[Path("Actions/Input/Input.GetKeyUp")]
	[Body("GetKeyUp", "Input")]
	public class InputGetKeyUp : ValueNode<bool>, IRegisterPorts {

		public InputValue<KeyCode> key;

		public void OnRegisterPorts() {
			key = RegisterInputValue<KeyCode>("key");
		}

		public override bool OnGetValue() {
			return Input.GetKeyUp(key.value);
		}
	}
}
