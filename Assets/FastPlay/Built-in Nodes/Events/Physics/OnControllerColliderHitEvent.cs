using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnControllerColliderHit")]
	[Path("Events/Physics/OnControllerColliderHit")]
	public class OnControllerColliderHitEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public ControllerColliderHit hit;

		public void OnRegisterEvents() {
			Current.controller.DoControllerColliderHit += OnControllerColliderHit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoControllerColliderHit -= OnControllerColliderHit;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<ControllerColliderHit>("hit", () => { return this.hit; });
		}

		public void OnControllerColliderHit(ControllerColliderHit hit) {
			this.hit = hit;
			Call(output);
		}
	}
}