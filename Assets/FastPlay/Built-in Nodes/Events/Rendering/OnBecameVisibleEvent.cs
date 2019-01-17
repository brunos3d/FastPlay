using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnBecameVisible")]
	[Path("Events/Rendering/OnBecameVisible")]
	[Summary("OnBecameVisible is called when the renderer became visible by any camera.")]
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
