using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnTransformChildrenChanged")]
	[Path("Events/Transform/OnTransformChildrenChanged")]
	public class OnTransformChildrenChangedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoTransformChildrenChanged += OnTransformChildrenChanged;
		}

		public void OnRemoveEvents() {
			Current.controller.DoTransformChildrenChanged -= OnTransformChildrenChanged;
		}

		public void OnTransformChildrenChanged() {
			Call(output);
		}
	}
}
