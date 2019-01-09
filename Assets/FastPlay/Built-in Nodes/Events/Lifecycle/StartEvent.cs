using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("StartEvent Icon")]
	[Title("Start")]
	[Path("Events/Lifecycle/Start")]
	public class StartEvent : EventNode {

		public bool started;

		public void Start() {
			if (!started) {
				Call(output);
				started = true;
			}
		}
	}
}
