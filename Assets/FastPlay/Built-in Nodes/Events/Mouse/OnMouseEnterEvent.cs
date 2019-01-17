using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnMouseEnter")]
	[Path("Events/Mouse/OnMouseEnter")]
	[Summary("Called when the mouse enters the GUIElement or Collider.")]
	public class OnMouseEnterEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseEnter += OnMouseEnter;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseEnter -= OnMouseEnter;
		}

		public void OnMouseEnter() {
			Call(output);
		}
	}
}
