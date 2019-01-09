using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnCollision2D")]
	[Path("Events/Collider/OnCollision2D")]
	public class OnCollision2DEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public Collision2D collision;
		public OutputAction on_enter;
		public OutputAction on_stay;
		public OutputAction on_exit;

		public override bool useOut { get { return false; } }

		public void OnRegisterEvents() {
			Current.controller.DoCollisionEnter2D += OnCollisionEnter2D;
			Current.controller.DoCollisionStay2D += OnCollisionStay2D;
			Current.controller.DoCollisionExit2D += OnCollisionExit2D;
		}

		public void OnRemoveEvents() {
			Current.controller.DoCollisionEnter2D -= OnCollisionEnter2D;
			Current.controller.DoCollisionStay2D -= OnCollisionStay2D;
			Current.controller.DoCollisionExit2D -= OnCollisionExit2D;
		}

		public void OnRegisterPorts() {
			on_enter = RegisterExitPort("Enter");
			on_stay = RegisterExitPort("Stay");
			on_exit = RegisterExitPort("Exit");
			RegisterOutputValue<Collision2D>("Collision2D", () => { return collision; });
		}

		public void OnCollisionEnter2D(Collision2D col) {
			collision = col;
			Call(on_enter);
		}

		public void OnCollisionStay2D(Collision2D col) {
			collision = col;
			Call(on_stay);
		}

		public void OnCollisionExit2D(Collision2D col) {
			collision = col;
			Call(on_exit);
		}
	}
}
