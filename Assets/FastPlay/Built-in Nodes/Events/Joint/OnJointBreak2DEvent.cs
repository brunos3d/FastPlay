using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnJointBreak2D")]
	[Path("Events/Joint/OnJointBreak2D")]
	[Summary("Called when a Joint2D attached to the same game object breaks.")]
	public class OnJointBreak2DEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public Joint2D joint;

		public void OnRegisterEvents() {
			Current.controller.DoJointBreak2D += OnJointBreak2D;
		}

		public void OnRemoveEvents() {
			Current.controller.DoJointBreak2D -= OnJointBreak2D;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<Joint2D>("joint", () => { return joint; });
		}

		public void OnJointBreak2D(Joint2D joint) {
			this.joint = joint;
			Call(output);
		}
	}
}
