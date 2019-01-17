using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnGUI")]
	[Path("Events/Lifecycle/OnGUI")]
	[Summary("OnGUI is called for rendering and handling GUI events.")]
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
