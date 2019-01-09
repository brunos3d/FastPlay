using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("FixedUpdate")]
	[Path("Events/Lifecycle/FixedUpdate")]
	public class FixedUpdateEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoFixedUpdate += FixedUpdate;
		}

		public void OnRemoveEvents() {
			Current.controller.DoFixedUpdate -= FixedUpdate;
		}

		public void FixedUpdate() {
			Call(output);
		}
	}
}
