using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Start")]
	[Path("Events/Lifecycle/Start")]
	[Body("Start")]
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
