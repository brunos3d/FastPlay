using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnCollision")]
	[Path("Events/Physics/OnCollision")]
	public class OnCollisionEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public Collision collision;
		public OutputAction on_enter;
		public OutputAction on_stay;
		public OutputAction on_exit;

		public override bool useOut { get { return false; } }

		public void OnRegisterEvents() {
			Current.controller.DoCollisionEnter += OnCollisionEnter;
			Current.controller.DoCollisionStay += OnCollisionStay;
			Current.controller.DoCollisionExit += OnCollisionExit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoCollisionEnter -= OnCollisionEnter;
			Current.controller.DoCollisionStay -= OnCollisionStay;
			Current.controller.DoCollisionExit -= OnCollisionExit;
		}

		public void OnRegisterPorts() {
			on_enter = RegisterExitPort("Enter");
			on_stay = RegisterExitPort("Stay");
			on_exit = RegisterExitPort("Exit");
			RegisterOutputValue<Collision>("Collision", () => { return collision; });
		}

		public void OnCollisionEnter(Collision col) {
			collision = col;
			Call(on_enter);
		}

		public void OnCollisionStay(Collision col) {
			collision = col;
			Call(on_stay);
		}

		public void OnCollisionExit(Collision col) {
			collision = col;
			Call(on_exit);
		}
	}
}
