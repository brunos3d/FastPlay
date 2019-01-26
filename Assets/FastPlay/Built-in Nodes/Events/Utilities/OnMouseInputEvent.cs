using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("Mouse Icon")]
	[Title("OnMouseInput")]
	[Path("Events/Utilities/OnMouseInput")]
	public class OnMouseInputEvent : EventNode, IUpdate, IRegisterPorts {

		public InputValue<MouseButton> button;
		public InputValue<InputActionMode> mode;

		public void OnRegisterPorts() {
			button = RegisterInputValue<MouseButton>("Button", MouseButton.Left);
			mode = RegisterInputValue<InputActionMode>("Action");
		}

		public void Update() {
			switch (mode.value) {
				case InputActionMode.Down:
					if (Input.GetMouseButtonDown((int)button.value)) {
						Call(output);
					}
					break;
				case InputActionMode.Update:
					if (Input.GetMouseButton((int)button.value)) {
						Call(output);
					}
					break;
				case InputActionMode.Up:
					if (Input.GetMouseButtonUp((int)button.value)) {
						Call(output);
					}
					break;
			}
		}
	}
}
