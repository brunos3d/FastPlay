using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("update_event_icon")]
	[Title("Update")]
	[Path("Events/Lifecycle/Update")]
	[Summary("Update is called every frame, if the MonoBehaviour is enabled.")]
	public class UpdateEvent : EventNode, IUpdate {

		public void Update() {
			Call(output);
		}
	}
}
