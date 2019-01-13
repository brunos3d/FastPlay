using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("UpdateEventNode Icon")]
	[Title("Update")]
	[Path("Events/Lifecycle/Update")]
	public class UpdateEvent : EventNode, IUpdate {

		public void Update() {
			Call(output);
		}
	}
}
