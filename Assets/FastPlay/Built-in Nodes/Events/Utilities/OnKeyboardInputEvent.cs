using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnKeyboardInputEvent")]
	[Path("Events/Utilities/OnKeyboardInput")]
	[Body("OnKeyboardInput")]
	public class OnKeyboardInputEvent : EventNode, IUpdate, IRegisterPorts {

		public InputValue<KeyCode> key;
		public InputValue<InputActionMode> mode;

		public void OnRegisterPorts() {
			key = RegisterInputValue<KeyCode>("Key", KeyCode.Space);
			mode = RegisterInputValue<InputActionMode>("Action");
		}

		public void Update() {
			switch (mode.value) {
				case InputActionMode.Down:
					if (Input.GetKeyDown(key.value)) {
						Call(output);
					}
					break;
				case InputActionMode.Update:
					if (Input.GetKey(key.value)) {
						Call(output);
					}
					break;
				case InputActionMode.Up:
					if (Input.GetKeyUp(key.value)) {
						Call(output);
					}
					break;
			}
		}
	}
}
