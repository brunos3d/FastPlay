using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnWillRenderObject")]
	[Path("Events/Rendering/OnWillRenderObject")]
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
