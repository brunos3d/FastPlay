using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnBecameInvisible")]
	[Path("Events/Rendering/OnBecameInvisible")]
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
