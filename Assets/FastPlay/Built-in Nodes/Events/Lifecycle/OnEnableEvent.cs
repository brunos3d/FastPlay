using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("StartEventNode Icon")]
	[Title("OnEnable")]
	[Path("Events/Lifecycle/OnEnable")]
	public class OnEnableEvent : EventNode {

		public void OnEnable() {
			Call(output);
		}
	}
}
