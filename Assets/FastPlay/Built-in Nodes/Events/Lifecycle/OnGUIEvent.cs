using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnGUI")]
	[Path("Events/Lifecycle/OnGUI")]
	public class OnGUIEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoGUI += OnGUI;
		}

		public void OnRemoveEvents() {
			Current.controller.DoGUI -= OnGUI;
		}

		public void OnGUI() {
			Call(output);
		}
	}
}
