using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("UpdateEventNode Icon")]
	[Title("LateUpdate")]
	[Path("Events/Lifecycle/LateUpdate")]
	[Summary("LateUpdate is called every frame, if the Behaviour is enabled.")]
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
