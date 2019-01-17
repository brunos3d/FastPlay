using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnBecameInvisible")]
	[Path("Events/Rendering/OnBecameInvisible")]
	[Summary("OnBecameInvisible is called when the renderer is no longer visible by any camera.")]
	public class OnBecameInvisibleEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoBecameInvisible += OnBecameInvisible;
		}

		public void OnRemoveEvents() {
			Current.controller.DoBecameInvisible -= OnBecameInvisible;
		}

		public void OnBecameInvisible() {
			Call(output);
		}
	}
}
