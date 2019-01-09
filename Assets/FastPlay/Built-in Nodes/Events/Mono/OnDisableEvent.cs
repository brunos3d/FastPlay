using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnDisable")]
	[Path("Events/Lifecycle/OnDisable")]
	[Body("OnDisable")]
	public class OnDisableEvent : EventNode {

		public void OnDisable() {
			Call(output);
		}
	}
}
