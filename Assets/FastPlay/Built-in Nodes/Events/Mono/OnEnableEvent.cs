using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("OnEnable")]
	[Path("Events/Lifecycle/OnEnable")]
	[Body("OnEnable")]
	public class OnEnableEvent : EventNode {

		public void OnEnable() {
			Call(output);
		}
	}
}
