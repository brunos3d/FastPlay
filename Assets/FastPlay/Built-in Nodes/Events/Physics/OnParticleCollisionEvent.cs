using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnParticleCollision")]
	[Path("Events/Physics/OnParticleCollision")]
	public class OnParticleCollisionEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public GameObject other;

		public void OnRegisterEvents() {
			Current.controller.DoParticleCollision += OnParticleCollision;
		}

		public void OnRemoveEvents() {
			Current.controller.DoParticleCollision -= OnParticleCollision;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<GameObject>("other", () => { return other; });
		}

		public void OnParticleCollision(GameObject other) {
			this.other = other;
			Call(output);
		}
	}
}
