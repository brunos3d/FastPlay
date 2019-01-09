using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Update")]
	[Path("Events/Lifecycle/Update")]
	[Body("Update")]
	public class UpdateEvent : EventNode, IUpdate {

		public void Update() {
			Call(output);
		}
	}
}
