using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnServerInitialized")]
	[Path("Events/Network/OnServerInitialized")]
	public class OnServerInitializedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoServerInitialized += OnServerInitialized;
		}

		public void OnRemoveEvents() {
			Current.controller.DoServerInitialized -= OnServerInitialized;
		}

		public void OnServerInitialized() {
			Call(output);
		}
	}
}
