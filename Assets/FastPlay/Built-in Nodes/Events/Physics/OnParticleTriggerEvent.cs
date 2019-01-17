using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnParticleTrigger")]
	[Path("Events/Physics/OnParticleTrigger")]
	[Summary("OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.")]
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
