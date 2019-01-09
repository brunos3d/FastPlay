using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseExit")]
	[Path("Events/Mouse/OnMouseExit")]
	[Body("OnMouseExit")]
	public class OnMouseExitEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseExit += OnMouseExit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseExit -= OnMouseExit;
		}

		public void OnMouseExit() {
			Call(output);
		}
	}
}
