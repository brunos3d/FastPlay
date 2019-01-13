using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnApplicationQuit")]
	[Path("Events/Application/OnApplicationQuit")]
	public class OnApplicationQuitEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoApplicationQuit += OnApplicationQuit;
		}

		public void OnRemoveEvents() {
			Current.controller.DoApplicationQuit -= OnApplicationQuit;
		}

		public void OnApplicationQuit() {
			Call(output);
		}
	}
}