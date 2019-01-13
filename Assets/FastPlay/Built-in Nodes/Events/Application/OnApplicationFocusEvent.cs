using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnApplicationFocus")]
	[Path("Events/Application/OnApplicationFocus")]
	public class OnApplicationFocusEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public bool focus;

		public void OnRegisterEvents() {
			Current.controller.DoApplicationFocus += OnApplicationFocus;
		}

		public void OnRemoveEvents() {
			Current.controller.DoApplicationFocus -= OnApplicationFocus;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<bool>("focus", () => { return focus; });
		}

		public void OnApplicationFocus(bool focus) {
			this.focus = focus;
			Call(output);
		}
	}
}