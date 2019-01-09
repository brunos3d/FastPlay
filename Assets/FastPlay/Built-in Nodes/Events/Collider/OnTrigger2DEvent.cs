using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnTrigger2D")]
	[Path("Events/Collider/OnTrigger2D")]
	[Body("OnTrigger2D")]
	public class OnTrigger2DEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public Collider2D collider;
		public OutputAction on_enter;
		public OutputAction on_stay;
		public OutputAction on_exit;

		public override bool useOut { get { return false; } }

		public void OnRegisterEvents() {
			Current.controller.DoTriggerEnter2D += OnTriggerEnter2D;
			Current.controller.DoTriggerStay2D += OnTriggerEnter2D;
			Current.controller.DoTriggerExit2D += OnTriggerEnter2D;
		}

		public void OnRemoveEvents() {
			Current.controller.DoTriggerEnter2D -= OnTriggerEnter2D;
			Current.controller.DoTriggerStay2D -= OnTriggerEnter2D;
			Current.controller.DoTriggerExit2D -= OnTriggerEnter2D;
		}

		public void OnRegisterPorts() {
			on_enter = RegisterExitPort("Enter");
			on_stay = RegisterExitPort("Stay");
			on_exit = RegisterExitPort("Exit");
			RegisterOutputValue<Collider2D>("Collider2D", () => { return collider; });
		}

		public void OnTriggerEnter2D(Collider2D col) {
			collider = col;
			Call(on_enter);
		}

		public void OnTriggerStay2D(Collider2D col) {
			collider = col;
			Call(on_stay);
		}

		public void OnTriggerExit2D(Collider2D col) {
			collider = col;
			Call(on_exit);
		}
	}
}
