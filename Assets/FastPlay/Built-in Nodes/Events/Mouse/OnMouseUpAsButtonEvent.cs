using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseUpAsButton")]
	[Path("Events/Mouse/OnMouseUpAsButton")]
	[Body("OnMouseUpAsButton")]
	public class OnMouseUpAsButtonEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseUpAsButton += OnMouseUpAsButton;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseUpAsButton -= OnMouseUpAsButton;
		}

		public void OnMouseUpAsButton() {
			Call(output);
		}
	}
}
