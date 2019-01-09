using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseEnter")]
	[Path("Events/Mouse/OnMouseEnter")]
	[Body("OnMouseEnter")]
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
