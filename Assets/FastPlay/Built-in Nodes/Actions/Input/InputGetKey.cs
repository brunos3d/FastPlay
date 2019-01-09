using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Input.GetKey")]
	[Path("Actions/Input/Input.GetKey")]
	[Body("GetKey", "Input")]
	public class InputGetKey : ValueNode<bool>, IRegisterPorts {

		public InputValue<KeyCode> key;

		public void OnRegisterPorts() {
			key = RegisterInputValue<KeyCode>("key");
		}

		public override bool OnGetValue() {
			return Input.GetKey(key.value);
		}
	}
}
