using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseDown")]
	[Path("Events/Mouse/OnMouseDown")]
	[Body("OnMouseDown")]
	public class OnMouseDownEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseDown += OnMouseDown;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseDown -= OnMouseDown;
		}

		public void OnMouseDown() {
			Call(output);
		}
	}
}
