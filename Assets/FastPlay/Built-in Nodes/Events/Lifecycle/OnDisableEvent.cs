using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("DisableEvent Icon")]
	[Title("OnDisable")]
	[Path("Events/Lifecycle/OnDisable")]
	public class OnDisableEvent : EventNode {

		public void OnDisable() {
			Call(output);
		}
	}
}
