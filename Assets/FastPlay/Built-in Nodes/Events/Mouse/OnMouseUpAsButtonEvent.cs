using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnMouseUpAsButton")]
	[Path("Events/Mouse/OnMouseUpAsButton")]
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
