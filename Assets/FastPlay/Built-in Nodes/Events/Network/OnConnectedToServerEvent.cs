using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnConnectedToServer")]
	[Path("Events/Network/OnConnectedToServer")]
	public class OnConnectedToServerEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoConnectedToServer += OnConnectedToServer;
		}

		public void OnRemoveEvents() {
			Current.controller.DoConnectedToServer -= OnConnectedToServer;
		}

		public void OnConnectedToServer() {
			Call(output);
		}
	}
}
