using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnTransformParentChanged")]
	[Path("Events/Transform/OnTransformParentChanged")]
	public class OnTransformParentChangedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoTransformParentChanged += OnTransformParentChanged;
		}

		public void OnRemoveEvents() {
			Current.controller.DoTransformParentChanged -= OnTransformParentChanged;
		}

		public void OnTransformParentChanged() {
			Call(output);
		}
	}
}
