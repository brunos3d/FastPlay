using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("StartEventNode Icon")]
	[Title("Start")]
	[Path("Events/Lifecycle/Start")]
	[Summary("Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.")]
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
