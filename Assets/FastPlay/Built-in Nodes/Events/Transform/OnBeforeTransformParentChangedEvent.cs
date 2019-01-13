using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnBeforeTransformParentChanged")]
	[Path("Events/Transform/OnBeforeTransformParentChanged")]
	public class OnBeforeTransformParentChangedEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoBeforeTransformParentChanged += OnBeforeTransformParentChanged;
		}

		public void OnRemoveEvents() {
			Current.controller.DoBeforeTransformParentChanged -= OnBeforeTransformParentChanged;
		}

		public void OnBeforeTransformParentChanged() {
			Call(output);
		}
	}
}
