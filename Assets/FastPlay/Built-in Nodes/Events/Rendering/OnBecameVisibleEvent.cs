using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnBecameVisible")]
	[Path("Events/Rendering/OnBecameVisible")]
	public class OnBecameVisibleEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoBecameVisible += OnBecameVisible;
		}

		public void OnRemoveEvents() {
			Current.controller.DoBecameVisible -= OnBecameVisible;
		}

		public void OnBecameVisible() {
			Call(output);
		}
	}
}
