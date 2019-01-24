using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("OnMouseDragEvent Icon")]
	[Title("OnMouseDrag")]
	[Path("Events/Mouse/OnMouseDrag")]
	[Summary("OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.")]
	public class OnMouseDragEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseDrag += OnMouseDrag;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseDrag -= OnMouseDrag;
		}

		public void OnMouseDrag() {
			Call(output);
		}
	}
}
