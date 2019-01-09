using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("LateUpdate")]
	[Path("Events/Lifecycle/LateUpdate")]
	public class LateUpdateEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoLateUpdate += LateUpdate;
		}

		public void OnRemoveEvents() {
			Current.controller.DoLateUpdate -= LateUpdate;
		}

		public void LateUpdate() {
			Call(output);
		}
	}
}
