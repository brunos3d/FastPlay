using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseUp")]
	[Path("Events/Mouse/OnMouseUp")]
	[Body("OnMouseUp")]
	public class OnMouseUpEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseUp += OnMouseUp;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseUp -= OnMouseUp;
		}

		public void OnMouseUp() {
			Call(output);
		}
	}
}
