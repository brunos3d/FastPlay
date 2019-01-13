using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnJointBreak")]
	[Path("Events/Joint/OnJointBreak")]
	public class OnJointBreakEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public float breakForce;

		public void OnRegisterEvents() {
			Current.controller.DoJointBreak += OnJointBreak;
		}

		public void OnRemoveEvents() {
			Current.controller.DoJointBreak -= OnJointBreak;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<float>("breakForce", () => { return breakForce; });
		}

		public void OnJointBreak(float breakForce) {
			this.breakForce = breakForce;
			Call(output);
		}
	}
}
