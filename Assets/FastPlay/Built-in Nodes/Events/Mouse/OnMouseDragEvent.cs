using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseDrag")]
	[Path("Events/Mouse/OnMouseDrag")]
	[Body("OnMouseDrag")]
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
