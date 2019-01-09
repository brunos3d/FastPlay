using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("LateUpdate")]
	[Path("Events/Lifecycle/LateUpdate")]
	[Body("LateUpdate")]
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
