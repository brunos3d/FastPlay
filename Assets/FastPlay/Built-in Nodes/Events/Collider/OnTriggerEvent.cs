using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnTrigger")]
	[Path("Events/Collider/OnTrigger")]
	[Body("OnTrigger")]
	public class OnTriggerEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public Collider collider;
		public OutputAction on_enter;
		public OutputAction on_stay;
		public OutputAction on_exit;

		public override bool useOut { get { return false; } }

		public void OnRegisterEvents() {
			Current.controller.DoTriggerEnter += OnTriggerEnter;
			Current.controller.DoTriggerStay += OnTriggerStay;
			Current.controller.DoTriggerExit += OnTriggerExit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoTriggerEnter -= OnTriggerEnter;
			Current.controller.DoTriggerStay -= OnTriggerStay;
			Current.controller.DoTriggerExit -= OnTriggerExit;
		}

		public void OnRegisterPorts() {
			on_enter = RegisterExitPort("Enter");
			on_stay = RegisterExitPort("Stay");
			on_exit = RegisterExitPort("Exit");
			RegisterOutputValue<Collider>("Collider", () => { return collider; });
		}

		public void OnTriggerEnter(Collider col) {
			collider = col;
			Call(on_enter);
		}

		public void OnTriggerStay(Collider col) {
			collider = col;
			Call(on_stay);
		}

		public void OnTriggerExit(Collider col) {
			collider = col;
			Call(on_exit);
		}
	}
}
