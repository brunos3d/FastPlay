using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("StartEventNode Icon")]
	[Title("OnEnable")]
	[Path("Events/Lifecycle/OnEnable")]
	[Summary("This function is called when the object becomes enabled and active.")]
	public class OnEnableEvent : EventNode {

		public void OnEnable() {
			Call(output);
		}
	}
}
