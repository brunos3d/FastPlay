using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnMouseDrag")]
	[Path("Events/Mouse/OnMouseDrag")]
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
