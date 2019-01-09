using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnMouseOver")]
	[Path("Events/Mouse/OnMouseOver")]
	[Body("OnMouseOver")]
	public class OnMouseOverEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoMouseOver += OnMouseOver;
		}

		public void OnRemoveEvents() {
			Current.controller.DoMouseOver -= OnMouseOver;
		}

		public void OnMouseOver() {
			Call(output);
		}
	}
}
