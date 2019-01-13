using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnParticleTrigger")]
	[Path("Events/Physics/OnParticleTrigger")]
	public class OnParticleTriggerEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoParticleTrigger += OnParticleTrigger;
		}

		public void OnRemoveEvents() {
			Current.controller.DoParticleTrigger -= OnParticleTrigger;
		}

		public void OnParticleTrigger() {
			Call(output);
		}
	}
}
