using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnWillRenderObject")]
	[Path("Events/Rendering/OnWillRenderObject")]
	[Summary("OnWillRenderObject is called for each camera if the object is visible and not a UI element.")]
	public class OnWillRenderObjectEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoWillRenderObject += OnWillRenderObject;
		}

		public void OnRemoveEvents() {
			Current.controller.DoWillRenderObject -= OnWillRenderObject;
		}

		public void OnWillRenderObject() {
			Call(output);
		}
	}
}
