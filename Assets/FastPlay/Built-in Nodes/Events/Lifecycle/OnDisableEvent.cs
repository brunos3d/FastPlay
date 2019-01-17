using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("DisableEventNode Icon")]
	[Title("OnDisable")]
	[Path("Events/Lifecycle/OnDisable")]
	[Summary("This function is called when the behaviour becomes disabled.")]
	public class OnDisableEvent : EventNode {

		public void OnDisable() {
			Call(output);
		}
	}
}
