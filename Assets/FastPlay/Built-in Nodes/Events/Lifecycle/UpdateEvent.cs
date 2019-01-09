using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("UpdateEvent Icon")]
	[Title("Update")]
	[Path("Events/Lifecycle/Update")]
	public class UpdateEvent : EventNode, IUpdate {

		public void Update() {
			Call(output);
		}
	}
}
