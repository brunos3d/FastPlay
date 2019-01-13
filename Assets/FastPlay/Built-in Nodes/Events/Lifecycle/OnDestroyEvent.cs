using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("DisableEvent Icon")]
	[Title("OnDestroy")]
	[Path("Events/Lifecycle/OnDestroy")]
	public class OnDestroyEvent : EventNode {

		public void OnRegisterEvents() {
			Current.controller.DoDestroy += OnDestroy;
		}

		public void OnRemoveEvents() {
			Current.controller.DoDestroy -= OnDestroy;
		}

		public void OnDestroy() {
			Call(output);
		}
	}
}
